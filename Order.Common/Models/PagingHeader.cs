namespace Order.Common.Models
{
    public class PagingHeader
    {
        public PagingHeader(int currentPage,int itemsPerPage,
                            int totalItems, int totalPage)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPage;
        }

        public int CurrentPage { get; }

        public int ItemsPerPage { get; }

        public int TotalItems { get; }

        public int TotalPages { get; set; }
    }
}
