using Altair.Domain.Contract;
using Altair.Domain.Interface.UseCase;
using Microsoft.AspNetCore.Mvc;

namespace Altair.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RenderController : ControllerBase
{
    private readonly IRenderUseCase _renderUseCase;
    public RenderController(IRenderUseCase renderUseCase)
    {
        _renderUseCase = renderUseCase;
    }

    [HttpPost]
    public async Task<ActionResult<Stream>> Render([FromBody] RenderRequest request)
    {
        var result = await _renderUseCase.ExecuteAsync(request);
        return File(result, "application/pdf");
    }
}
