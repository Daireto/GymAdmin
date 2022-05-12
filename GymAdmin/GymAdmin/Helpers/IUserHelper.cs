using GymAdmin.Data.Entities;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;

namespace GymAdmin.Helpers
{
    public interface IUserHelper
    {
        Task<IdentityResult> AddUserAsync(User user, string password);
        Task<User> AddUserAsync(AddUserViewModel model, Guid imageId);
        Task<User> AddUserAsync(AddProfessionalViewModel model, Guid imageId);
        Task AddUserToRoleAsync(User user, string roleName);
        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);
        Task CheckRoleAsync(string roleName);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<User> GetUserAsync(string email);
        Task<User> GetUserAsync(Guid userId);
        Task<User> GetUserAsync(AddUserViewModel model);
        Task<User> GetUserAsync(AddProfessionalViewModel model);
        Task<bool> IsUserInRoleAsync(User user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
        Task<IdentityResult> UpdateUserAsync(User user);
    }
}
