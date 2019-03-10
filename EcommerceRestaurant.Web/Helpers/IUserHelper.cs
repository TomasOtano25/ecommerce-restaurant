namespace EcommerceRestaurant.Web.Helpers
{
    using System.Threading.Tasks;
    using Data.Entities;
    using Microsoft.AspNetCore.Identity;

    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByNameAsync(string userName);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordUserAsync(User user, string oldPassword, string newPassword);

        Task<IdentityResult> AddUserToRoleUserAsync(User user, string roleName);

        object GenerateToken(User user);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<User> GetUserByIdAsync(string userId);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<bool> IsUserInRoleAsync(User user, string role);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);

        Task CheckRoleAsync(string roleName);
    }
}
