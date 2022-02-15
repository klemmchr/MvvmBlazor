namespace MvvmBlazor.CodeGenerators.Extensions;

internal static class StringBuilderExtensions
{
    public static void AppendLineFormat(this StringBuilder stringBuilder, string format, object arg0)
    {
        stringBuilder.AppendFormat(format, arg0);
        stringBuilder.AppendLine();
    }

    public static void AppendLineFormat(this StringBuilder stringBuilder, string format, object arg0, object arg1)
    {
        stringBuilder.AppendFormat(format, arg0, arg1);
        stringBuilder.AppendLine();
    }
}