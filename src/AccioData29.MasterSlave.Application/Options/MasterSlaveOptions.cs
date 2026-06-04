namespace AccioData29.MasterSlave.Application.Options;

/// <summary>
/// Opciones de configuración del sistema Master-Slave para el contenedor DI.
/// Se usa a través del método de extensión
/// <c>services.AddMasterSlave&lt;TInput, TOutput&gt;(options => ...)</c>.
/// </summary>
/// <typeparam name="TInput">Tipo de dato de entrada.</typeparam>
/// <typeparam name="TOutput">Tipo de dato de salida.</typeparam>
public sealed class MasterSlaveOptions<TInput, TOutput>
{
    internal List<Type> SlaveTypes { get; } = new();

    /// <summary>
    /// Registra un tipo concreto de Slave para ser inyectado en el Master.
    /// Puede llamarse múltiples veces para agregar varios Slaves.
    /// </summary>
    /// <typeparam name="TSlave">Implementación concreta de <see cref="ISlave{TInput,TOutput}"/>.</typeparam>
    public MasterSlaveOptions<TInput, TOutput> AddSlave<TSlave>()
        where TSlave : class, ISlave<TInput, TOutput>
    {
        SlaveTypes.Add(typeof(TSlave));
        return this;
    }
}
