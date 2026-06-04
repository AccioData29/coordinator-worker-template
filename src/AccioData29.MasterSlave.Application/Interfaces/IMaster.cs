namespace AccioData29.MasterSlave.Application.Interfaces;

/// <summary>
/// Puerto de entrada del patrón Master-Slave.
/// El Master es el único punto de contacto externo: recibe todas las tareas,
/// las distribuye a los Slaves y agrega los resultados.
///
/// SOLID:
///   - SRP: coordina la ejecución; no procesa tareas directamente.
///   - DIP: depende de <see cref="ISlave{TInput,TOutput}"/> e
///          <see cref="IWorkDistributor{TInput,TOutput}"/>, no de implementaciones concretas.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada de cada tarea.</typeparam>
/// <typeparam name="TOutput">Tipo del resultado producido por cada Slave.</typeparam>
public interface IMaster<TInput, TOutput>
{
    /// <summary>
    /// Distribuye las tareas entre los Slaves disponibles, ejecuta el
    /// procesamiento en paralelo y retorna los resultados agregados.
    /// </summary>
    /// <param name="workItems">Colección de tareas a procesar.</param>
    /// <param name="cancellationToken">Token para cancelación cooperativa.</param>
    /// <returns>Un resultado por cada WorkItem, exitoso o fallido.</returns>
    /// <exception cref="NoAvailableSlavesException">
    /// Si no hay Slaves con estado <see cref="WorkerStatus.Available"/>.
    /// </exception>
    Task<IEnumerable<WorkResult<TInput, TOutput>>> ExecuteAsync(
        IEnumerable<WorkItem<TInput>> workItems,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un Slave manualmente. Útil en escenarios sin inyección de dependencias.
    /// </summary>
    /// <exception cref="ArgumentNullException">Si slave es null.</exception>
    /// <exception cref="InvalidOperationException">Si ya existe un Slave con el mismo <see cref="ISlave{TInput,TOutput}.SlaveId"/>.</exception>
    void RegisterSlave(ISlave<TInput, TOutput> slave);

    /// <summary>Retorna la lista de Slaves registrados en este Master.</summary>
    IReadOnlyList<ISlave<TInput, TOutput>> GetSlaves();
}
