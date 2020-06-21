using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using ServiceInterfaces;
using ServiceInterfaces.Dto;
using ServiceInterfaces.Exceptions;

namespace logic
{
    public class StipiStopi
    {
        private ISsRepository SsRepository { get; }

        public StipiStopi(ISsRepository ssRepository)
        {
            SsRepository = ssRepository;
        }

        public void Populate()
        {
            SsRepository.Transaction(() =>
            {
                var testuser = new SsUserSecret(new SsUser("test", "test", UserRole.Admin));
                SsRepository.SaveUser(testuser);
                var ncu139 = new SsResource("ncu1", "10.10.148.8");
                SsRepository.SaveResource(ncu139);
                var ncu140 = new SsResource("ncu2", "10.10.148.9");
                SsRepository.SaveResource(ncu140);
                SsRepository.Lock(ncu140, testuser.UserName, "");
            });
        }

        public void EnsureAdminExists()
        {
            var userCount = -1;
            SsRepository.Transaction(() => userCount = SsRepository.GetUsers().Count());
            if (userCount == 0)
                Populate();
        }

        public List<SsResource> GetResources()
        {
            List<SsResource> resources = null;
            SsRepository.Transaction(() =>
                resources = SsRepository.GetResources());
            return resources;
        }

        public SsResource UpdateResourceDescription(string shortName, string oldDescription, string newDescription, SsUser user)
        {
            return SsRepository.Transaction<SsResource>(() =>
            {
                Authenticated(user);
                var dbResource = GetExistingResource(shortName);
                if (dbResource.Description == oldDescription)
                {
                    dbResource.Description = newDescription;
                    SsRepository.SaveResource(dbResource);
                }
                return dbResource;
            }
            );
        }

        public IEnumerable<SsUser> GetUsers(SsUser user)
        {
            IEnumerable<SsUser> users = Array.Empty<SsUser>();
            SsRepository.Transaction(() =>
            {
                Authenticated(user);
                users = SsRepository.GetUsers();
            });
            return users;
        }

        public SsResource NewResource(SsResource ssResource, SsUser creator)
        {
            Contract.Requires(ssResource != null);
            SsRepository.Transaction(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsRepository.GetResource(ssResource.ShortName) != null)
                    throw new ResourceAlreadyExistsException(ssResource.ShortName);

                SsRepository.SaveResource(ssResource);
            });
            return ssResource;
        }

        public bool DelResource(string shortName, SsUser creator)
        {
            Contract.Requires(shortName != null);
            return SsRepository.Transaction<bool>(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                return SsRepository.DeleteResource(shortName);
            });
        }

        public SsUser NewUser(SsUser newUser, SsUser creator)
        {
            SsRepository.Transaction(() =>
            {
                RequiresAdmin(creator);

                if (SsRepository.GetUser(newUser.UserName) != null)
                    throw new UserAlreadyExistsException(newUser.UserName);

                SsRepository.SaveUser(new SsUserSecret(newUser));
            });
            return newUser;
        }

        public bool DbImport(SsUser admin, string content)
        {
            return SsRepository.Transaction<bool>(() =>
            {
                RequiresAdmin(admin);
                return SsRepository.DbImport(content);
            });
        }

        public string DbExport(SsUser admin)
        {
            return SsRepository.Transaction<string>(() =>
            {
                RequiresAdmin(admin);
                return SsRepository.DbExport();
            });
        }

        private void RequiresAdmin(SsUser admin)
        {
                if (Authenticated(admin)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(admin.UserName);
        }

        public bool DelUser(string userName, SsUser creator)
        {
            Contract.Requires(userName != null);
            return SsRepository.Transaction<bool>(() =>
            {
                if (Authenticated(creator)?.Role != UserRole.Admin)
                    throw new InsufficientRoleException(creator.UserName);

                if (SsUserSecret.NormalizeUserName(userName) == creator.UserName)
                    return false;

                return SsRepository.DeleteUser(userName);
            });
        }

        private SsUserSecret Authenticated(SsUser user)
        {
            var userSecret = SsRepository.GetUser(user.UserName);
            if (userSecret == null)
                throw new UserDoesNotExistException(user.UserName);

            if (!userSecret.IsValid(user))
                throw new InvalidPasswordException(user.UserName);

            return userSecret;
        }

        public bool LockResource(string shortName, SsUser user, string comment = "")
        {
            bool success = false;
            SsRepository.Transaction(() =>
            {
                var authenticated = Authenticated(user);
                var dbResource = GetExistingResource(shortName);

                var lockedBy = SsRepository.GetLocking(dbResource)?.LockedBy;
                if (lockedBy != null)
                {
                    success = false;
                }
                else
                {
                    SsRepository.Lock(dbResource, authenticated.UserName, comment);
                    success = true;
                }
            });
            return success;
        }

        public bool ReleaseResource(string shortName, SsUser user)
        {
            bool success = false;
            SsRepository.Transaction(() =>
            {
                var authenticated = Authenticated(user);
                var dbResource = GetExistingResource(shortName);

                var lockedBy = SsRepository.GetLocking(dbResource)?.LockedBy;
                if (lockedBy != null && lockedBy.UserName.Equals(user.UserName, StringComparison.InvariantCultureIgnoreCase))
                {
                    SsRepository.Release(dbResource);
                    success = true;
                }
                else
                {
                    success = false;
                }
            });
            return success;
        }

        public bool IsLocked(SsResource res)
        {
            // TODO: this operation should require a valid user
            return SsRepository.Transaction<bool>(() =>
                SsRepository.GetLocking(res) != null
            );
        }

        public bool IsFree(SsResource res)
        {
            return !IsLocked(res);
        }

        private SsResource GetExistingResource(string shortName)
        {
            SsResource dbResource = SsRepository.GetResource(shortName);
            if (dbResource == null)
                throw new ResourceDoesNotExistException(shortName);
            return dbResource;
        }
    }
}
