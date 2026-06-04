namespace AccioData29.CoordinatorWorker.Tests.Helpers;

/// <summary>
/// Factoría de mocks de <see cref="IWorker{TInput,TOutput}"/> para los tests.
/// Centraliza la configuración repetida de estado y comportamiento.
/// </summary>
public static class WorkerMockHelper
{
    /// <summary>
    /// Crea un mock de Worker con estado <see cref="WorkerStatus.Available"/>
    /// que retorna un resultado exitoso aplicando <paramref name="process"/>.
    /// </summary>
    public static Mock<IWorker<TInput, TOutput>> CreateAvailable<TInput, TOutput>(
        string workerId,
        Func<TInput, TOutput> process)
    {
        var mock = new Mock<IWorker<TInput, TOutput>>();
        mock.Setup(w => w.WorkerId).Returns(workerId);
        mock.Setup(w => w.Status).Returns(WorkerStatus.Available);
        mock.Setup(w => w.ProcessAsync(It.IsAny<WorkItem<TInput>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkItem<TInput> item, CancellationToken _) =>
                WorkResult<TInput, TOutput>.Success(item.Id, workerId, process(item.Payload)));
        return mock;
    }

    /// <summary>
    /// Crea un mock de Worker con estado <see cref="WorkerStatus.Failed"/>
    /// que siempre retorna un resultado fallido.
    /// </summary>
    public static Mock<IWorker<TInput, TOutput>> CreateFailed<TInput, TOutput>(string workerId)
    {
        var mock = new Mock<IWorker<TInput, TOutput>>();
        mock.Setup(w => w.WorkerId).Returns(workerId);
        mock.Setup(w => w.Status).Returns(WorkerStatus.Failed);
        return mock;
    }
}
