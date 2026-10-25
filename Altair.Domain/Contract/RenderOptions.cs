using System.ComponentModel.DataAnnotations;
using Altair.Domain.Constant;
namespace Altair.Domain.Contract;

public class RenderOptions
{
    /// <summary>
    /// The paper size to use, such as A4, Letter, or Legal.
    /// </summary>
    public string? PaperSize { get; set; }

    /// <summary>
    /// The page orientation. Valid values are "portrait" and "landscape".
    /// </summary>
    [RegularExpression($"{RenderOrientationConstant.Portrait}|{RenderOrientationConstant.Landscape}", ErrorMessage = "Orientation must be either 'portrait' or 'landscape'.")]
    public string? Orientation { get; set; }

    /// <summary>
    /// Optional page margins, in CSS units such as mm, cm, or in.
    /// </summary>
    public RenderMargins? Margins { get; set; }

    /// <summary>
    /// The scale factor to apply to the page, from 0.1 to 2.0.
    /// </summary>
    [Range(0.1, 2.0, ErrorMessage = "Scale must be between 0.1 and 2.0.")]
    public double? Scale { get; set; }

    /// <summary>
    /// Whether background graphics should be printed.
    /// </summary>
    public bool? PrintBackground { get; set; }

    /// <summary>
    /// Whether to use the page size defined by CSS.
    /// </summary>
    public bool? PreferCssPageSize { get; set; }

    /// <summary>
    /// The delay in seconds before rendering starts.
    /// </summary>
    [Range(0, 15, ErrorMessage = "Wait delay must be between 0 and 15 seconds.")]
    public int? WaitDelaySeconds { get; set; }

    /// <summary>
    /// Optional HTML content to use as the page header.
    /// </summary>
    public string? HeaderHtml { get; set; }

    /// <summary>
    /// Optional HTML content to use as the page footer.
    /// </summary>
    public string? FooterHtml { get; set; }

    /// <summary>
    /// The suggested name for the generated PDF file.
    /// </summary>
    public string? FileName { get; set; }
}