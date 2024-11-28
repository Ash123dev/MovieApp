namespace MovieApp.Models
{
	public class Movie
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Year { get; set; }
		public string Overview { get; set; } // Additional Data
		public double Popularity { get; set; } // Additional Data
	}
}
