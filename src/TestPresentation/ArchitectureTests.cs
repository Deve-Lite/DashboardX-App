
namespace TestPresentation;

public class ArchitectureTests
{
    private const string Common = "Common";
    private const string Core = "Core";
    private const string Presentation = "Presentation";

    [Fact]
    public void CommonIsNotDependentOnPresentation()
    {
        //var assembly = typeof(Core.AssemblyReference).Assembly;

        //var dependencies = new()
        //{
        //    Presentation,
        //};
        Assert.True(false);
    }

    [Fact]
    public void CommonIsNotDependentOnCore()
    {
        Assert.True(false);

    }

    [Fact]
    public void CoreIsNotDependentOnPresentation()
    {

        Assert.True(false);
    }

}
