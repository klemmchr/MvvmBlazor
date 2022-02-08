using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;
using Shouldly;
using Xunit;
using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters;

public class ParameterInfoTests
{
    private Mock<PropertyInfo> GenerateProperty(string propertyName)
    {
        var property = new Mock<PropertyInfo>();
        property.SetupGet(x => x.Name).Returns(propertyName);
        return property;
    }

    [Fact]
    public void IgnoresMissingPropertyOnViewModel()
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
    public void SortsProperties()
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

    [Fact]
    public void ValidatesParameters()
    {
        Should.Throw<ArgumentNullException>(() => new ParameterInfo(null, new List<PropertyInfo>()));
        Should.Throw<ArgumentNullException>(() => new ParameterInfo(new List<PropertyInfo>(), null));
    }
}