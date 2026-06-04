namespace AccioData29.CoordinatorWorker.Domain.Exceptions;

/// <summary>
/// Se lanza cuando el Coordinator intenta distribuir tareas pero ningún Worker
/// tiene estado <see cref="Enums.WorkerStatus.Available"/>.
/// </summary>
public sealed class NoAvailableWorkersException : Exception
{
    public NoAvailableWorkersException(string message) : base(message) { }

    public NoAvailableWorkersException(string message, Exception inner) : base(message, inner) { }
}
