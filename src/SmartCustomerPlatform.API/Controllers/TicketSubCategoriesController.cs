using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.TicketSubCategories.Commands.CreateTicketSubCategory;
using SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategories;
using SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoriesByCategory;
using SmartCustomerPlatform.Application.Features.TicketSubCategories.Queries.GetTicketSubCategoryById;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketSubCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketSubCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTicketSubCategoryCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(
            new GetTicketSubCategoriesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(
            new GetTicketSubCategoryByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategory(Guid categoryId)
    {
        var result = await _mediator.Send(
            new GetTicketSubCategoriesByCategoryQuery(categoryId));

        return Ok(result);
    }
}
