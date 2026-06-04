namespace AccioData29.CoordinatorWorker.Domain.Entities;

/// <summary>
/// Representa una unidad de trabajo que el Coordinator distribuye a los Workers.
/// Es inmutable por diseño: una vez creado, el payload no cambia.
/// </summary>
/// <typeparam name="T">Tipo del dato que contiene la tarea a procesar.</typeparam>
public sealed class WorkItem<T>
{
    /// <summary>Identificador único de esta unidad de trabajo.</summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>Dato a procesar por el Worker asignado.</summary>
    public T Payload { get; }

    /// <summary>Momento en que fue creada la tarea (UTC).</summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <param name="payload">Dato que el Worker debe procesar. No puede ser null.</param>
    /// <exception cref="ArgumentNullException">Si payload es null.</exception>
    public WorkItem(T payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        Payload = payload;
    }
}
