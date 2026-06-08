using Altair.Application.UseCase;
using Altair.Domain.Interface.UseCase;
using Microsoft.Extensions.DependencyInjection;

namespace Altair.Application;

public static class DependencyInjection
{
    public static void InjectApplication(this IServiceCollection services)
    {
        services.AddScoped<IRenderUseCase, RenderUseCase>();
    }
}
