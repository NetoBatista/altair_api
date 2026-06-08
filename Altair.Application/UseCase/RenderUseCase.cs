using Altair.Domain.Abstraction;
using Altair.Domain.Constant;
using Altair.Domain.Contract;
using Altair.Domain.Interface.UseCase;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Application.Builders;
using Gotenberg.Sharp.API.Client.Domain.Pages;

namespace Altair.Application.UseCase;

public class RenderUseCase : IRenderUseCase
{
    private readonly GotenbergSharpClient _gotenbergClient;
    public RenderUseCase(GotenbergSharpClient gotenbergClient)
    {
        _gotenbergClient = gotenbergClient;
    }

    public Task<Stream> ExecuteAsync(RenderRequest request)
    {
        if (request.Type == RenderTypeConstant.Url)
        {

            if (!IsValidUrl(request.Content))
            {
                throw new UseCaseException("Invalid URL format.");
            }

            var urlRequest = new UrlRequestBuilder()
                .SetUrl(request.Content)
                .WithPageProperties(pp =>
                {
                    pp.SetPaperSize(PaperSizes.A4)
                        .SetMargins(Margins.Default)
                        .SetScale(.90)
                        .SetLandscape();
                });

            return _gotenbergClient.UrlToPdfAsync(urlRequest);
        }

        var htmlRequest = new HtmlRequestBuilder()
                .AddDocument(doc => doc.SetBody(request.Content))
                .WithPageProperties(pp =>
                {
                    pp.SetPaperSize(PaperSizes.A4)
                        .SetMargins(Margins.Default);
                });

        return _gotenbergClient.HtmlToPdfAsync(htmlRequest);

    }

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}