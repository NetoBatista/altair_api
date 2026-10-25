namespace Altair.Domain.Contract;

public class RenderResponse
{
    public Stream Content { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
