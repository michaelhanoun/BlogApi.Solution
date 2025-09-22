namespace Blog.Api.Helper
{
    public class Pagination<T>
    {
        public Pagination(int index,int size,int count,IReadOnlyList<T> data)
        {
            PageIndex = index;
            PageSize = size;
            Count = count;
            Data = data;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; }
    }
}
