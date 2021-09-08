﻿using System.Threading.Tasks;
using Microsoft.Graph;
using NRCan.Datahub.Shared.Data;

namespace NRCan.Datahub.Shared.Services
{
    public interface IDataRemovalService
    {
        Task<bool> Delete(Shared.Data.Folder folder, User currentUser);
        Task<bool> Delete(FileMetaData file, User currentUser);
        Task<bool> DeleteStorageBlob(FileMetaData file, string project, User currentUser);
    }
}