namespace AccioData29.CoordinatorWorker.Domain.Enums;

/// <summary>
/// Representa el estado actual de un nodo Worker dentro del sistema Coordinator-Worker.
/// El Coordinator consulta este estado antes de asignar tareas para garantizar que
/// solo los workers disponibles reciban trabajo.
/// </summary>
public enum WorkerStatus
{
    /// <summary>El worker está listo para recibir y procesar tareas.</summary>
    Available = 1,

    /// <summary>El worker está procesando una tarea y no puede recibir más trabajo.</summary>
    Busy = 2,

    /// <summary>El worker encontró un error y no debe recibir tareas hasta ser recuperado.</summary>
    Failed = 3,

    /// <summary>El worker está desconectado o no disponible en el sistema.</summary>
    Offline = 4
}
