﻿using System.Threading.Tasks;
using Microsoft.Graph;
using NRCan.Datahub.Shared.Data;
using NRCan.Datahub.Shared.Services;

namespace NRCan.Datahub.Portal.Services.Offline
{
    public class OfflineDataRemovalService : IDataRemovalService
    {
        public OfflineDataRemovalService()
        {
        }

        public Task<bool> Delete(Shared.Data.Folder folder, User currentUser)
        {
            return Task.FromResult(true);
        }

        public Task<bool> Delete(FileMetaData file, User currentUser)
        {
            return Task.FromResult(true);
        }
    }
}