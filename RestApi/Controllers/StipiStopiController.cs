using System;
using System.Collections.Generic;
using System.Linq;
using logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceInterfaces;
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
            // TODO Avoid repeated queries
            return stipiStopi.GetResources().Select(ssr => new ResourceInfo
            {
                ShortName = ssr.ShortName,
                Address = ssr.Address,
                Description = ssr.Description,
                IsAvailable = stipiStopi.IsFree(ssr),
                LockedBy = stipiStopi.GetLockedBy(ssr)?.UserName,
            });
        }

        [HttpPost("users")]
        public IEnumerable<SsUser> GetUsers(SsUser user)
        {
            return stipiStopi.GetUsers(user);
        }

        [HttpPost("register")]
        public SsUser NewUser(NewUserParameter newUser)
        {
            _logger.LogInformation($"Registering {newUser.User.UserName} with a password of length {newUser.User.Password.Length}");
            return stipiStopi.NewUser(newUser.User, newUser.Creator);
        }

        [HttpPost("resource")]
        public SsResource NewResource(NewResourceParameter newResource)
        {
            _logger.LogInformation($"New resource: {newResource.Resource.ShortName} @ {newResource.Resource.Address}");
            return stipiStopi.NewResource(newResource.Resource, newResource.Creator);
        }

        [HttpPost("resource/delete")]
        public bool DelResource(NewResourceParameter delResource)
        {
            _logger.LogInformation($"Deleting resource {delResource.Resource.ShortName}");
            return stipiStopi.DelResource(delResource.Resource.ShortName, delResource.Creator);
        }

        [HttpPost("resource/description")]
        public bool SetResourceDescription(SetResourceDescriptionParameter parameter)
        {
            _logger.LogInformation(
                $"Changing resource description of {parameter.Resource.ShortName} " +
                $"from {parameter.Resource.Description} to {parameter.Description}");
            return stipiStopi.UpdateResourceDescription(parameter.Resource, parameter.Description, parameter.User);
        }

        [HttpPost("lock")]
        public bool LockResource(LockParameter @lock)
        {
            _logger.LogInformation($"Trying to lock {@lock.ResourceName} for {@lock.User.UserName}");
            var resource = stipiStopi.GetResources()
                .SingleOrDefault(r => string.Equals(r.ShortName, @lock.ResourceName, StringComparison.InvariantCultureIgnoreCase));
            _logger.LogInformation($"Identified resource {resource.ShortName} @ {resource.Address}");
            return stipiStopi.LockResource(resource, new SsUser(@lock.User.UserName, @lock.User.Password));
        }

        [HttpPost("release")]
        public bool ReleaseResource(LockParameter @lock)
        {
            _logger.LogInformation($"Trying to release {@lock.ResourceName} for {@lock.User.UserName}");
            var resource = stipiStopi.GetResources()
                .SingleOrDefault(r => string.Equals(r.ShortName, @lock.ResourceName, StringComparison.InvariantCultureIgnoreCase));
            _logger.LogInformation($"Identified resource {resource.ShortName} @ {resource.Address}");
            return stipiStopi.ReleaseResource(resource, new SsUser(@lock.User.UserName, @lock.User.Password));
        }
    }

    public class NewResourceParameter
    {
        public SsResource Resource { get; set; }
        public SsUser Creator { get; set; }
    }

    public class SetResourceDescriptionParameter
    {
        public SsResource Resource { get; set; }
        public string Description { get; set; }
        public SsUser User { get; set; }
    }

    public class NewUserParameter
    {
        public SsUser User { get; set; }
        public SsUser Creator { get; set; }
    }

    public class LockParameter
    {
        public SsUser User { get; set; }
        public string ResourceName { get; set; }
    }

    public class ResourceInfo : SsResource
    {
        // TODO: should have an SsResource instance instead of deriving from it
        public bool IsAvailable { get; set; }
        public string LockedBy { get; set; }
    }
}
