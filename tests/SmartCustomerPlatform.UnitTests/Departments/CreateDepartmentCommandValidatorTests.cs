using SmartCustomerPlatform.Application.Features.Departments.Commands.CreateDepartment;
using SmartCustomerPlatform.Application.Features.Departments.Commands.CreateDepartment.Validators;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.UnitTests.Departments;

public class CreateDepartmentCommandValidatorTests
{
    private readonly CreateDepartmentCommandValidator _validator;

    public CreateDepartmentCommandValidatorTests()
    {
        _validator = new CreateDepartmentCommandValidator();
    }

    [Fact]
    public void Valid_department_should_pass_validation()
    {
        // Arrange
        var command = new CreateDepartmentCommand(
            "TECH",
            "Technical Support",
            "Technical customer support department",
            DepartmentStatus.Active);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Empty_code_should_fail_validation()
    {
        // Arrange
        var command = new CreateDepartmentCommand(
            "",
            "Technical Support",
            "Technical customer support department",
            DepartmentStatus.Active);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Code");
    }
}