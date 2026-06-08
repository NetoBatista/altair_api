using Altair.Domain.Contract;

namespace Altair.Domain.Interface.UseCase;

public interface IRenderUseCase
{
    Task<Stream> ExecuteAsync(RenderRequest request);
}