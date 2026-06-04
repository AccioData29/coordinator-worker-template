namespace AccioData29.MasterSlave.Tests.Services;

/// <summary>
/// Tests unitarios de <see cref="MasterService{TInput,TOutput}"/>.
/// Validan la coordinación del Master: registro de Slaves, distribución
/// de tareas y manejo de escenarios de error.
/// </summary>
public class MasterServiceTests
{
    private readonly Mock<IWorkDistributor<int, int>> _distributorMock;
    private readonly MasterService<int, int> _master;

    public MasterServiceTests()
    {
        _distributorMock = new Mock<IWorkDistributor<int, int>>();
        _master = new MasterService<int, int>(_distributorMock.Object, Enumerable.Empty<ISlave<int, int>>());
    }

    // ── RegisterSlave ──────────────────────────────────────────────────────────

    [Fact]
    public void RegisterSlave_AgregaSlave_CuandoNoEstaRegistrado()
    {
        var slave = SlaveMockHelper.CreateAvailable<int, int>("slave-1", x => x);

        _master.RegisterSlave(slave.Object);

        _master.GetSlaves().Should().HaveCount(1);
    }

    [Fact]
    public void RegisterSlave_LanzaExcepcion_CuandoSlaveYaEstaRegistrado()
    {
        var slave = SlaveMockHelper.CreateAvailable<int, int>("slave-1", x => x);
        _master.RegisterSlave(slave.Object);

        var act = () => _master.RegisterSlave(slave.Object);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*slave-1*");
    }

    [Fact]
    public void RegisterSlave_LanzaExcepcion_CuandoSlaveEsNull()
    {
        var act = () => _master.RegisterSlave(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    // ── ExecuteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task ExecuteAsync_RetornaVacio_CuandoNoHayTareas()
    {
        var results = await _master.ExecuteAsync(Enumerable.Empty<WorkItem<int>>());

        results.Should().BeEmpty();
        _distributorMock.Verify(
            d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<ISlave<int, int>>>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_LanzaNoAvailableSlavesException_CuandoNoHaySlaves()
    {
        var workItems = new[] { new WorkItem<int>(1) };

        var act = () => _master.ExecuteAsync(workItems);

        await act.Should().ThrowAsync<NoAvailableSlavesException>();
    }

    [Fact]
    public async Task ExecuteAsync_LanzaNoAvailableSlavesException_CuandoTodosLosSlavesEstaenFallidos()
    {
        var slave = SlaveMockHelper.CreateFailed<int, int>("slave-1");
        _master.RegisterSlave(slave.Object);

        var act = () => _master.ExecuteAsync(new[] { new WorkItem<int>(1) });

        await act.Should().ThrowAsync<NoAvailableSlavesException>();
    }

    [Fact]
    public async Task ExecuteAsync_ProcesaTodasLasTareas_CuandoHaySlavesDisponibles()
    {
        var slave1 = SlaveMockHelper.CreateAvailable<int, int>("slave-1", x => x * 2);
        var slave2 = SlaveMockHelper.CreateAvailable<int, int>("slave-2", x => x * 2);
        _master.RegisterSlave(slave1.Object);
        _master.RegisterSlave(slave2.Object);

        var workItems = new[] { new WorkItem<int>(1), new WorkItem<int>(2), new WorkItem<int>(3) };

        _distributorMock
            .Setup(d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<ISlave<int, int>>>()))
            .Returns<IEnumerable<WorkItem<int>>, IReadOnlyList<ISlave<int, int>>>((items, slaves) =>
                items.Select((item, i) => (item, slaves[i % slaves.Count])));

        var results = await _master.ExecuteAsync(workItems);

        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        results.Should().AllSatisfy(r => r.Output.Should().Be(r.Output));
    }

    [Fact]
    public async Task ExecuteAsync_DistribuyeConEstrategia_LlamandoAlDistribuidor()
    {
        var slave = SlaveMockHelper.CreateAvailable<int, int>("slave-1", x => x);
        _master.RegisterSlave(slave.Object);

        var workItems = new[] { new WorkItem<int>(10) };

        _distributorMock
            .Setup(d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<ISlave<int, int>>>()))
            .Returns<IEnumerable<WorkItem<int>>, IReadOnlyList<ISlave<int, int>>>((items, slaves) =>
                items.Select(item => (item, slaves[0])));

        await _master.ExecuteAsync(workItems);

        _distributorMock.Verify(
            d => d.Distribute(It.IsAny<IEnumerable<WorkItem<int>>>(), It.IsAny<IReadOnlyList<ISlave<int, int>>>()),
            Times.Once);
    }
}
