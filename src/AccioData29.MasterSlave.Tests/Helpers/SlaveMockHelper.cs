namespace AccioData29.MasterSlave.Tests.Helpers;

/// <summary>
/// Factoría de mocks de <see cref="ISlave{TInput,TOutput}"/> para los tests.
/// Centraliza la configuración repetida de estado y comportamiento.
/// </summary>
public static class SlaveMockHelper
{
    /// <summary>
    /// Crea un mock de Slave con estado <see cref="WorkerStatus.Available"/>
    /// que retorna un resultado exitoso aplicando <paramref name="process"/>.
    /// </summary>
    public static Mock<ISlave<TInput, TOutput>> CreateAvailable<TInput, TOutput>(
        string slaveId,
        Func<TInput, TOutput> process)
    {
        var mock = new Mock<ISlave<TInput, TOutput>>();
        mock.Setup(s => s.SlaveId).Returns(slaveId);
        mock.Setup(s => s.Status).Returns(WorkerStatus.Available);
        mock.Setup(s => s.ProcessAsync(It.IsAny<WorkItem<TInput>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkItem<TInput> item, CancellationToken _) =>
                WorkResult<TInput, TOutput>.Success(item.Id, slaveId, process(item.Payload)));
        return mock;
    }

    /// <summary>
    /// Crea un mock de Slave con estado <see cref="WorkerStatus.Failed"/>
    /// que siempre retorna un resultado fallido.
    /// </summary>
    public static Mock<ISlave<TInput, TOutput>> CreateFailed<TInput, TOutput>(string slaveId)
    {
        var mock = new Mock<ISlave<TInput, TOutput>>();
        mock.Setup(s => s.SlaveId).Returns(slaveId);
        mock.Setup(s => s.Status).Returns(WorkerStatus.Failed);
        return mock;
    }
}
