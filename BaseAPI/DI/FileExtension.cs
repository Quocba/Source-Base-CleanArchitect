using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace BaseAPI.DI;

public static class FileExtension
{
    public static IApplicationBuilder UseSharedStaticFiles(this IApplicationBuilder app, IConfiguration configuration)
    {
        var projectSharedPath = configuration["StorageSettings:RootPath"];

        if (string.IsNullOrEmpty(projectSharedPath))
        {
            var staticFilesSection = configuration.GetSection("StaticFiles");
            var sharedRoot = staticFilesSection["SharedRoot"];
            var projectFolder = staticFilesSection["ProjectFolder"];

            if (string.IsNullOrEmpty(sharedRoot) || string.IsNullOrEmpty(projectFolder))
                return app;

            projectSharedPath = Path.Combine(sharedRoot, projectFolder);
        }

        if (!Directory.Exists(projectSharedPath))
        {
            try
            {
                Directory.CreateDirectory(projectSharedPath);
            }
            catch (Exception)
            {
                projectSharedPath = Path.Combine(Directory.GetCurrentDirectory(), "DCM-Storage");

                if (!Directory.Exists(projectSharedPath))
                {
                    Directory.CreateDirectory(projectSharedPath);
                }

                configuration["StorageSettings:RootPath"] = projectSharedPath;
            }
        }

        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".sql"] = "application/sql";

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(projectSharedPath),
            RequestPath = "",
            ContentTypeProvider = provider,
            ServeUnknownFileTypes = true,
            DefaultContentType = "application/octet-stream"
        });

        return app;
    }
}
