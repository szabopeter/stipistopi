using System.Collections.Generic;
using System.Linq;
using logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceInterfaces.Dto;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StipiStopiController : ControllerBase
    {
        private readonly ILogger<StipiStopiController> _logger;
        private readonly StipiStopi stipiStopi;

        public StipiStopiController(ILogger<StipiStopiController> logger, StipiStopi stipiStopi)
        {
            _logger = logger;
            this.stipiStopi = stipiStopi;
            stipiStopi.EnsureAdminExists();
        }

        [HttpGet("resources")]
        public IEnumerable<ResourceInfo> GetResources()
        {
            // TODO: this should just return the LockingInfos
            return stipiStopi.GetLockingInfos().Select(locking =>
            new ResourceInfo
            {
                Resource = locking.Resource,
                IsAvailable = locking.LockedBy == null,
                LockedBy = locking.LockedBy?.UserName,
                Locking = locking,
            });
        }

        [HttpPost("users")]
        public IEnumerable<SsUser> GetUsers(SsUser user)
        {
            return stipiStopi.GetUsers(user);
        }

        [HttpPost("register")]
        public SsUser NewUser(UserAndUserParameter newUser)
        {
            _logger.LogInformation($"Registering {newUser.User.UserName} with a password of length {newUser.User.Password.Length}");
            return stipiStopi.NewUser(newUser.User, newUser.Creator);
        }

        [HttpPost("user/delete")]
        public bool DelUser(UserAndUserParameter delUser)
        {
            _logger.LogInformation($"Deleting resource {delUser.User.UserName}");
            return stipiStopi.DelUser(delUser.User.UserName, delUser.Creator);
        }

        [HttpPost("resource")]
        public SsResource NewResource(ResourceAndUserParameter newResource)
        {
            _logger.LogInformation($"New resource: {newResource.Resource.ShortName} @ {newResource.Resource.Address}");
            return stipiStopi.NewResource(newResource.Resource, newResource.Creator);
        }

        [HttpPost("resource/delete")]
        public bool DelResource(ResourceAndUserParameter delResource)
        {
            _logger.LogInformation($"Deleting resource {delResource.Resource.ShortName}");
            return stipiStopi.DelResource(delResource.Resource.ShortName, delResource.Creator);
        }

        [HttpPost("resource/description")]
        public SsResource SetResourceDescription(ResourceDescriptionParameter parameter)
        {
            _logger.LogInformation(
                $"Changing resource description of {parameter.ResourceName} " +
                $"from {parameter.OldDescription} to {parameter.NewDescription}");
            return stipiStopi.UpdateResourceDescription(
                parameter.ResourceName, parameter.OldDescription, parameter.NewDescription, parameter.User);
        }

        [HttpPost("lock")]
        public bool LockResource(LockParameter @lock)
        {
            _logger.LogInformation($"Trying to lock {@lock.ResourceName} for {@lock.User.UserName}");
            return stipiStopi.LockResource(@lock.ResourceName, new SsUser(@lock.User.UserName, @lock.User.Password));
        }

        [HttpPost("release")]
        public bool ReleaseResource(LockParameter @lock)
        {
            _logger.LogInformation($"Trying to release {@lock.ResourceName} for {@lock.User.UserName}");
            return stipiStopi.ReleaseResource(@lock.ResourceName, new SsUser(@lock.User.UserName, @lock.User.Password));
        }
    }
}
