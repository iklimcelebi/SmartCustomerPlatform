using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Departments.Commands.CreateDepartment;
using SmartCustomerPlatform.Application.Features.Departments.Commands.UpdateDepartment;
using SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartmentById;
using SmartCustomerPlatform.Application.Features.Departments.Queries.GetDepartments;
using SmartCustomerPlatform.Application.Features.Departments.Commands.DeleteDepartment;
namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DepartmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDepartmentCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(Create),
            new { id },
            id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments = await _mediator.Send(
            new GetDepartmentsQuery());

        return Ok(departments);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var department = await _mediator.Send(
            new GetDepartmentByIdQuery(id));

        if (department is null)
            return NotFound();

        return Ok(department);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDepartmentCommand command)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id must match.");

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(
                new DeleteDepartmentCommand(id));

            if (!result)
                return NotFound();

            return NoContent();
        }




}