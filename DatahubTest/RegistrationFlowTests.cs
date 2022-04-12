using System;
using System.Threading.Tasks;
using Datahub.Core.EFCore;
using Datahub.Portal.Data.Forms;
using Datahub.Portal.Services;
using Datahub.Tests.Portal;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Datahub.Tests;



public class RegistrationFlowTests
{
    private readonly MockProjectDbContextFactory _dbFactory;
    private readonly RegistrationService _registrationService;

    public RegistrationFlowTests()
    {
        _dbFactory = new MockProjectDbContextFactory();
        var logger = Mock.Of<ILogger<RegistrationService>>();
        _registrationService = new RegistrationService(_dbFactory, logger);
    }
    
    [Fact]
    public async Task EnsureDatabaseSharedTest()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        
        context.Registration_Requests.Add(
            new Datahub_Registration_Request
            {
                ProjectName = "EnsureDatabaseSharedTest Project",
                Email = "EnsureDatabaseSharedTest@email.com",
                DepartmentName = "EnsureDatabaseSharedTest Department",
                CreatedBy = RegistrationService.SELF_SIGNUP,
                CreatedAt = DateTime.Now
            });
        await context.SaveChangesAsync();
        
        await using var context2 = await _dbFactory.CreateDbContextAsync();
        Assert.NotEmpty(await context2.Registration_Requests.ToListAsync());
    }

    [Fact]
    public async Task RegistrationSubmitTest()
    {
        var intakeForm = new BasicIntakeForm()
        {
            DepartmentName = "Test department",
            ProjectName = "Test project",
            Email = "test@email.com",
        };
        
        await _registrationService.SubmitRegistration(intakeForm, RegistrationService.SELF_SIGNUP);
        
        await using var db = await _dbFactory.CreateDbContextAsync();
        var result = await db.Registration_Requests
            .FirstOrDefaultAsync(r => r.Email == intakeForm.Email);
        
        Assert.NotNull(result);
        Assert.Equal(intakeForm.Email, result.Email);
        Assert.Equal(intakeForm.DepartmentName, result.DepartmentName);
        Assert.Equal(intakeForm.ProjectName, result.ProjectName);
        Assert.NotNull(result.CreatedAt);
        Assert.Equal(RegistrationService.SELF_SIGNUP, result.CreatedBy);
        Assert.True(result.Id > 0, "Id should be set");
    }
    
    [Fact]
    public async Task RegistrationResubmitTest()
    {
        var intakeForm = new BasicIntakeForm()
        {
            DepartmentName = "Test department",
            ProjectName = "Test project",
            Email = "test@email.com",
        };
        
        await _registrationService.SubmitRegistration(intakeForm, RegistrationService.SELF_SIGNUP);
        
        intakeForm.ProjectName = "Test project 2";
        intakeForm.DepartmentName = "Test department 2";
        
        await _registrationService.SubmitRegistration(intakeForm, RegistrationService.SELF_SIGNUP);
        
        await using var db = await _dbFactory.CreateDbContextAsync();
        var result = await db.Registration_Requests
            .FirstOrDefaultAsync(r => r.Email == intakeForm.Email);
        
        Assert.NotNull(result);
        Assert.NotNull(result.UpdatedAt);
        Assert.Equal(RegistrationService.SELF_SIGNUP, result.UpdatedBy);
        Assert.Equal(intakeForm.DepartmentName, result.DepartmentName);
        Assert.Equal(intakeForm.ProjectName, result.ProjectName);
    }
}