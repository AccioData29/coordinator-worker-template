namespace AccioData29.CoordinatorWorker.Application.Interfaces;

/// <summary>
/// Estrategia de distribución de tareas entre Workers (patrón Strategy).
/// Permite cambiar el algoritmo de distribución sin modificar el Coordinator.
///
/// SOLID:
///   - OCP: nuevas estrategias se agregan implementando esta interfaz,
///          sin tocar <see cref="ICoordinator{TInput,TOutput}"/> ni sus implementaciones.
///   - SRP: solo decide qué Worker recibe qué tarea.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada.</typeparam>
/// <typeparam name="TOutput">Tipo del resultado esperado.</typeparam>
public interface IWorkDistributor<TInput, TOutput>
{
    /// <summary>
    /// Asigna cada <see cref="WorkItem{T}"/> a un Worker disponible.
    /// </summary>
    /// <param name="workItems">Tareas a distribuir.</param>
    /// <param name="workers">Workers disponibles en el momento de la distribución.</param>
    /// <returns>Pares (tarea, worker) listos para ejecución paralela.</returns>
    IEnumerable<(WorkItem<TInput> WorkItem, IWorker<TInput, TOutput> Worker)> Distribute(
        IEnumerable<WorkItem<TInput>> workItems,
        IReadOnlyList<IWorker<TInput, TOutput>> workers);
}
