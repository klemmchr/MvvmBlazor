using System.ComponentModel;
using System.Text.Json;
using BlazorSample.Domain.Converters;

namespace BlazorSample.Domain.Entities;

[TypeConverter(typeof(IdTypeConverter))]
public record IdType(Guid Value)
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}