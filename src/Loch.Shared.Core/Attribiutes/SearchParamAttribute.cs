namespace Loch.Shared.Core.Attribiutes;

/// <summary>
/// Use this [attribiute] top of the each property that you want to define as Elastic search query string.
/// </summary>
public class SearchParamAttribute : Attribute
{
    public int Priority { get; }
    public SearchParamAttribute() { }
    /// <summary>
    /// Set relevant
    /// </summary>
    /// <param name="i"></param>
    public SearchParamAttribute(int i) => Priority = i;
}