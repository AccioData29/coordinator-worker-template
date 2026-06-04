namespace AccioData29.CoordinatorWorker.Application.Services;

/// <summary>
/// Implementación del nodo Coordinator del patrón Coordinator-Worker.
///
/// Responsabilidades:
///   1. Mantener el registro de Workers disponibles.
///   2. Delegar la distribución de tareas al <see cref="IWorkDistributor{TInput,TOutput}"/>.
///   3. Ejecutar el procesamiento en paralelo con <c>Task.WhenAll</c>.
///   4. Agregar y retornar todos los resultados al llamador.
///
/// SOLID:
///   - SRP: solo coordina; no procesa tareas ni decide cómo distribuirlas.
///   - DIP: depende de <see cref="IWorkDistributor{TInput,TOutput}"/> e
///          <see cref="IWorker{TInput,TOutput}"/>, inyectados por constructor.
/// </summary>
public sealed class CoordinatorService<TInput, TOutput> : ICoordinator<TInput, TOutput>
{
    private readonly IWorkDistributor<TInput, TOutput> _distributor;
    private readonly List<IWorker<TInput, TOutput>> _workers;

    /// <summary>
    /// Constructor para uso con inyección de dependencias.
    /// Los Workers se registran en el contenedor DI y se inyectan automáticamente.
    /// </summary>
    /// <param name="distributor">Estrategia de distribución de tareas.</param>
    /// <param name="workers">Workers registrados en el contenedor DI.</param>
    public CoordinatorService(
        IWorkDistributor<TInput, TOutput> distributor,
        IEnumerable<IWorker<TInput, TOutput>> workers)
    {
        ArgumentNullException.ThrowIfNull(distributor);
        ArgumentNullException.ThrowIfNull(workers);
        _distributor = distributor;
        _workers = workers.ToList();
    }

    /// <inheritdoc/>
    public void RegisterWorker(IWorker<TInput, TOutput> worker)
    {
        ArgumentNullException.ThrowIfNull(worker);

        if (_workers.Any(w => w.WorkerId == worker.WorkerId))
            throw new InvalidOperationException(
                $"Ya existe un Worker registrado con el Id '{worker.WorkerId}'.");

        _workers.Add(worker);
    }

    /// <inheritdoc/>
    public IReadOnlyList<IWorker<TInput, TOutput>> GetWorkers() =>
        _workers.AsReadOnly();

    /// <inheritdoc/>
    public async Task<IEnumerable<WorkResult<TInput, TOutput>>> ExecuteAsync(
        IEnumerable<WorkItem<TInput>> workItems,
        CancellationToken cancellationToken = default)
    {
        var itemList = workItems.ToList();

        if (itemList.Count == 0)
            return Enumerable.Empty<WorkResult<TInput, TOutput>>();

        var availableWorkers = _workers
            .Where(w => w.Status == WorkerStatus.Available)
            .ToList()
            .AsReadOnly();

        if (availableWorkers.Count == 0)
            throw new NoAvailableWorkersException(
                "No hay Workers disponibles para procesar las tareas. " +
                "Verificá que al menos un Worker tenga estado Available.");

        var assignments = _distributor.Distribute(itemList, availableWorkers);

        // Procesamiento paralelo: todos los Workers trabajan al mismo tiempo.
        var results = await Task.WhenAll(
            assignments.Select(a => a.Worker.ProcessAsync(a.WorkItem, cancellationToken))
        );

        return results;
    }
}
