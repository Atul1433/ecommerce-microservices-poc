using Ordering.Domain.Abstractions;

namespace Ordering.Domain.Models;

public class Customer : Entity<CustomerId>
{
    public string Name { get; private set; } = default;
    public string Email { get; private set; } = default;

    public static Customer Create(CustomerId id, string Name, string Email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(Name);
        ArgumentException.ThrowIfNullOrWhiteSpace(Email);

        var customer = new Customer
        {
            Id = id,
            Name = Name,
            Email = Email
        };
        return customer;
    }   
}