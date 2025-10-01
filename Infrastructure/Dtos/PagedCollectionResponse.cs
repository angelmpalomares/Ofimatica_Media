namespace Application.Services.Implementations
{
    public class PagedCollectionResponse<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int Total {  get; set; }
        public PagedCollectionResponse(IEnumerable<T> items, int Total)
        {
            this.Items = items;
            this.Total = Total;
        }

        public PagedCollectionResponse()
        {
        }
    }
}
