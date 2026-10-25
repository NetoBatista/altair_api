using Altair.Domain.Abstraction;
using Altair.Domain.Constant;
using Altair.Domain.Contract;
using Altair.Domain.Interface.UseCase;
using Gotenberg.Sharp.API.Client;
using Gotenberg.Sharp.API.Client.Application.Builders;
using Gotenberg.Sharp.API.Client.Application.Pages;
using Gotenberg.Sharp.API.Client.Domain.Pages;

namespace Altair.Application.UseCase;

public class RenderUseCase : IRenderUseCase
{
    private readonly GotenbergSharpClient _gotenbergClient;
    public RenderUseCase(GotenbergSharpClient gotenbergClient)
    {
        _gotenbergClient = gotenbergClient;
    }

    public async Task<RenderResponse> ExecuteAsync(RenderRequest request)
    {
        var result = new RenderResponse
        {
            FileName = GetPdfFileName(request.Options?.FileName)
        };

        var options = request.Options;

        if (request.Type == RenderTypeConstant.Url)
        {

            if (!IsValidUrl(request.Content))
            {
                throw new UseCaseException("Invalid URL format.");
            }

            var urlRequest = new UrlRequestBuilder()
                .SetUrl(request.Content)
                .WithPageProperties(pp => ConfigurePageProperties(pp, options, true, .90));

            if (options?.HeaderHtml is not null || options?.FooterHtml is not null)
            {
                urlRequest.AddHeaderFooter(headerFooter =>
                {
                    if (options.HeaderHtml is not null)
                    {
                        headerFooter.SetHeader(options.HeaderHtml);
                    }

                    if (options.FooterHtml is not null)
                    {
                        headerFooter.SetFooter(options.FooterHtml);
                    }
                });
            }

            if (options?.WaitDelaySeconds is int waitDelaySeconds)
            {
                urlRequest.SetConversionBehaviors(behavior => behavior.SetBrowserWaitDelay(waitDelaySeconds));
            }

            if (result.FileName is not null)
            {
                urlRequest.ConfigureRequest(config => config.SetResultFileName(result.FileName));
            }

            result.Content = await _gotenbergClient.UrlToPdfAsync(urlRequest);
            return result;
        }

        var htmlRequest = new HtmlRequestBuilder()
            .AddDocument(doc =>
            {
                doc.SetBody(request.Content);

                if (options?.HeaderHtml is not null)
                {
                    doc.SetHeader(options.HeaderHtml);
                }

                if (options?.FooterHtml is not null)
                {
                    doc.SetFooter(options.FooterHtml);
                }
            })
            .WithPageProperties(pp => ConfigurePageProperties(pp, options, false, null));

        if (options?.WaitDelaySeconds is int htmlWaitDelaySeconds)
        {
            htmlRequest.SetConversionBehaviors(behavior => behavior.SetBrowserWaitDelay(htmlWaitDelaySeconds));
        }

        if (result.FileName is not null)
        {
            htmlRequest.ConfigureRequest(config => config.SetResultFileName(result.FileName));
        }

        result.Content = await _gotenbergClient.HtmlToPdfAsync(htmlRequest);
        return result;

    }

    private static void ConfigurePageProperties(
        PagePropertyBuilder pageProperties,
        RenderOptions? options,
        bool defaultLandscape,
        double? defaultScale)
    {
        pageProperties.SetPaperSize(GetPaperSize(options?.PaperSize));

        if (options?.Orientation is not null)
        {
            pageProperties.SetLandscape(string.Equals(options.Orientation, "landscape", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            pageProperties.SetLandscape(defaultLandscape);
        }

        if (options?.Margins is null)
        {
            pageProperties.SetMargins(Margins.Default);
        }
        else
        {
            pageProperties.SetMargins(GetMargins(options.Margins));
        }

        if (options?.Scale is double scale)
        {
            pageProperties.SetScale(scale);
        }
        else if (defaultScale is double scaleDefault)
        {
            pageProperties.SetScale(scaleDefault);
        }

        if (options?.PrintBackground is bool printBackground)
        {
            pageProperties.SetPrintBackground(printBackground);
        }

        if (options?.PreferCssPageSize is bool preferCssPageSize)
        {
            pageProperties.SetPreferCssPageSize(preferCssPageSize);
        }
    }

    private static PaperSizes GetPaperSize(string? paperSize)
    {
        return paperSize?.ToUpperInvariant() switch
        {
            null => PaperSizes.A4,
            "A3" => PaperSizes.A3,
            "A4" => PaperSizes.A4,
            "A5" => PaperSizes.A5,
            "A6" => PaperSizes.A6,
            "LETTER" => PaperSizes.Letter,
            "LEGAL" => PaperSizes.Legal,
            "TABLOID" => PaperSizes.Tabloid,
            "D" => PaperSizes.D,
            "E" => PaperSizes.E,
            _ => throw new UseCaseException("Unsupported paper size.")
        };
    }

    private static string GetMargins(RenderMargins margins)
    {
        const string defaultMargin = "0.39in";
        return string.Join(" ",
            margins.Top ?? defaultMargin,
            margins.Right ?? defaultMargin,
            margins.Bottom ?? defaultMargin,
            margins.Left ?? defaultMargin);
    }

    private static string GetPdfFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            return $"{fileName}.pdf";
        }

        fileName = Path.GetFileName(fileName.Trim().Replace('\\', '/'));
        fileName = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new UseCaseException("Invalid PDF file name.");
        }

        return $"{fileName}.pdf";
    }

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}