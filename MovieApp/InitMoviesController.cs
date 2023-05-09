using Azure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.ViewModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using azureTable = Azure.Data.Tables;
using System.Linq;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieApp
{
    [Route("api/InitMovies")]
    [ApiController]
    public class InitMoviesController : ControllerBase
    {
        private readonly IConfiguration config;

        public InitMoviesController(IConfiguration config) 
        {
            this.config = config;
        }

        [HttpPost]
        public async Task Post()
        {
            try
            {
                string moviesJson = System.IO.File.ReadAllText(this.config["MoviesInit"]);
                var movies = JsonSerializer.Deserialize<MoviesModel>(moviesJson);

                azureTable.TableClient client = new azureTable.TableClient(this.config["MoviesTable"], "movies");
                await client.CreateIfNotExistsAsync();
                foreach (var movie in movies.Movies)
                {
                    await client.UpsertEntityAsync(movie);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<MoviesModel> Get(string continuationToken=null, int pageSize = 10)
        {
            try
            {
                azureTable.TableClient client = new azureTable.TableClient(this.config["MoviesTable"], "movies");

                var moviesList = client
                    .QueryAsync<MovieModel>(maxPerPage: pageSize);

                var pages = moviesList.AsPages(continuationToken, pageSize);
                var moviesResult = await pages.FirstOrDefaultAsync();
                return new MoviesModel()
                {
                    Movies = moviesResult.Values.ToArray(),
                    ContinuationToken = moviesResult.ContinuationToken
                };
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}
