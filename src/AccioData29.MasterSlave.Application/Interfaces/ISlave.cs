namespace AccioData29.MasterSlave.Application.Interfaces;

/// <summary>
/// Puerto de salida del patrón Master-Slave.
/// Cada implementación representa un nodo trabajador independiente que
/// recibe una tarea del Master y retorna un resultado.
///
/// SOLID:
///   - SRP: solo procesa tareas, no coordina ni distribuye.
///   - LSP: cualquier implementación es intercambiable sin cambiar el Master.
///   - ISP: interfaz mínima — solo lo necesario para ser un Slave.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada a procesar.</typeparam>
/// <typeparam name="TOutput">Tipo del resultado producido.</typeparam>
public interface ISlave<TInput, TOutput>
{
    /// <summary>
    /// Identificador único e inmutable del Slave dentro del sistema.
    /// El Master lo usa para trazabilidad y detección de duplicados.
    /// </summary>
    string SlaveId { get; }

    /// <summary>
    /// Estado actual del Slave. El Master solo asigna tareas a slaves
    /// con estado <see cref="WorkerStatus.Available"/>.
    /// </summary>
    WorkerStatus Status { get; }

    /// <summary>
    /// Procesa una unidad de trabajo y retorna un resultado.
    /// Las implementaciones deben capturar sus propias excepciones internas
    /// y retornar <see cref="WorkResult{TInput,TOutput}.Failure"/> en vez de lanzarlas,
    /// para no interrumpir el procesamiento paralelo de otras tareas.
    /// </summary>
    /// <param name="workItem">Tarea asignada por el Master.</param>
    /// <param name="cancellationToken">Token para cancelación cooperativa.</param>
    Task<WorkResult<TInput, TOutput>> ProcessAsync(
        WorkItem<TInput> workItem,
        CancellationToken cancellationToken = default);
}
