using Application.Common.ElasticSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Elasticsearch
{
    public class ElasticIndexNameResolver : IElasticIndexNameResolver
    {
        public string Resolve<T>()
        {
            return typeof(T).Name
                .Replace("ElasticDto", "")
                .ToLower() + "s";
        }
    }
}
