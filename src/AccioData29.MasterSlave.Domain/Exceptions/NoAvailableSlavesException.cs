namespace AccioData29.MasterSlave.Domain.Exceptions;

/// <summary>
/// Se lanza cuando el Master intenta distribuir tareas pero ningún Slave
/// tiene estado <see cref="Enums.WorkerStatus.Available"/>.
/// </summary>
public sealed class NoAvailableSlavesException : Exception
{
    public NoAvailableSlavesException(string message) : base(message) { }

    public NoAvailableSlavesException(string message, Exception inner) : base(message, inner) { }
}
