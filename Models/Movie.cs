using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
	public class MovieTable
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Year { get; set; }
		public string Overview { get; set; } // Additional Data
		public double Popularity { get; set; } // Additional Data
	}
}
