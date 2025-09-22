using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Specifications.Post_Specs
{
    public class PostSpecParam
    {
        public string? CategorySlug { get; set; }
        public int? TagId { get; set; }
        public string? Sort { get; set; }
        private const int MaxPageSize = 10;
        private int pageSize=5;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        private int pageIndex = 1;

        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value < 1 ? 1 : value; }
        }

        private string? search;
        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
        private string? authorId;

        public string? AuthorId
        {
            get { return authorId; }
            set { authorId = value?.ToLower(); }
        }


    }
}
