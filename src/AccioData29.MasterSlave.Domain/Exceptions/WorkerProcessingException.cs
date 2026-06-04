namespace AccioData29.CoordinatorWorker.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un Worker encuentra un error irrecuperable al procesar una tarea.
/// Incluye el <see cref="WorkerId"/> para que el Coordinator pueda identificar
/// qué nodo falló y tomar acciones de recuperación.
/// </summary>
public sealed class WorkerProcessingException : Exception
{
    /// <summary>Identificador del Worker que generó la excepción.</summary>
    public string WorkerId { get; }

    public WorkerProcessingException(string workerId, string message)
        : base(message)
    {
        WorkerId = workerId;
    }

    public WorkerProcessingException(string workerId, string message, Exception inner)
        : base(message, inner)
    {
        WorkerId = workerId;
    }
}
