# AccioData29.CoordinatorWorker

Implementación genérica del patrón **Coordinator-Worker** para .NET 8, con distribución Round-Robin, soporte para `CancellationToken` e integración con el sistema de DI de .NET.

## Estructura del proyecto

```
src/
├── AccioData29.CoordinatorWorker.Domain/
│   ├── Entities/
│   │   ├── WorkItem<T>          — unidad de trabajo inmutable
│   │   └── WorkResult<TIn,TOut> — resultado exitoso o fallido (patrón Result)
│   ├── Enums/
│   │   └── WorkerStatus         — Available | Busy | Failed | Offline
│   └── Exceptions/
│       ├── NoAvailableWorkersException
│       └── WorkerProcessingException
│
├── AccioData29.CoordinatorWorker.Application/
│   ├── Interfaces/
│   │   ├── ICoordinator<TIn,TOut>     — punto de entrada del patrón
│   │   ├── IWorker<TIn,TOut>          — nodo trabajador
│   │   └── IWorkDistributor<TIn,TOut> — estrategia de distribución
│   ├── Services/
│   │   └── CoordinatorService         — implementación del Coordinator
│   ├── Strategies/
│   │   └── RoundRobinDistributor      — distribución cíclica equitativa
│   ├── Options/
│   │   └── CoordinatorWorkerOptions   — configuración del contenedor DI
│   └── Extensions/
│       └── ServiceCollectionExtensions — AddCoordinatorWorker<TIn,TOut>()
│
└── AccioData29.CoordinatorWorker.Tests/
    ├── Services/
    │   └── CoordinatorServiceTests
    ├── Strategies/
    │   └── RoundRobinDistributorTests
    └── Helpers/
        └── WorkerMockHelper
```

## Cómo funciona

El patrón divide responsabilidades en dos roles:

- **Coordinator** (`CoordinatorService`): recibe todas las tareas, consulta qué Workers están disponibles, delega la distribución a la estrategia configurada y ejecuta el procesamiento en paralelo con `Task.WhenAll`.
- **Worker** (`IWorker<TIn, TOut>`): procesa una sola unidad de trabajo (`WorkItem`) y retorna un `WorkResult`. No sabe nada del Coordinator ni de los otros Workers.

## Uso con inyección de dependencias

```csharp
// Program.cs / Startup.cs
services.AddCoordinatorWorker<string, int>(options =>
{
    options.AddWorker<MiPrimerWorker>();
    options.AddWorker<MiSegundoWorker>();
});
```

```csharp
// Implementar un Worker
public class MiPrimerWorker : IWorker<string, int>
{
    public string WorkerId => "worker-1";
    public WorkerStatus Status => WorkerStatus.Available;

    public Task<WorkResult<string, int>> ProcessAsync(
        WorkItem<string> workItem,
        CancellationToken cancellationToken = default)
    {
        var result = workItem.Payload.Length;
        return Task.FromResult(
            WorkResult<string, int>.Success(workItem.Id, WorkerId, result));
    }
}
```

```csharp
// Ejecutar tareas
public class MiServicio
{
    private readonly ICoordinator<string, int> _coordinator;

    public MiServicio(ICoordinator<string, int> coordinator)
        => _coordinator = coordinator;

    public async Task EjecutarAsync()
    {
        var tareas = new[]
        {
            new WorkItem<string>("hola"),
            new WorkItem<string>("mundo"),
        };

        var resultados = await _coordinator.ExecuteAsync(tareas);

        foreach (var r in resultados)
            Console.WriteLine(r.IsSuccess ? $"OK: {r.Output}" : $"Error: {r.ErrorMessage}");
    }
}
```

## Uso sin inyección de dependencias

```csharp
var distributor = new RoundRobinDistributor<string, int>();
var coordinator = new CoordinatorService<string, int>(distributor, []);

coordinator.RegisterWorker(new MiPrimerWorker());
coordinator.RegisterWorker(new MiSegundoWorker());

var resultados = await coordinator.ExecuteAsync(tareas);
```

## Estrategia de distribución

La estrategia por defecto es **Round-Robin**: las tareas se asignan a los Workers de forma cíclica.

| Tarea | Worker asignado |
|-------|----------------|
| 0     | Worker 0       |
| 1     | Worker 1       |
| 2     | Worker 0       |
| 3     | Worker 1       |

Para agregar una estrategia personalizada, implementar `IWorkDistributor<TIn, TOut>` y registrarla en el contenedor DI antes de llamar a `AddCoordinatorWorker`.

## Manejo de errores

- Si no hay Workers con estado `Available`, se lanza `NoAvailableWorkersException`.
- Cada Worker debe capturar sus propias excepciones y retornar `WorkResult.Failure(...)` para no interrumpir el procesamiento paralelo de las demás tareas.
- El Coordinator agrega todos los resultados (exitosos y fallidos) y los retorna al llamador.

## Ejecutar los tests

```bash
dotnet test
```

## Requisitos

- .NET 8
- `Microsoft.Extensions.DependencyInjection.Abstractions` 8.0.2
