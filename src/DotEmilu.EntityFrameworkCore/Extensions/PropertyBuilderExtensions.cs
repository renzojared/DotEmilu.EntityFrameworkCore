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
}