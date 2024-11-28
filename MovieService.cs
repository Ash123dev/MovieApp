	using MovieApp.Data;
using MovieApp.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;

public class MovieService
{
	private readonly string _apiKey;
	private readonly MovieDbContext _context;

	public MovieService(MovieDbContext context,IConfiguration  configuration)
	{
		_context = context;
		_apiKey = configuration["TMDB:ApiKey"];
	}

    public async Task FetchAndStoreMoviesAsync()
    {
        try
        {

            //var options = new RestClientOptions("")
            //{
            //    MaxTimeout = -1,
            //};
            //var client = new RestClient(options);
            //var request = new RestRequest("https://api.themoviedb.org/3/movie/popular?api_key=", Method.Get);
            //RestResponse response = await client.ExecuteAsync(request);


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            var options = new RestClientOptions("https://api.themoviedb.org/3/movie/popular")
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            var client = new RestClient(options);
            var request = new RestRequest();
            request.AddQueryParameter("api_key", _apiKey);

            var response = await client.GetAsync(request);

            if (response == null)
            {
                Console.WriteLine("No response from the server.");
                return;
            }

            if (response.IsSuccessful)
            {
                var movies = JsonConvert.DeserializeObject<MovieApiResponse>(response.Content);
                if (movies?.Results != null)
                {
                    foreach (var movie in movies.Results)
                    {
                        if (!_context.Movies.Any(m => m.Id == movie.Id))
                        {
                            _context.Movies.Add(new Movie
                            {
                                Id = movie.Id,
                                Name = movie.Title,
                                Year = DateTime.Parse(movie.ReleaseDate).Year,
                                Overview = movie.Overview,
                                Popularity = movie.Popularity
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
            }
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
	public string ReleaseDate { get; set; }
	public string Overview { get; set; }
	public double Popularity { get; set; }
}