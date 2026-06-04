namespace AccioData29.CoordinatorWorker.Application.Strategies;

/// <summary>
/// Estrategia de distribución Round-Robin: asigna tareas a los Workers
/// de forma cíclica para garantizar una carga equitativa.
///
/// Ejemplo con 3 tareas y 2 workers:
///   Tarea 0 → Worker 0
///   Tarea 1 → Worker 1
///   Tarea 2 → Worker 0  (vuelve al inicio)
///
/// SOLID — OCP: es una implementación concreta de <see cref="IWorkDistributor{TInput,TOutput}"/>.
/// Para cambiar la estrategia (ej. aleatoria, por peso) basta agregar
/// otra clase que implemente la misma interfaz.
/// </summary>
public sealed class RoundRobinDistributor<TInput, TOutput> : IWorkDistributor<TInput, TOutput>
{
    /// <inheritdoc/>
    public IEnumerable<(WorkItem<TInput> WorkItem, IWorker<TInput, TOutput> Worker)> Distribute(
        IEnumerable<WorkItem<TInput>> workItems,
        IReadOnlyList<IWorker<TInput, TOutput>> workers)
    {
        ArgumentNullException.ThrowIfNull(workItems);
        ArgumentNullException.ThrowIfNull(workers);

        return workItems.Select((item, index) =>
            (item, workers[index % workers.Count]));
    }
}
