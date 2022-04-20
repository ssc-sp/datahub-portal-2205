using Datahub.Core.EFCore;
using Datahub.Portal.Data.Forms;
using Microsoft.EntityFrameworkCore;

namespace Datahub.Portal.Services;

public class RegistrationService
{
    public const string SELF_SIGNUP = "self-signup page";

    private readonly IDbContextFactory<DatahubProjectDBContext> _dbFactory;
    private readonly ILogger<RegistrationService> _logger;

    private readonly List<string> _blacklistedAcronyms = new()
    {
        "new",
    };

    public RegistrationService(IDbContextFactory<DatahubProjectDBContext> dbFactory, ILogger<RegistrationService> logger)
    {
        _dbFactory = dbFactory;
        _logger = logger;
    }

    public async Task<bool> IsValidRegistrationRequest(Datahub_Registration_Request registrationRequest)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var exists = await db.Registration_Requests
            .FirstOrDefaultAsync(r => r.Email == registrationRequest.Email);

        if (exists == null)
            return false;

        var validAcronym = await IsValidUniqueProjectAcronym(exists.ProjectAcronym);

        return validAcronym;
    }

    public async Task<bool> IsValidUniqueProjectAcronym(string projectAcronym)
    {
        if (string.IsNullOrWhiteSpace(projectAcronym) || _blacklistedAcronyms.Any(a => a.ToLower() == projectAcronym.ToLower()))
            return false;
        
        await using var db = await _dbFactory.CreateDbContextAsync();
        
        return !await db.Projects.AnyAsync(p =>
            p.Project_Acronym_CD.ToLower() == projectAcronym.ToLower());
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