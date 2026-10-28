using Application.IService;
using Application.Payload.Request.Uploads;
using Domain.Payload.Base;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Service;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly IConfiguration _config;
    private readonly ILogger<GoogleDriveService> _logger;
    private DriveService? _service;
    private string? _folderId;
    private readonly object _lock = new();

    public GoogleDriveService(IConfiguration config, ILogger<GoogleDriveService> logger)
    {
        _config = config;
        _logger = logger;
    }

    private DriveService GetService()
    {
        if (_service != null) return _service;

        lock (_lock)
        {
            if (_service != null) return _service;

            var settings = _config.GetSection("GoogleDrive");
            _folderId = settings["FolderId"];
            var serviceAccountFile = settings["ServiceAccountKeyFile"];

            if (string.IsNullOrEmpty(_folderId))
                throw new InvalidOperationException("GoogleDrive:FolderId is missing in configuration.");

            if (string.IsNullOrEmpty(serviceAccountFile) || !File.Exists(serviceAccountFile))
                throw new FileNotFoundException($"GoogleDrive:ServiceAccountKeyFile path is invalid or file not found: {serviceAccountFile}");

            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountFile, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.Scope.Drive);
            }

            _service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "CleanArchitectureBase"
            });

            return _service;
        }
    }

    public async Task<ApiResponse<string>> UploadAsync(UploadRequest request)
    {
        try
        {
            var file = request.File;
            if (file == null || file.Length == 0)
            {
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "File khÃ´ng Ä‘Æ°á»£c Ä‘á»ƒ trá»‘ng",
                    Data = null
                };
            }

            var driveService = GetService();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var metadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = string.IsNullOrEmpty(_folderId) ? null : new List<string> { _folderId }
            };

            var uploadRequest = driveService.Files.Create(metadata, ms, file.ContentType);
            uploadRequest.Fields = "id, name, webViewLink";

            var result = await uploadRequest.UploadAsync();

            if (result.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                _logger.LogError("Google Drive Upload failed: {Message}", result.Exception?.Message);
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = $"Táº£i lÃªn Google Drive tháº¥t báº¡i: {result.Exception?.Message ?? result.Status.ToString()}",
                    Data = null
                };
            }

            var uploaded = uploadRequest.ResponseBody;
            if (uploaded == null || string.IsNullOrEmpty(uploaded.Id))
            {
                return new ApiResponse<string>
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "KhÃ´ng nháº­n Ä‘Æ°á»£c pháº£n há»“i File ID tá»« Google Drive",
                    Data = null
                };
            }

            var url = uploaded.WebViewLink ?? $"https://drive.google.com/file/d/{uploaded.Id}/view";
            return new ApiResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Táº£i lÃªn Google Drive thÃ nh cÃ´ng",
                Data = url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Google Drive upload: {Message}", ex.Message);
            return new ApiResponse<string>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = $"Lá»—i khi táº£i lÃªn Google Drive: {ex.Message}",
                Data = null
            };
        }
    }
}
