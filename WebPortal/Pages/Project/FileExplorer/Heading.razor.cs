using Microsoft.JSInterop;

namespace Datahub.Portal.Pages.Project.FileExplorer;

public partial class Heading
{
    private async Task BreadcrumbClicked(string breadcrumb)
    {
        var index = CurrentFolder.IndexOf(breadcrumb, StringComparison.OrdinalIgnoreCase);
        await SetCurrentFolder.InvokeAsync(CurrentFolder[..(index + breadcrumb.Length)] + "/");
    }

    private async Task HandleDownload()
    {
        if (_selectedFile is not null)
        {
            await OnFileDownload.InvokeAsync(_selectedFile.name);
        }
    }

    private void HandleShare()
    {
        if (_selectedFile is null)
            return;

        var sb = new System.Text.StringBuilder();
        sb.Append("/sharingworkflow/");
        sb.Append(_selectedFile.fileid);
        sb.Append("?filename=");
        sb.Append(_selectedFile.filename);
        if (!string.IsNullOrWhiteSpace(ProjectAcronym))
        {
            sb.Append("&project=");
            sb.Append(ProjectAcronym);
        }
        else
        {
            sb.Append("&folderpath=");
            sb.Append(_selectedFile.folderpath);
        }

        _navigationManager.NavigateTo(sb.ToString());
    }

    private async Task HandleDelete()
    {
        if (_selectedFile is not null && _ownsSelectedFile)
        {
            await OnFileDelete.InvokeAsync(_selectedFile.name);
        }
    }

    private async Task HandleRename()
    {
        if (_selectedFile is not null && _ownsSelectedFile)
        {
            var newName = await _jsRuntime.InvokeAsync<string>("prompt", "Enter new name", 
                FileExplorer.GetFileName(_selectedFile.filename));
            newName = newName?.Replace("/", "").Trim();

            await OnFileRename.InvokeAsync(newName);
        }
    }

    private async Task HandleNewFolder()
    {
        var newFolderName = await _module.InvokeAsync<string>("promptForNewFolderName");
        if (!string.IsNullOrWhiteSpace(newFolderName))
        {
            await OnNewFolder.InvokeAsync(newFolderName.Trim());
        }
    }

    private enum ButtonAction
    {
        Download,
        Share,
        Delete,
        Rename
    }

    private bool IsActionDisabled(ButtonAction buttonAction)
    {
        return buttonAction switch
        {
            ButtonAction.Download => _selectedFile is null || !_ownsSelectedFile || SelectedItems.Count > 1,
            ButtonAction.Share => _selectedFile is null || !_ownsSelectedFile || SelectedItems.Count > 1,
            ButtonAction.Delete => _selectedFile is null || !_ownsSelectedFile || SelectedItems.Count > 1,
            ButtonAction.Rename => _selectedFile is null || !_ownsSelectedFile || SelectedItems.Count > 1,
            _ => false
        };
    }
}