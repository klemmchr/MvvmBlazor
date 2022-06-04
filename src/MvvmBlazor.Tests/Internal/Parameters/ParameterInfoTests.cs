using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters;

public class ParameterInfoTests
{
    private static Mock<PropertyInfo> GenerateProperty(string propertyName)
    {
        var property = new StrictMock<PropertyInfo>();
        property.SetupGet(x => x.Name).Returns(propertyName);
        property.SetupGet(x => x.DeclaringType).Returns(typeof(ParameterInfoTests));
        property.Setup(x => x.GetHashCode()).Returns(propertyName.GetHashCode(StringComparison.OrdinalIgnoreCase));
        property.Setup(x => x.Equals(It.IsAny<PropertyInfo>())).Returns<object?>((obj) => obj is PropertyInfo p && p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        return property;
    }

    [Fact]
    public void Ignores_missing_property_on_viewmodel()
    {
        var p1 = GenerateProperty("p1");
        var p2 = GenerateProperty("p2");
        var componentProperties = new List<PropertyInfo> { p1.Object, p2.Object };

        var vmp1 = GenerateProperty("p1");
        var viewModelProperties = new List<PropertyInfo> { vmp1.Object };

        var info = new ParameterInfo(componentProperties, viewModelProperties);
        info.Parameters.ShouldNotBeNull();
        info.Parameters.Count.ShouldBe(1);
        info.Parameters.ElementAt(0).Key.ShouldBe(p1.Object);
        info.Parameters.ElementAt(0).Value.ShouldBe(vmp1.Object);
    }

    [Fact]
    public void Throws_for_missing_property_on_component()
    {
        var p1 = GenerateProperty("p1");
        var p2 = GenerateProperty("p2");
        var componentProperties = new List<PropertyInfo> { p2.Object };

        var vmp1 = GenerateProperty("p1");
        var viewModelProperties = new List<PropertyInfo> { vmp1.Object };

        Should.Throw<ParameterException>(() => new ParameterInfo(componentProperties, viewModelProperties));
    }

    [Fact]
    public void Sorts_properties()
    {
        var p1 = GenerateProperty("p1");
        var p2 = GenerateProperty("p2");
        var componentProperties = new List<PropertyInfo> { p1.Object, p2.Object };

        var vmp1 = GenerateProperty("p1");
        var vmp2 = GenerateProperty("p2");
        var viewModelProperties = new List<PropertyInfo> { vmp2.Object, vmp1.Object };

        var info = new ParameterInfo(componentProperties, viewModelProperties);
        info.Parameters.ShouldNotBeNull();
        info.Parameters.Count.ShouldBe(2);
        info.Parameters.ElementAt(0).Key.ShouldBe(p1.Object);
        info.Parameters.ElementAt(0).Value.ShouldBe(vmp1.Object);
        info.Parameters.ElementAt(1).Key.ShouldBe(p2.Object);
        info.Parameters.ElementAt(1).Value.ShouldBe(vmp2.Object);
    }
}