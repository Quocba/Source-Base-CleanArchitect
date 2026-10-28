using Application.Payload.Request.Uploads;
using Domain.Payload.Base;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IGoogleDriveService
    {
        Task<ApiResponse<string>> UploadAsync(UploadRequest request);
    }
}
