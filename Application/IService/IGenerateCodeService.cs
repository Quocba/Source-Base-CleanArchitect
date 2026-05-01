using System.Threading;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IGenerateCodeService
    {
        string GenerateDepartmentCode(string departmentName, int sequence);
        string GenerateEmployeeCode();
        string GenerateWarehouseCode(string address);
    }
}