namespace Altair.Domain.Contract;

public class RenderMargins
{
    /// <summary>
    /// The top margin, in a CSS length unit such as mm, cm, or in.
    /// </summary>
    public string? Top { get; set; }

    /// <summary>
    /// The right margin, in a CSS length unit such as mm, cm, or in.
    /// </summary>
    public string? Right { get; set; }

    /// <summary>
    /// The bottom margin, in a CSS length unit such as mm, cm, or in.
    /// </summary>
    public string? Bottom { get; set; }

    /// <summary>
    /// The left margin, in a CSS length unit such as mm, cm, or in.
    /// </summary>
    public string? Left { get; set; }
}