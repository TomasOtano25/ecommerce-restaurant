namespace EcommerceRestaurant.Web.Helpers
{
    using System.Threading.Tasks;
    using Data.Entities;
    using Microsoft.AspNetCore.Identity;

    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordUserAsync(User user, string oldPassword, string newPassword);

        Task<IdentityResult> AddToRoleUserAsync(User user, string roleName);

        object GenerateToken(User user);

    }
}
