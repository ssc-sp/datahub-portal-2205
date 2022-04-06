using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Datahub.Core.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datahub.Core.UserTracking;

namespace Datahub.Core.Services
{
    public class UserLocationManagerService
    {
        private ILogger<UserLocationManagerService> _logger;
        private IUserInformationService _userInformationService;
        private IDbContextFactory<UserTrackingContext> _userTrackingContextFactory;


        public UserLocationManagerService(ILogger<UserLocationManagerService> logger,
                                        IUserInformationService userInformationService,
                                        IDbContextFactory<UserTrackingContext> userTrackingContextFactory)
        {
            _logger = logger;
            _userInformationService = userInformationService;
            _userTrackingContextFactory = userTrackingContextFactory;
        }
        
        
        public const ushort MaxLocationHistory = 6;

        public async Task RegisterNavigation(UserRecentLink link, bool isNew)
        {
            try
            {
                var user = await _userInformationService.GetUserAsync();
                var userId = user.Id;

                await using var efCoreDatahubContext = await _userTrackingContextFactory.CreateDbContextAsync();

                var userRecent = await efCoreDatahubContext.UserRecent
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                
                if (userRecent == null)
                {
                    userRecent = new UserRecent { UserId = userId };
                    efCoreDatahubContext.UserRecent.Add(userRecent);
                }

                userRecent.UserRecentActions.Add(link);
                
                TrimExcessNavigations(userRecent);
                
                await efCoreDatahubContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cannot update navigation");
            }
        }

        private void TrimExcessNavigations(UserRecent userRecent)
        {
            if (userRecent == null)
                return;

            var excessNavigations = userRecent.UserRecentActions
                .OrderBy(x => x.accessedTime)
                .Skip(MaxLocationHistory)
                .ToList();
            
            _logger.LogInformation("Found {Count} excess navigations", excessNavigations.Count);

            foreach (var recentLink in excessNavigations)
            {
                userRecent.UserRecentActions.Remove(recentLink);
            }
            
            _logger.LogInformation("UserRecent now has {Count} recent navigations", userRecent.UserRecentActions.Count);

        }

        public async Task DeleteUserRecent(string userId)
        {
            using (var efCoreDatahubContext = _userTrackingContextFactory.CreateDbContext())
            {
                var userRecentActions = efCoreDatahubContext.UserRecent.Where(u => u.UserId == userId).FirstOrDefault();
                if (userRecentActions != null)
                {
                    efCoreDatahubContext.UserRecent.Remove(userRecentActions);
                    await efCoreDatahubContext.SaveChangesAsync();
                }
            }
        }

        public async Task<UserRecent> ReadRecentNavigations(string userId)
        {
            await using var efCoreDatahubContext = await _userTrackingContextFactory.CreateDbContextAsync();
            var userRecentActions = await efCoreDatahubContext.UserRecent
                .FirstOrDefaultAsync(u => u.UserId == userId);
            return userRecentActions;
        }

        public async Task RegisterNavigation(UserRecent recent)
        {
            using (var efCoreDatahubContext = _userTrackingContextFactory.CreateDbContext())
            {
                efCoreDatahubContext.UserRecent.Add(recent);
                await efCoreDatahubContext.SaveChangesAsync();
            }
        }
    }
}
