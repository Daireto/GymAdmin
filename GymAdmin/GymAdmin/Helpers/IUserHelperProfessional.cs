using GymAdmin.Data.Entities;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;

namespace GymAdmin.Helpers
{
    public interface IUserHelperProfessional
    {
        Task<IdentityResult> AddUserAsync(UserProfessional user, string password);
        Task<UserProfessional> AddUserAsync(AddUserViewModelProfessional model, Guid imageId);
        Task AddUserToRoleAsync(UserProfessional user, string roleName);
        Task<IdentityResult> ChangePasswordAsync(UserProfessional user, string oldPassword, string newPassword);
        Task CheckRoleAsync(string roleName);
        Task<IdentityResult> ConfirmEmailAsync(UserProfessional user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(UserProfessional user);
        Task<string> GeneratePasswordResetTokenAsync(UserProfessional user);
        Task<UserProfessional> GetUserAsync(string email);
        Task<UserProfessional> GetUserAsync(Guid userId);
        Task<UserProfessional> GetUserAsync(AddUserViewModelProfessional model);
        Task<bool> IsUserInRoleAsync(UserProfessional user, string roleName);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<IdentityResult> ResetPasswordAsync(UserProfessional user, string token, string password);
        Task<IdentityResult> UpdateUserAsync(UserProfessional user);
    }

}