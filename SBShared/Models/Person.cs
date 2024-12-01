
using System.ComponentModel.DataAnnotations;

public class Person
{
	[Required]
	public string FirstName { get; set; } = string.Empty;

	[Required]
	public string LastName { get; set; } = string.Empty;
}