using System.ComponentModel.DataAnnotations;
using Altair.Domain.Constant;

namespace Altair.Domain.Contract;

public class RenderRequest
{
    /// <summary>
    /// The content to be rendered. This can be either raw HTML or a URL, depending on the value of the Type property.
    /// </summary>
    [Required]
    public string Content { get; set; } = null!;
    /// <summary>
    /// Specifies the type of content being provided. Valid values are "html" for raw HTML content and "url" for a web page URL. This property is required and must be one of the specified values.
    /// </summary>
    [Required]
    [RegularExpression($"^({RenderTypeConstant.Html}|{RenderTypeConstant.Url})$", ErrorMessage = "Type must be either 'html' or 'url'.")]
    public string Type { get; set; } = null!;
}