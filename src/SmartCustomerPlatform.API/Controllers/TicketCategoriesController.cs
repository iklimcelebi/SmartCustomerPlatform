using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.TicketCategories.Commands.CreateTicketCategory;
using SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategories;
using SmartCustomerPlatform.Application.Features.TicketCategories.Queries.GetTicketCategoryById;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTicketCategoryCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(Get),
            new { id },
            id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(
            new GetTicketCategoriesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(
            new GetTicketCategoryByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
