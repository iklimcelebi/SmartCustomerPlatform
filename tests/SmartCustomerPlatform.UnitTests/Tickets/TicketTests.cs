using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Domain.Enums;
using Xunit;

namespace SmartCustomerPlatform.UnitTests.Tickets;

public class TicketTests
{
    private static Ticket CreateTicket(
        TicketStatus status = TicketStatus.Open,
        TicketPriority priority = TicketPriority.Medium)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = "TCK-TEST-001",
            CustomerId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Subject = "Test ticket",
            Description = "Test description"
        };

        ticket.SetInitialPriority(priority);

        return ticket;
    }

    [Fact]
    public void ChangePriority_ShouldUpdatePriority()
    {
        var ticket = CreateTicket();

        ticket.ChangePriority(TicketPriority.High);

        Assert.Equal(TicketPriority.High, ticket.Priority);
    }

    [Fact]
    public void ChangePriority_OnClosedTicket_ShouldThrow()
    {
        var ticket = CreateTicket();

        ticket.ChangeStatus(TicketStatus.InProgress);
        ticket.ChangeStatus(TicketStatus.Resolved);
        ticket.ChangeStatus(TicketStatus.Closed);

        Assert.Throws<InvalidOperationException>(() =>
            ticket.ChangePriority(TicketPriority.High));
    }

    [Fact]
    public void TransferDepartment_ShouldChangeDepartment()
    {
        var ticket = CreateTicket();

        var newDepartmentId = Guid.NewGuid();

        ticket.TransferDepartment(newDepartmentId);

        Assert.Equal(newDepartmentId, ticket.DepartmentId);
    }

    [Fact]
    public void TransferDepartment_ShouldClearAssignedUser()
    {
        var ticket = CreateTicket();

        var userId = Guid.NewGuid();
        var newDepartmentId = Guid.NewGuid();

        ticket.Assign(userId);
        ticket.TransferDepartment(newDepartmentId);

        Assert.Null(ticket.AssignedUserId);
    }

    [Fact]
    public void TransferDepartment_OnClosedTicket_ShouldThrow()
    {
        var ticket = CreateTicket();

        ticket.ChangeStatus(TicketStatus.InProgress);
        ticket.ChangeStatus(TicketStatus.Resolved);
        ticket.ChangeStatus(TicketStatus.Closed);

        Assert.Throws<InvalidOperationException>(() =>
            ticket.TransferDepartment(Guid.NewGuid()));
    }

    [Fact]
    public void StatusTransition_OpenToInProgress_ShouldBeAllowed()
    {
        var ticket = CreateTicket();

        ticket.ChangeStatus(TicketStatus.InProgress);

        Assert.Equal(
            TicketStatus.InProgress,
            ticket.Status);
    }

    [Fact]
    public void StatusTransition_OpenToResolved_ShouldThrow()
    {
        var ticket = CreateTicket();

        Assert.Throws<InvalidOperationException>(() =>
            ticket.ChangeStatus(TicketStatus.Resolved));
    }

    [Fact]
    public void InitializeSla_ShouldSetSlaDates()
    {
        var ticket = CreateTicket(
            priority: TicketPriority.Medium);

        var startedAt = DateTime.UtcNow;

        ticket.InitializeSla(startedAt);

        Assert.Equal(startedAt, ticket.SlaStartedAt);
        Assert.True(
            ticket.SlaResponseDueAt > startedAt);
        Assert.True(
            ticket.SlaResolutionDueAt > startedAt);
        Assert.False(ticket.IsSlaPaused);
        Assert.Equal(
            TimeSpan.Zero,
            ticket.TotalSlaPausedDuration);
    }

    [Fact]
    public void WaitingForCustomer_ShouldPauseSla()
    {
        var ticket = CreateTicket();

        ticket.InitializeSla(DateTime.UtcNow);

        ticket.ChangeStatus(TicketStatus.InProgress);
        ticket.ChangeStatus(TicketStatus.WaitingForCustomer);

        Assert.True(ticket.IsSlaPaused);
        Assert.NotNull(ticket.SlaPausedAt);
    }

    [Fact]
    public void WaitingForCustomerToInProgress_ShouldResumeSla()
    {
        var ticket = CreateTicket();

        ticket.InitializeSla(DateTime.UtcNow);

        ticket.ChangeStatus(TicketStatus.InProgress);
        ticket.ChangeStatus(TicketStatus.WaitingForCustomer);

        ticket.ChangeStatus(TicketStatus.InProgress);

        Assert.False(ticket.IsSlaPaused);
        Assert.Null(ticket.SlaPausedAt);
        Assert.True(
            ticket.TotalSlaPausedDuration >= TimeSpan.Zero);
    }
}