using Microsoft.JSInterop;

namespace Datahub.Portal.Pages.Admin.Requests.Registration;

public partial class RegistrationActions
{

    private async Task HandleEditAcronym()
    {
        var acronym = await _jsRuntime.InvokeAsync<string>("prompt", "Please enter the new acronym", RegistrationRequest.ProjectAcronym ?? "");

        if (!await _registrationService.IsValidUniqueProjectAcronym(acronym))
        {
            await _jsRuntime.InvokeVoidAsync("alert", "The acronym is not unique or valid");
            return;
        }
        RegistrationRequest.ProjectAcronym = acronym.ToUpper();
        await OnRequestUpdated.InvokeAsync(RegistrationRequest);
    }
    
}