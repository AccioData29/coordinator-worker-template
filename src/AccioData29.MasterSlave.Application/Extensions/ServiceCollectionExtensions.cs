using AccioData29.CoordinatorWorker.Application.Services;
using AccioData29.CoordinatorWorker.Application.Strategies;

namespace AccioData29.CoordinatorWorker.Application.Extensions;

/// <summary>
/// Extensiones de <see cref="IServiceCollection"/> para registrar el sistema Coordinator-Worker
/// en el contenedor de inyección de dependencias de .NET.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra el Coordinator, la estrategia Round-Robin y los Workers configurados.
    ///
    /// Uso:
    /// <code>
    /// services.AddCoordinatorWorker&lt;int, int&gt;(options =>
    /// {
    ///     options.AddWorker&lt;MiPrimerWorker&gt;();
    ///     options.AddWorker&lt;MiSegundoWorker&gt;();
    /// });
    /// </code>
    /// </summary>
    /// <typeparam name="TInput">Tipo del dato de entrada.</typeparam>
    /// <typeparam name="TOutput">Tipo del resultado producido.</typeparam>
    /// <param name="services">Contenedor de servicios de la aplicación.</param>
    /// <param name="configure">Acción para configurar los Workers a registrar.</param>
    public static IServiceCollection AddCoordinatorWorker<TInput, TOutput>(
        this IServiceCollection services,
        Action<CoordinatorWorkerOptions<TInput, TOutput>>? configure = null)
    {
        var options = new CoordinatorWorkerOptions<TInput, TOutput>();
        configure?.Invoke(options);

        services.AddScoped<IWorkDistributor<TInput, TOutput>, RoundRobinDistributor<TInput, TOutput>>();

        foreach (var workerType in options.WorkerTypes)
            services.AddScoped(typeof(IWorker<TInput, TOutput>), workerType);

        services.AddScoped<ICoordinator<TInput, TOutput>, CoordinatorService<TInput, TOutput>>();

        return services;
    }
}
