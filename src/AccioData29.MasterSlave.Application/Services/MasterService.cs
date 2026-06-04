namespace AccioData29.MasterSlave.Application.Services;

/// <summary>
/// Implementación del nodo Master del patrón Master-Slave.
///
/// Responsabilidades:
///   1. Mantener el registro de Slaves disponibles.
///   2. Delegar la distribución de tareas al <see cref="IWorkDistributor{TInput,TOutput}"/>.
///   3. Ejecutar el procesamiento en paralelo con <c>Task.WhenAll</c>.
///   4. Agregar y retornar todos los resultados al llamador.
///
/// SOLID:
///   - SRP: solo coordina; no procesa tareas ni decide cómo distribuirlas.
///   - DIP: depende de <see cref="IWorkDistributor{TInput,TOutput}"/> e
///          <see cref="ISlave{TInput,TOutput}"/>, inyectados por constructor.
/// </summary>
public sealed class MasterService<TInput, TOutput> : IMaster<TInput, TOutput>
{
    private readonly IWorkDistributor<TInput, TOutput> _distributor;
    private readonly List<ISlave<TInput, TOutput>> _slaves;

    /// <summary>
    /// Constructor para uso con inyección de dependencias.
    /// Los Slaves se registran en el contenedor DI y se inyectan automáticamente.
    /// </summary>
    /// <param name="distributor">Estrategia de distribución de tareas.</param>
    /// <param name="slaves">Slaves registrados en el contenedor DI.</param>
    public MasterService(
        IWorkDistributor<TInput, TOutput> distributor,
        IEnumerable<ISlave<TInput, TOutput>> slaves)
    {
        ArgumentNullException.ThrowIfNull(distributor);
        ArgumentNullException.ThrowIfNull(slaves);
        _distributor = distributor;
        _slaves = slaves.ToList();
    }

    /// <inheritdoc/>
    public void RegisterSlave(ISlave<TInput, TOutput> slave)
    {
        ArgumentNullException.ThrowIfNull(slave);

        if (_slaves.Any(s => s.SlaveId == slave.SlaveId))
            throw new InvalidOperationException(
                $"Ya existe un Slave registrado con el Id '{slave.SlaveId}'.");

        _slaves.Add(slave);
    }

    /// <inheritdoc/>
    public IReadOnlyList<ISlave<TInput, TOutput>> GetSlaves() =>
        _slaves.AsReadOnly();

    /// <inheritdoc/>
    public async Task<IEnumerable<WorkResult<TInput, TOutput>>> ExecuteAsync(
        IEnumerable<WorkItem<TInput>> workItems,
        CancellationToken cancellationToken = default)
    {
        var itemList = workItems.ToList();

        if (itemList.Count == 0)
            return Enumerable.Empty<WorkResult<TInput, TOutput>>();

        var availableSlaves = _slaves
            .Where(s => s.Status == WorkerStatus.Available)
            .ToList()
            .AsReadOnly();

        if (availableSlaves.Count == 0)
            throw new NoAvailableSlavesException(
                "No hay Slaves disponibles para procesar las tareas. " +
                "Verificá que al menos un Slave tenga estado Available.");

        var assignments = _distributor.Distribute(itemList, availableSlaves);

        // Procesamiento paralelo: todos los Slaves trabajan al mismo tiempo.
        var results = await Task.WhenAll(
            assignments.Select(a => a.Slave.ProcessAsync(a.WorkItem, cancellationToken))
        );

        return results;
    }
}
