using Application.Common.ElasticSearch;
using Application.IGenericRepository;
using Elastic.Clients.Elasticsearch;

namespace Infrastructure.Elasticsearch
{
    public class ElasticRepository<T> : IElasticRepository<T>
        where T : class
    {
        private readonly ElasticsearchClient _client;
        private readonly string _index;

        public ElasticRepository(
            ElasticsearchClient client,
            IElasticIndexNameResolver resolver)
        {
            _client = client;
            _index = resolver.Resolve<T>();
        }

        public async Task IndexAsync(T document, string name)
        {
            await _client.IndexAsync(document, i => i
                .Index(_index)
                .Id(name)
            );
        }

        public async Task UpdateAsync(T document, Guid id)
        {
            await _client.IndexAsync(document, i => i
                .Index(_index)
                .Id(id)
            );
        }

        public async Task DeleteAsync(Guid id)
        {
            await _client.DeleteAsync(_index, id);
        }

        public async Task<List<T>> SearchAsync(string keyword)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(_index)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(keyword)
                        .Fields("*")
                    )
                )
            );

            return response.Documents.ToList();
        }

        public async Task<Tuple<List<T>, long>> SearchPagingAsync(string keyword, int pageNumber, int pageSize)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Index(_index)
                .From((pageNumber - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(keyword)
                        .Fields("*")
                    )
                )
            );

            return new Tuple<List<T>, long>(response.Documents.ToList(), response.Total);
        }
    }
}
