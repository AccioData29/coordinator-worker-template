namespace AccioData29.CoordinatorWorker.Tests.Services;

/// <summary>
/// Tests unitarios de <see cref="CoordinatorService{TInput,TOutput}"/>.
/// Validan la coordinación del Coordinator: registro de Workers, distribución
/// de tareas y manejo de escenarios de error.
/// </summary>
public class CoordinatorServiceTests
{
    private readonly Mock<IWorkDistributor<int, int>> _distributorMock;
    private readonly CoordinatorService<int, int> _coordinator;

    public CoordinatorServiceTests()
    {
        _distributorMock = new Mock<IWorkDistributor<int, int>>();
        _coordinator = new CoordinatorService<int, int>(_distributorMock.Object, Enumerable.Empty<IWorker<int, int>>());
    }

    // ── RegisterWorker ──────────────────────────────────────────────────────────

    [Fact]
    public void RegisterWorker_AgregaWorker_CuandoNoEstaRegistrado()
    {
        var worker = WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x);

        _coordinator.RegisterWorker(worker.Object);

        _coordinator.GetWorkers().Should().HaveCount(1);
    }

    [Fact]
    public void RegisterWorker_LanzaExcepcion_CuandoWorkerYaEstaRegistrado()
    {
        var worker = WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x);
        _coordinator.RegisterWorker(worker.Object);

        var act = () => _coordinator.RegisterWorker(worker.Object);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*worker-1*");
    }

    [Fact]
    public void RegisterWorker_LanzaExcepcion_CuandoWorkerEsNull()
    {
        var act = () => _coordinator.RegisterWorker(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    // ── ExecuteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task ExecuteAsync_RetornaVacio_CuandoNoHayTareas()
    {
        var results = await _coordinator.ExecuteAsync(Enumerable.Empty<WorkItem<int>>());

        results.Should().BeEmpty();
        _distributorMock.Verify(
            d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<IWorker<int, int>>>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_LanzaNoAvailableWorkersException_CuandoNoHayWorkers()
    {
        var workItems = new[] { new WorkItem<int>(1) };

        var act = () => _coordinator.ExecuteAsync(workItems);

        await act.Should().ThrowAsync<NoAvailableWorkersException>();
    }

    [Fact]
    public async Task ExecuteAsync_LanzaNoAvailableWorkersException_CuandoTodosLosWorkersEstaenFallidos()
    {
        var worker = WorkerMockHelper.CreateFailed<int, int>("worker-1");
        _coordinator.RegisterWorker(worker.Object);

        var act = () => _coordinator.ExecuteAsync(new[] { new WorkItem<int>(1) });

        await act.Should().ThrowAsync<NoAvailableWorkersException>();
    }

    [Fact]
    public async Task ExecuteAsync_ProcesaTodasLasTareas_CuandoHayWorkersDisponibles()
    {
        var worker1 = WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x * 2);
        var worker2 = WorkerMockHelper.CreateAvailable<int, int>("worker-2", x => x * 2);
        _coordinator.RegisterWorker(worker1.Object);
        _coordinator.RegisterWorker(worker2.Object);

        var workItems = new[] { new WorkItem<int>(1), new WorkItem<int>(2), new WorkItem<int>(3) };

        _distributorMock
            .Setup(d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<IWorker<int, int>>>()))
            .Returns<IEnumerable<WorkItem<int>>, IReadOnlyList<IWorker<int, int>>>((items, workers) =>
                items.Select((item, i) => (item, workers[i % workers.Count])));

        var results = await _coordinator.ExecuteAsync(workItems);

        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        results.Should().AllSatisfy(r => r.Output.Should().Be(r.Output));
    }

    [Fact]
    public async Task ExecuteAsync_DistribuyeConEstrategia_LlamandoAlDistribuidor()
    {
        var worker = WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x);
        _coordinator.RegisterWorker(worker.Object);

        var workItems = new[] { new WorkItem<int>(10) };

        _distributorMock
            .Setup(d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<IWorker<int, int>>>()))
            .Returns<IEnumerable<WorkItem<int>>, IReadOnlyList<IWorker<int, int>>>((items, workers) =>
                items.Select(item => (item, workers[0])));

        await _coordinator.ExecuteAsync(workItems);

        _distributorMock.Verify(
            d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<IWorker<int, int>>>()),
            Times.Once);
    }
}
