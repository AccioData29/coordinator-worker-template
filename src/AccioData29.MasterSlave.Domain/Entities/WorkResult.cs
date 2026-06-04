namespace AccioData29.CoordinatorWorker.Domain.Entities;

/// <summary>
/// Representa el resultado de procesar un <see cref="WorkItem{TInput}"/> por un Worker.
/// Usa el patrón Result para evitar excepciones en el flujo normal: el Coordinator
/// inspecciona <see cref="IsSuccess"/> para decidir qué hacer con cada resultado.
/// </summary>
/// <typeparam name="TInput">Tipo del dato de entrada del WorkItem original.</typeparam>
/// <typeparam name="TOutput">Tipo del dato producido por el Worker.</typeparam>
public sealed class WorkResult<TInput, TOutput>
{
    /// <summary>Id del <see cref="WorkItem{TInput}"/> que originó este resultado.</summary>
    public Guid WorkItemId { get; private init; }

    /// <summary>Id del Worker que procesó la tarea.</summary>
    public string WorkerId { get; private init; } = string.Empty;

    /// <summary>Resultado producido por el Worker. Null si <see cref="IsSuccess"/> es false.</summary>
    public TOutput? Output { get; private init; }

    /// <summary>Indica si el procesamiento fue exitoso.</summary>
    public bool IsSuccess { get; private init; }

    /// <summary>Mensaje de error en caso de fallo. Null si <see cref="IsSuccess"/> es true.</summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>Momento en que el Worker terminó el procesamiento (UTC).</summary>
    public DateTime ProcessedAt { get; private init; }

    private WorkResult() { }

    /// <summary>
    /// Crea un resultado exitoso.
    /// </summary>
    public static WorkResult<TInput, TOutput> Success(Guid workItemId, string workerId, TOutput output) =>
        new()
        {
            WorkItemId = workItemId,
            WorkerId = workerId,
            Output = output,
            IsSuccess = true,
            ProcessedAt = DateTime.UtcNow
        };

    /// <summary>
    /// Crea un resultado fallido con el mensaje de error correspondiente.
    /// </summary>
    public static WorkResult<TInput, TOutput> Failure(Guid workItemId, string workerId, string errorMessage) =>
        new()
        {
            WorkItemId = workItemId,
            WorkerId = workerId,
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ProcessedAt = DateTime.UtcNow
        };
}
