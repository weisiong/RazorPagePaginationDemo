using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RazorPagePaginationDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //page number variable
        [BindProperty(SupportsGet = true)]
        public int P { get; set; } = 1;

        //page size variable
        [BindProperty(SupportsGet = true)]
        public int S { get; set; } = 10;

        //total number of records
        public int TotalRecords { get; set; } = 0;

        //variable for text search
        [BindProperty(SupportsGet = true)]
        public string Q { get; set; } = string.Empty;

        public IEnumerable<string> ViewModels { get; set; }
        public void OnGet()
        {
            var query = GetStuffFromDatabase();

            //if the search text is not empty, then apply where clause
            if (!string.IsNullOrWhiteSpace(Q))
            {
                var _keyWords = Q.Split(new[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries).Distinct();

                //null check is not required in our case for this sample, 
                //added just for demonstration in case the search will be done in nullable database fields
                query = query.Where(x => _keyWords.Any(kw => x != null && x.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            TotalRecords = query.Count();

            var lst = query
                //make sure to order items before paging
                .OrderBy(x => x)
                //skip items before current page
                .Skip((P - 1) * S)
                //take only 10 (page size) items
                .Take(S)
                .ToList();
            ViewModels = lst;
        }


        protected IEnumerable<string> GetStuffFromDatabase()
        {
            var sampleData = System.IO.File.ReadAllText("Names.txt");
            return sampleData.Split('\n');
        }

    }
}
