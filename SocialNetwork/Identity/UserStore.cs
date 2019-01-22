using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SocialNetwork.Identity
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        public UserStore(UsersApi usersApi)
        {
            this.usersApi = usersApi;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var result = usersApi.AddUser(new Models.Users.UserModel { Name = user.UserName, Password = user.PasswordHash });
            if (result != null)
                return IdentityResult.Success;
            return IdentityResult.Failed();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = usersApi.FindUsersById(Convert.ToInt32(userId));
            if (result != null)
                return CreateUserFromModel(result);
            return null;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var result = usersApi.FindUsersByName(normalizedUserName);
            if (result != null)
                return CreateUserFromModel(result);
            return null;
        }

        private static User CreateUserFromModel(Models.Users.UserModel result)
        {
            return new User { UserName = result.Name, PasswordHash = result.Password, SecurityStamp = result.Password };
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return user.NormalizedUserName;
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return user.PasswordHash;
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            var result = usersApi.FindIdUserByName(user.UserName);
            if (result != -1)
                return result.ToString();
            else
                return null;
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return user.UserName;
        }

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user.PasswordHash != null)
                return true;
            return false;
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
        }

        private readonly UsersApi usersApi;

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
        }
        
        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
