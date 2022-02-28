using System.ComponentModel;
using System.Globalization;
using BlazorSample.Domain.Entities;

namespace BlazorSample.Domain.Converters;

public class IdTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertTo(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object? value,
        Type destinationType)
    {
        return value is not string s || !Guid.TryParse(s, out var guid) ? null : new IdType(guid);
    }
}