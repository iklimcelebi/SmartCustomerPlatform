using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartCustomerPlatform.Application.Features.Packages.Commands.CreatePackage;
using SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackageById;
using SmartCustomerPlatform.Application.Features.Packages.Queries.GetPackages;

namespace SmartCustomerPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PackagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePackageCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetPackagesQuery());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _mediator.Send(new GetPackageByIdQuery(id));

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}
