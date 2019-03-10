﻿namespace EcommerceRestaurant.Web.Helpers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Data.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> userManager;

        private readonly IOptions<TokenConfig> tokenConfig;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserHelper(UserManager<User> userManager, IOptions<TokenConfig> tokenConfig, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.tokenConfig = tokenConfig;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await this.userManager.CreateAsync(user, password);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await this.userManager.FindByEmailAsync(email);
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            return await this.userManager.FindByNameAsync(userName);
        }


        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await this.userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordUserAsync(User user, string oldPassword, string newPassword)
        {
            return await this.userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<IdentityResult> AddUserToRoleUserAsync(User user, string roleName)
        {
            return await this.userManager.AddToRoleAsync(user, roleName);
        }

        public object GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.tokenConfig.Value.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                this.tokenConfig.Value.Issuer,
                this.tokenConfig.Value.Audience,
                claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials
                );

            return new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await this.userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await this.userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string role)
        {
            return await this.userManager.IsInRoleAsync(user, role);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await this.userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await this.userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await this.roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await this.roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

    }
}
