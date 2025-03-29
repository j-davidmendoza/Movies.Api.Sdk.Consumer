
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Consumer;
using Movies.Contracts.Requests;
using Refit;
using System.Text.Json;

///var moviesApi = RestService.For<IMoviesApi>("https://localhost:52578");

var services = new ServiceCollection();
services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    {
        AuthorizationHeaderValueGetter = async () => await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync(),
    })
    .ConfigureHttpClient(x =>
    x.BaseAddress = new Uri("https://localhost:56288"));
var provider = services.BuildServiceProvider();
// above Takes care of handling the http client using httpclienfactory 
var moviesApi = provider.GetRequiredService<IMoviesApi>();


var movie = await moviesApi.GetMovieAsync("saving-ryans-privates-2-1998");
//Console.WriteLine(JsonSerializer.Serialize(movie));


var newMovie = await moviesApi.CreateMovieAsync(new CreateMovieRequest
{
    Title = "Saving Ryan's Privates 3",
    YearOfRelease = 2001,
    Genres = new[] { "Comedy" },

});

await moviesApi.UpdateMovieAsync(newMovie.Id, new UpdateMovieRequest()
{
    Title = "Saving Ryan's Privates 3",
    YearOfRelease = 2001,
    Genres = new[] { "Action", "Adventure" },

});

await moviesApi.DeleteMovieAsync(newMovie.Id);
var request = new GetAllMoviesRequest
    { Title = null, Year = null, SortBy = null, Page = 1, PageSize = 3 };
var movies = await moviesApi.GetMoviesAsync(request);
foreach (var movieResponse in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));

}

//Console.WriteLine(JsonSerializer.Serialize(movies));
