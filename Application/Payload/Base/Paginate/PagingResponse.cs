namespace Application.Payload.Base.Paginate
{
    public class PagingResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecord / PageSize);
        public List<T> Items { get; set; }

        public PagingResponse(List<T> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecord = totalCount;
        }
    }

}
