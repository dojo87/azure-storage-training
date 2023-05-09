using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovieApp.ViewModel;
using azureTable = Azure.Data.Tables;
using azureBlob = Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs;

namespace MovieApp.Pages
{
    public class MoviePageModel : PageModel
    {
        private readonly IConfiguration config;

        public MoviePageModel(IConfiguration config)
        {
            this.config = config;
        }

        [BindProperty]
        public MovieModel Movie { get; set; }


        [BindProperty]
        public Uri MovieUri { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            await SetMovie(id);

            BlobClient movieBlob = GetMovieBlob(id);

            SetMovieUriWithSas(movieBlob);
            await SetMovieCounterAsync(movieBlob);

            return Page();
        }

        private BlobClient GetMovieBlob(string id)
        {
            return new azureBlob.BlobClient(this.config["BlobStorageAdmin"], "movies", $"{id}.mp4");
        }

        private void SetMovieUriWithSas(BlobClient movieBlob)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = movieBlob.BlobContainerName,
                BlobName = movieBlob.Name,
                Resource = "b"
            };

            sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2);
            sasBuilder.SetPermissions(BlobSasPermissions.Read |
                BlobSasPermissions.Write);

            MovieUri = movieBlob.GenerateSasUri(sasBuilder);
        }

        private async Task SetMovieCounterAsync(BlobClient movieBlob)
        {
            var properties = await movieBlob.GetPropertiesAsync();
            var metadata = properties.Value.Metadata;
            if (!metadata.ContainsKey("ViewCounter"))
            {
                metadata.Add("ViewCounter", "0");
            }

            var counter = int.Parse(metadata["ViewCounter"]);
            counter++;
            metadata["ViewCounter"] = counter.ToString();
            await movieBlob.SetMetadataAsync(metadata);
        }

        private async Task SetMovie(string id)
        {
            azureTable.TableClient client = new azureTable.TableClient(this.config["MoviesTable"], "movies");

            Movie = await client.GetEntityAsync<MovieModel>(id, id);
        }
    }
}
