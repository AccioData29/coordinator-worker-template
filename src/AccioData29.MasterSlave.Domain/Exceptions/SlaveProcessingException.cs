namespace AccioData29.MasterSlave.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un Slave encuentra un error irrecuperable al procesar una tarea.
/// Incluye el <see cref="SlaveId"/> para que el Master pueda identificar
/// qué nodo falló y tomar acciones de recuperación.
/// </summary>
public sealed class SlaveProcessingException : Exception
{
    /// <summary>Identificador del Slave que generó la excepción.</summary>
    public string SlaveId { get; }

    public SlaveProcessingException(string slaveId, string message)
        : base(message)
    {
        SlaveId = slaveId;
    }

    public SlaveProcessingException(string slaveId, string message, Exception inner)
        : base(message, inner)
    {
        SlaveId = slaveId;
    }
}
