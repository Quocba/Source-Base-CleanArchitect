using Microsoft.Extensions.FileProviders;

namespace BaseAPI.DI;

public static class FileExtension
{
    public static IApplicationBuilder UseSharedStaticFiles(this IApplicationBuilder app, IConfiguration configuration)
    {
        var staticFilesSection = configuration.GetSection("StaticFiles");
        var sharedRoot = staticFilesSection["SharedRoot"];
        var projectFolder = staticFilesSection["ProjectFolder"];

        if (string.IsNullOrEmpty(sharedRoot) || string.IsNullOrEmpty(projectFolder))
            return app;

        var projectSharedPath = Path.Combine(sharedRoot, projectFolder);

        if (Directory.Exists(projectSharedPath))
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(projectSharedPath),
                RequestPath = ""
            });
        }

        return app;
    }
}
