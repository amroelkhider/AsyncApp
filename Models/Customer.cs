namespace AsyncApp.Models;

public class Customer
{
    public int Index { get; set; }
    public required string CustomerId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Company { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public required string Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public required string Email { get; set; }
    public DateTime SubscriptionDate { get; set; }
    public string? Website { get; set; }

}
