using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace TestProject1;

public class Tests
{
    
    [SetUp]
    public void Setup()
    {
        var result = Program.AddNumbers([1, 2, 3]);
        Assert.That(result, Is.EqualTo(6));
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}