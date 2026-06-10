using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Helper
{
    public class DataHelper(IUnitOfWork.IUnitOfWork _unitOfWork, ILogger<DataHelper> _logger)
    {
        public async Task<IEnumerable<dynamic>> ExecuteProcedureAsync(string procedure, object parameters, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = _unitOfWork.Context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                var rows = await connection.QueryAsync<dynamic>(
                       procedure,
                       parameters,
                       commandType: System.Data.CommandType.StoredProcedure
                    );

                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã có lỗi xảy ra khi thực thi Procedure: {Procedure}", procedure);
                throw; 
            }
        }

        public async Task<IEnumerable<T>> ExecuteProcedureAsync<T>(string procedure, object parameters, CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = _unitOfWork.Context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                var rows = await connection.QueryAsync<T>(
                       procedure,
                       parameters,
                       commandType: System.Data.CommandType.StoredProcedure
                    );

                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã có lỗi xảy ra khi thực thi Procedure: {Procedure}", procedure);
                throw; 
            }
        }
    }
}
