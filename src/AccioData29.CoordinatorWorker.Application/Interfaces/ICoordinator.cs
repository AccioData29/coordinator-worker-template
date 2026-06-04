namespace AccioData29.CoordinatorWorker.Application.Interfaces;

/// <summary>
/// Puerto de entrada del patrón Coordinator-Worker.
/// El Coordinator es el único punto de contacto externo: recibe todas las tareas,
/// las distribuye a los Workers y agrega los resultados.
///
/// SOLID:
///   - SRP: coordina la ejecución; no procesa tareas directamente.
///   - DIP: depende de <see cref="IWorker{TInput,TOutput}"/> e
///          <see cref="IWorkDistributor{TInput,TOutput}"/>, no de implementaciones concretas.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada de cada tarea.</typeparam>
/// <typeparam name="TOutput">Tipo del resultado producido por cada Worker.</typeparam>
public interface ICoordinator<TInput, TOutput>
{
    /// <summary>
    /// Distribuye las tareas entre los Workers disponibles, ejecuta el
    /// procesamiento en paralelo y retorna los resultados agregados.
    /// </summary>
    /// <param name="workItems">Colección de tareas a procesar.</param>
    /// <param name="cancellationToken">Token para cancelación cooperativa.</param>
    /// <returns>Un resultado por cada WorkItem, exitoso o fallido.</returns>
    /// <exception cref="NoAvailableWorkersException">
    /// Si no hay Workers con estado <see cref="WorkerStatus.Available"/>.
    /// </exception>
    Task<IEnumerable<WorkResult<TInput, TOutput>>> ExecuteAsync(
        IEnumerable<WorkItem<TInput>> workItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un Worker manualmente. Útil en escenarios sin inyección de dependencias.
    /// </summary>
    /// <exception cref="ArgumentNullException">Si worker es null.</exception>
    /// <exception cref="InvalidOperationException">Si ya existe un Worker con el mismo <see cref="IWorker{TInput,TOutput}.WorkerId"/>.</exception>
    void RegisterWorker(IWorker<TInput, TOutput> worker);

    /// <summary>Retorna la lista de Workers registrados en este Coordinator.</summary>
    IReadOnlyList<IWorker<TInput, TOutput>> GetWorkers();
}
