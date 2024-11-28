using MovieApp.Data;
using MovieApp.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

public class MovieService
{
    private readonly string _apiKey;
    private readonly MovieDbContext _context;

    public MovieService(MovieDbContext context, IConfiguration configuration)
    {
        _context = context;
        _apiKey = configuration["TMDB:ApiKey"];
    }

    #region [comment]
    //public async Task FetchAndStoreMoviesAsync()
    //{
    //    try
    //    {

    //        //var options = new RestClientOptions("")
    //        //{
    //        //    MaxTimeout = -1,
    //        //};
    //        //var client = new RestClient(options);
    //        //var request = new RestRequest("https://api.themoviedb.org/3/movie/popular?api_key=", Method.Get);
    //        //RestResponse response = await client.ExecuteAsync(request);


    //        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

    //        //var options = new RestClientOptions("https://api.themoviedb.org/3/movie/popular")
    //        //{
    //        //    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
    //        //};

    //        //var client = new RestClient(options);
    //        //var request = new RestRequest();
    //        //request.AddQueryParameter("api_key", _apiKey);

    //        //var response = await client.GetAsync(request);

    //        var options = new RestClientOptions("https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=popularity.desc");
    //        var client = new RestClient(options);
    //        var request = new RestRequest("");
    //        request.AddHeader("accept", "application/json");
    //        request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI2N2MxYWRjNGUxY2FkOWYyNDI5MmE2ODgyNzNjZGNhYSIsIm5iZiI6MTczMjgyMjA1Ny43NjcwMjc2LCJzdWIiOiI2NzQ4ODVlMDFkMGI1YjNmYjgzM2ZlNGEiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.E5JpCEEgI5ZJV1R8YgUhwoCctfdRqEYeDi6qYzqwr1g");
    //        var response = await client.GetAsync(request);

    //        Console.WriteLine("{0}", response.Content);

    //        if (response == null)
    //        {
    //            Console.WriteLine("No response from the server.");
    //            return;
    //        }

    //        if (response.IsSuccessful)
    //        {
    //            var movies = JsonConvert.DeserializeObject<MovieApiResponse>(response.Content);
    //            if (movies?.Results != null)
    //            {
    //                foreach (var movie in movies.Results)
    //                {
    //                    if (!_context.Movies.Any(m => m.Id == movie.Id))
    //                    {
    //                        _context.Movies.Add(new Movie
    //                        {
    //                            Id = movie.Id,
    //                            Name = movie.Title,
    //                            Year = DateTime.Parse(movie.ReleaseDate).Year,
    //                            Overview = movie.Overview,
    //                            Popularity = movie.Popularity
    //                        });
    //                    }
    //                }
    //                await _context.SaveChangesAsync();
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Exception: {ex.Message}");
    //        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    //    }
    //}
    #endregion

    public async Task FetchAndStoreMoviesAsync()
    {
        try
        {
            int page = 2;

            System.Net.ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var options = new RestClientOptions("https://api.themoviedb.org/3/discover/movie")
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };


            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddQueryParameter("include_adult", "false");
            request.AddQueryParameter("include_video", "false");
            request.AddQueryParameter("page", page.ToString());
            request.AddQueryParameter("language", "en-US");
            request.AddQueryParameter("sort_by", "popularity.desc");

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI2N2MxYWRjNGUxY2FkOWYyNDI5MmE2ODgyNzNjZGNhYSIsIm5iZiI6MTczMjgyMjA1Ny43NjcwMjc2LCJzdWIiOiI2NzQ4ODVlMDFkMGI1YjNmYjgzM2ZlNGEiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.E5JpCEEgI5ZJV1R8YgUhwoCctfdRqEYeDi6qYzqwr1g");// Execute the GET request asynchronously
            var response = await client.GetAsync(request);

            if (response == null)
            {
                Console.WriteLine("No response from the server.");
                return;
            }

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
                return;
            }

            var movies = JsonConvert.DeserializeObject<MovieApiResponse>(response.Content);
            if (movies?.Results == null)
            {
                Console.WriteLine("No movies found in the response.");
                return;
            }

            foreach (var movie in movies.Results)
            {
                if (!_context.MovieTables.Any(m => m.Id == movie.Id))
                {
                    _context.MovieTables.Add(new MovieTable
                    {
                        Name = movie.Title,
                        Year = (movie.ReleaseDate).ToString(),
                        Overview = movie.Overview,
                        Popularity = movie.Popularity
                    });
                }
            }

            await _context.SaveChangesAsync();

            Console.WriteLine($"Page {page}: Movies successfully fetched and stored.");
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"JSON Parsing Error: {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}

public class MovieApiResponse
{
    public List<MovieResult> Results { get; set; }
}

public class MovieResult
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Overview { get; set; }
    public double Popularity { get; set; }
}