using Datahub.Core.EFCore;
using Datahub.Portal.Data.Forms;
using Microsoft.EntityFrameworkCore;

namespace Datahub.Portal.Services;

public class RegistrationService
{
    public static readonly string SELF_SIGNUP = "self-signup page";
    
    private readonly IDbContextFactory<DatahubProjectDBContext> _dbFactory;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(IDbContextFactory<DatahubProjectDBContext> dbFactory, ILogger<RegistrationService> logger)
    {
        _dbFactory = dbFactory;
        _logger = logger;
    }
    
    public async Task SubmitRegistration(BasicIntakeForm basicIntakeForm, string createdBy)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var exists = await db.Registration_Requests
            .FirstOrDefaultAsync(r => r.Email == basicIntakeForm.Email);

        if (exists != null)
        {
            _logger.LogInformation("Registration request for {Email} already exists", basicIntakeForm.Email);
            exists.UpdatedBy = createdBy;
            exists.ProjectName = basicIntakeForm.ProjectName;
            exists.DepartmentName = basicIntakeForm.DepartmentName;
            exists.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _logger.LogInformation("Submitting new Registration request for {Email} from {Department} on {Project}]",
                basicIntakeForm.Email, basicIntakeForm.DepartmentName, basicIntakeForm.ProjectName);

            var registrationRequest = new Datahub_Registration_Request()
            {
                Email = basicIntakeForm.Email,
                DepartmentName = basicIntakeForm.DepartmentName,
                ProjectName = basicIntakeForm.ProjectName,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now,
            };
            await db.Registration_Requests.AddAsync(registrationRequest);
        }

        await db.SaveChangesAsync();
    }
}