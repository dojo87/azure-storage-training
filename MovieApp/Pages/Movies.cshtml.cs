using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.ViewModel;
using azureTable = Azure.Data.Tables;

namespace MovieApp.Pages
{
    public class MoviesPageModel : PageModel
    {
        private readonly IConfiguration config;

        public MoviesPageModel(IConfiguration config)
        {
            this.config = config;
        }

        [BindProperty]
        public List<MovieModel> Movies { get; set; }

        public async Task<IActionResult> OnGet()
        {
            azureTable.TableClient client = new azureTable.TableClient(this.config["MoviesTable"], "movies");

            Movies = await client
                .QueryAsync<MovieModel>()
                .ToListAsync();

            return Page();
        }
    }
}
