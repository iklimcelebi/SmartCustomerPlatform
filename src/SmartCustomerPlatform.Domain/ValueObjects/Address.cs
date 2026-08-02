namespace SmartCustomerPlatform.Domain.ValueObjects;

public class Address
{
    public string City { get; init; } = string.Empty;
    public string District { get; init; } = string.Empty;

    public string Street { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string Country { get; init; } = "Turkey";
}