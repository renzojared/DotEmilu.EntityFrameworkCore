namespace DotEmilu.EntityFrameworkCore.Extensions;

public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Converts boolean to short integer (0/1) in a database.
    /// Recommended: Use properties starting with 'Is' (e.g., IsActive, IsDeleted, IsDisabled) 
    /// so that 1 always represents the true/positive state.
    /// </summary>
    public static PropertyBuilder<bool> HasShortConversion(this PropertyBuilder<bool> propertyBuilder)
        => propertyBuilder.HasConversion(
            convertToProviderExpression: b => b ? (short)1 : (short)0,
            convertFromProviderExpression: value => value == 1);

    /// <summary>
    /// Adds smart comment based on format placeholders:
    /// <list type="bullet">
    /// <item><description>{0} = Numeric value</description></item>
    /// <item><description>{1} = Enum name</description></item>
    /// <item><description>{2} = Description attribute (falls back to enum name)</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="builder">The PropertyBuilder</param>
    /// <param name="format">Format string. Default: "{0} = {1}"</param>
    /// <param name="includeTitle">Whether to include the enum's class description as a title</param>
    /// <returns>The PropertyBuilder for method chaining</returns>
    public static PropertyBuilder<TEnum> HasFormattedComment<TEnum>(this PropertyBuilder<TEnum> builder,
        string format = "{0} = {1}", bool includeTitle = false) where TEnum : struct, Enum
        => builder.HasComment(GetSmartEnumComments<TEnum>(format, includeTitle));

    /// <summary>
    /// Adds smart comment based on format placeholders:
    /// <list type="bullet">
    /// <item><description>{0} = Numeric value</description></item>
    /// <item><description>{1} = Enum name</description></item>
    /// <item><description>{2} = Description attribute (falls back to enum name)</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TEnum">The enum type</typeparam>
    /// <param name="builder">The PropertyBuilder</param>
    /// <param name="format">Format string. Default: "{0} = {1}"</param>
    /// <param name="includeTitle">Whether to include the enum's class description as a title</param>
    /// <returns>The PropertyBuilder for method chaining</returns>
    public static PropertyBuilder<TEnum?> HasFormattedComment<TEnum>(this PropertyBuilder<TEnum?> builder,
        string format = "{0} = {1}", bool includeTitle = false) where TEnum : struct, Enum
        => builder.HasComment(GetSmartEnumComments<TEnum>(format, includeTitle));

    private static string GetSmartEnumComments<TEnum>(string format = "{0} = {1}", bool includeTitle = false)
        where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);

        var needsDescription = format.Contains("{2}");

        var result = string.Join(", ", GetValueComments());

        return !includeTitle ? result : $"{GetTypeDescription()}: {result}";

        IEnumerable<string> GetValueComments()
            => Enum.GetValues<TEnum>().Select(enumValue =>
            {
                var numericValue = Convert.ToInt32(enumValue);
                var enumName = Enum.GetName(enumValue) ?? enumValue.ToString();

                return needsDescription
                    ? string.Format(format, numericValue, enumName, GetValueDescription(enumName))
                    : string.Format(format, numericValue, enumName);
            });

        string GetValueDescription(string enumName)
        {
            var fieldInfo = enumType.GetField(enumName);
            var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? enumName;
        }

        string GetTypeDescription()
        {
            var descriptionAttribute = enumType.GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? enumType.Name;
        }
    }
}