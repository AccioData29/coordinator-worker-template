namespace AccioData29.CoordinatorWorker.Application.Options;

/// <summary>
/// Opciones de configuración del sistema Coordinator-Worker para el contenedor DI.
/// Se usa a través del método de extensión
/// <c>services.AddCoordinatorWorker&lt;TInput, TOutput&gt;(options => ...)</c>.
/// </summary>
/// <typeparam name="TInput">Tipo de dato de entrada.</typeparam>
/// <typeparam name="TOutput">Tipo de dato de salida.</typeparam>
public sealed class CoordinatorWorkerOptions<TInput, TOutput>
{
    internal List<Type> WorkerTypes { get; } = new();

    /// <summary>
    /// Registra un tipo concreto de Worker para ser inyectado en el Coordinator.
    /// Puede llamarse múltiples veces para agregar varios Workers.
    /// </summary>
    /// <typeparam name="TWorker">Implementación concreta de <see cref="IWorker{TInput,TOutput}"/>.</typeparam>
    public CoordinatorWorkerOptions<TInput, TOutput> AddWorker<TWorker>()
        where TWorker : class, IWorker<TInput, TOutput>
    {
        WorkerTypes.Add(typeof(TWorker));
        return this;
    }
}
