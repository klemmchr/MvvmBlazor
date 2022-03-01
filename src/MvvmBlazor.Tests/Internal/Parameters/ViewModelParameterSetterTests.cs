using System.Globalization;
using ParameterInfo = MvvmBlazor.Internal.Parameters.ParameterInfo;

namespace MvvmBlazor.Tests.Internal.Parameters;

public class ViewModelParameterSetterTests : UnitTest
{
    private static readonly StronglyTypedParameter TestParameter = new();

    public ViewModelParameterSetterTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

    protected override void RegisterServices(IServiceCollection services)
    {
        services.StrictMock<IParameterResolver>();
        services.StrictMock<ComponentBase>();
        services.StrictMock<ViewModelBase>();
        services.Provide<ViewModelParameterSetter>();
    }

    private static Mock<PropertyInfo> GenerateProperty(string propertyName, Type propertyType)
    {
        var property = new Mock<PropertyInfo>();
        property.Setup(x => x.Name).Returns(propertyName).Verifiable();
        property.SetupGet(x => x.PropertyType).Returns(propertyType).Verifiable();
        return property;
    }

    [Fact]
    public void ResolveAndSet_resolves_parameters_of_same_type()
    {
        const string propName = "p1";
        const string componentValue = "foo";
        var cp1 = GenerateProperty(propName, componentValue.GetType());
        var componentProperties = new List<PropertyInfo> { cp1.Object };

        var vmp1 = GenerateProperty(propName, componentValue.GetType());
        var viewModelProperties = new List<PropertyInfo> { vmp1.Object };
        var parameterInfo = new ParameterInfo(componentProperties, viewModelProperties);

        var resolver = Services.GetMock<IParameterResolver>();
        var component = Services.GetMock<ComponentBase>();
        var viewModel = Services.GetMock<ViewModelBase>();
        resolver.Setup(x => x.ResolveParameters(component.Object.GetType(), viewModel.Object.GetType()))
            .Returns(parameterInfo)
            .Verifiable();
        cp1.Setup(x => x.GetValue(component.Object, null)).Returns(componentValue).Verifiable();
        vmp1.Setup(x => x.SetValue(viewModel.Object, componentValue, null)).Verifiable();

        var setter = Services.GetRequiredService<ViewModelParameterSetter>();

        setter.ResolveAndSet(component.Object, viewModel.Object);

        cp1.Verify();
        vmp1.Verify();
        resolver.Verify();
    }

    [Fact]
    public void ResolveAndSet_resolves_parameters_of_convertible_types()
    {
        const string propName = "p1";
        const int componentValue = 42;
        var cp1 = GenerateProperty(propName, componentValue.GetType());
        var componentProperties = new List<PropertyInfo> { cp1.Object };

        var vmp1 = GenerateProperty(propName, typeof(StronglyTypedParameter));
        var viewModelProperties = new List<PropertyInfo> { vmp1.Object };
        var parameterInfo = new ParameterInfo(componentProperties, viewModelProperties);

        var resolver = Services.GetMock<IParameterResolver>();
        var component = Services.GetMock<ComponentBase>();
        var viewModel = Services.GetMock<ViewModelBase>();
        resolver.Setup(x => x.ResolveParameters(component.Object.GetType(), viewModel.Object.GetType()))
            .Returns(parameterInfo)
            .Verifiable();
        cp1.Setup(x => x.GetValue(component.Object, null)).Returns(componentValue).Verifiable();
        vmp1.Setup(x => x.SetValue(viewModel.Object, TestParameter, null)).Verifiable();

        var setter = Services.GetRequiredService<ViewModelParameterSetter>();

        setter.ResolveAndSet(component.Object, viewModel.Object);

        cp1.Verify();
        vmp1.Verify();
        resolver.Verify();
    }

    [TypeConverter(typeof(StronglyTypedParameterConverter))]
    private class StronglyTypedParameter { }

    private class StronglyTypedParameterConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(int);
        }

        public override object ConvertTo(
            ITypeDescriptorContext? context,
            CultureInfo? culture,
            object? value,
            Type destinationType)
        {
            return TestParameter;
        }
    }
}