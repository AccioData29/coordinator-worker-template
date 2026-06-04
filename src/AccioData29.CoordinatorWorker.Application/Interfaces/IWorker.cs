namespace AccioData29.CoordinatorWorker.Application.Interfaces;

/// <summary>
/// Puerto de salida del patrón Coordinator-Worker.
/// Cada implementación representa un nodo trabajador independiente que
/// recibe una tarea del Coordinator y retorna un resultado.
///
/// SOLID:
///   - SRP: solo procesa tareas, no coordina ni distribuye.
///   - LSP: cualquier implementación es intercambiable sin cambiar el Coordinator.
///   - ISP: interfaz mínima — solo lo necesario para ser un Worker.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada a procesar.</typeparam>
/// <typeparam name="TOutput">Tipo del resultado producido.</typeparam>
public interface IWorker<TInput, TOutput>
{
    /// <summary>
    /// Identificador único e inmutable del Worker dentro del sistema.
    /// El Coordinator lo usa para trazabilidad y detección de duplicados.
    /// </summary>
    string WorkerId { get; }

    /// <summary>
    /// Estado actual del Worker. El Coordinator solo asigna tareas a workers
    /// con estado <see cref="WorkerStatus.Available"/>.
    /// </summary>
    WorkerStatus Status { get; }

    /// <summary>
    /// Procesa una unidad de trabajo y retorna un resultado.
    /// Las implementaciones deben capturar sus propias excepciones internas
    /// y retornar <see cref="WorkResult{TInput,TOutput}.Failure"/> en vez de lanzarlas,
    /// para no interrumpir el procesamiento paralelo de otras tareas.
    /// </summary>
    /// <param name="workItem">Tarea asignada por el Coordinator.</param>
    /// <param name="cancellationToken">Token para cancelación cooperativa.</param>
    Task<WorkResult<TInput, TOutput>> ProcessAsync(
        WorkItem<TInput> workItem,
        CancellationToken cancellationToken = default);
}
