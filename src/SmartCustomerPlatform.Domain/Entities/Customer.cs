using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Domain.ValueObjects;

namespace SmartCustomerPlatform.Domain.Entities;
public class Customer : BaseEntity //inheritance from BaseEntity class
{
    public string TCKNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;


    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }
    
    public Gender Gender { get; set; }

    public CustomerType CustomerType { get; set; } 


    public Address Address { get;  set; } = new();
    
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
}