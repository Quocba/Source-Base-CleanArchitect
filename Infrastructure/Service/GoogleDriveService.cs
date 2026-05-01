using Application.IService;
using Application.Payload.Request.Uploads;
using Domain;
using Domain.Payload.Base;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly DriveService _service;
    private readonly string _folderId;
    private readonly ILogger<GoogleDriveService> _logger;

    public GoogleDriveService(IConfiguration config, ILogger<GoogleDriveService> logger)
    {
        _logger = logger;

        var settings = config.GetSection("GoogleDrive");
        _folderId = settings["FolderId"];
        var serviceAccountFile = settings["ServiceAccountKeyFile"];

        if (string.IsNullOrEmpty(_folderId))
            throw new Exception("GoogleDrive FolderId is missing in configuration");

        if (string.IsNullOrEmpty(serviceAccountFile) || !File.Exists(serviceAccountFile))
            throw new Exception("ServiceAccountKeyFile path is invalid");

        GoogleCredential credential;
        using (var stream = new FileStream(serviceAccountFile, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(DriveService.Scope.Drive);
        }

        _service = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyApp"
        });
    }

    public async Task<ApiResponse<string>> UploadAsync(UploadRequest request)
    {
        try
        {
            var file = request.File;
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadAsync called with empty file");
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "File is null or empty",
                    Data = null
                };
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var metadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { _folderId }
            };

            var uploadRequest = _service.Files.Create(metadata, ms, file.ContentType);
            uploadRequest.Fields = "id, name, webViewLink";
            uploadRequest.SupportsAllDrives = false; // Không dùng Shared Drive

            var result = await uploadRequest.UploadAsync();

            if (result.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                _logger.LogError("Upload failed: {Status} - {Exception}", result.Status, result.Exception);
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Upload failed: {result.Exception?.Message ?? result.Status.ToString()}",
                    Data = null
                };
            }

            var uploaded = uploadRequest.ResponseBody;

            if (uploaded == null || string.IsNullOrEmpty(uploaded.Id))
            {
                _logger.LogError("Upload failed: ResponseBody or File ID is null");
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Upload failed: ResponseBody or File ID is null",
                    Data = null
                };
            }

            var url = uploaded.WebViewLink ?? $"https://drive.google.com/file/d/{uploaded.Id}/view";

            _logger.LogInformation("File uploaded successfully: {FileName} - {Url}", file.FileName, url);

            return new ApiResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Upload Success",
                Data = url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Google Drive upload");
            return new ApiResponse<string>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Upload failed: {ex.Message}",
                Data = null
            };
        }
    }
}
