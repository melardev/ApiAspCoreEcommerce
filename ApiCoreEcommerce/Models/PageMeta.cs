using System;

namespace BlogDotNet.Models
{
    public class PageMeta
    {
        public int CurrentPage { get; set; }

        public int TotalItemsCount { get; set; }
        public int CurrentItemsCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPrevPage { get; set; }
        public string PrevUrl { get; set; }
        public string NextUrl { get; set; }

        public int PreviousPage { get; set; }
        public int NextPage { get; set; }
        public string BasePath { get; set; }
        public int TotalPagesCount { get; set; }
        public int RequestedPageSize { get; set; }

        public PageMeta()
        {
        }

        public PageMeta(int currentItemsCount, string basePath, int currentPage, int pageSize, int totalItemCount)
        {
            CurrentPage = currentPage;
            CurrentItemsCount = currentItemsCount;
            TotalItemsCount = totalItemCount;
            BasePath = basePath;
            RequestedPageSize = pageSize;

            PreviousPage = currentPage;
            NextPage = currentPage;

            var skipt = (CurrentPage - 1) * RequestedPageSize;
            var traversedSoFar = skipt + CurrentItemsCount;
            var remaining = TotalItemsCount - traversedSoFar;
            HasNextPage = remaining > pageSize;
            HasPrevPage = currentPage > 1;
            if (pageSize == 0) // avoid the 0/0 Division
                TotalPagesCount = 0;
            else
                TotalPagesCount = (int) Math.Ceiling((decimal) (totalItemCount/pageSize ));


            if (HasNextPage)
                NextPage = CurrentPage + 1;

            NextUrl = $"{basePath}/?page={NextPage}&pageSize={RequestedPageSize}";

            if (HasPrevPage)
                PreviousPage = CurrentPage - 1;


            PrevUrl = $"{basePath}/?page={PreviousPage}&pageSize={RequestedPageSize}";
        }
    }
}