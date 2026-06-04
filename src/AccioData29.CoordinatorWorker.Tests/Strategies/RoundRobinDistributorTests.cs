namespace AccioData29.CoordinatorWorker.Tests.Strategies;

/// <summary>
/// Tests unitarios de <see cref="RoundRobinDistributor{TInput,TOutput}"/>.
/// Validan que la distribución cíclica sea correcta en distintos escenarios.
/// </summary>
public class RoundRobinDistributorTests
{
    private readonly RoundRobinDistributor<int, int> _distributor = new();

    [Fact]
    public void Distribute_AsignaTareasEnOrdenCiclico()
    {
        var workers = new[]
        {
            WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x).Object,
            WorkerMockHelper.CreateAvailable<int, int>("worker-2", x => x).Object
        };
        var items = new[] { new WorkItem<int>(1), new WorkItem<int>(2), new WorkItem<int>(3) };

        var assignments = _distributor.Distribute(items, workers).ToList();

        assignments[0].Worker.WorkerId.Should().Be("worker-1");
        assignments[1].Worker.WorkerId.Should().Be("worker-2");
        assignments[2].Worker.WorkerId.Should().Be("worker-1"); // vuelve al inicio
    }

    [Fact]
    public void Distribute_AsignaTodoAlMismoWorker_CuandoHayUnSoloWorker()
    {
        var workers = new[]
        {
            WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x).Object
        };
        var items = new[] { new WorkItem<int>(1), new WorkItem<int>(2), new WorkItem<int>(3) };

        var assignments = _distributor.Distribute(items, workers).ToList();

        assignments.Should().HaveCount(3);
        assignments.Should().AllSatisfy(a => a.Worker.WorkerId.Should().Be("worker-1"));
    }

    [Fact]
    public void Distribute_RetornaVacio_CuandoNoHayTareas()
    {
        var workers = new[]
        {
            WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x).Object
        };

        var assignments = _distributor.Distribute(Enumerable.Empty<WorkItem<int>>(), workers);

        assignments.Should().BeEmpty();
    }

    [Fact]
    public void Distribute_LanzaExcepcion_CuandoWorkItemsEsNull()
    {
        var workers = new[]
        {
            WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x).Object
        };

        var act = () => _distributor.Distribute(null!, workers).ToList();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Distribute_MantieneReferenciasOriginales_DeWorkItems()
    {
        var workers = new[]
        {
            WorkerMockHelper.CreateAvailable<int, int>("worker-1", x => x).Object
        };
        var original = new WorkItem<int>(42);

        var assignment = _distributor.Distribute(new[] { original }, workers).Single();

        assignment.WorkItem.Should().BeSameAs(original);
    }
}
