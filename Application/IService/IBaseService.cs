using Application.Payload.Request.Products;
using Domain.Payload.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IBaseService
    {
        Task<ApiResponse<string>> CreateProduct(CreateProductRequest request);
    }
}
