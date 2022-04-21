using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymAdmin.Helpers
{
    public class UserProfessionalHelper : IUserHelperProfessional
    {
        private readonly DataContext _context;
        private readonly UserManager<UserProfessional> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<UserProfessional> _signInManager;

        public UserProfessionalHelper(DataContext context, UserManager<UserProfessional> userManager, RoleManager<IdentityRole> roleManager, SignInManager<UserProfessional> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        //This method is to register users from SeedDb
        public async Task<IdentityResult> AddUserAsync(UserProfessional user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<UserProfessional> AddUserAsync(AddUserViewModelProfessional model, Guid imageId)
        {
            UserProfessional user = new()
            {
                Email = model.Username,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Document = model.Document,
                DocumentType = model.DocumentType,
                ImageId = imageId,
                UserName = model.Username,
                UserType = model.UserType,
                ServiceId = model.Service.Id
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return null;
            }

            UserProfessional user2 = await GetUserAsync(model.Username);
            await AddUserToRoleAsync(user2, user.UserType.ToString());
            return user2;
        }

        public async Task AddUserToRoleAsync(UserProfessional user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(UserProfessional user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool result = await _roleManager.RoleExistsAsync(roleName);
            if (!result)
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
        }

        public async Task<IdentityResult> ConfirmEmailAsync(UserProfessional user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(UserProfessional user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(UserProfessional user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<UserProfessional> GetUserAsync(string email)
        {
            return await _context.UsersProfessional
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserProfessional> GetUserAsync(Guid userId)
        {
            return await _context.UsersProfessional
                .FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }
        public async Task<User> GetUserAsync(AddUserViewModel model)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Document == model.Document && u.DocumentType == model.DocumentType);
        }

        public Task<UserProfessional> GetUserAsync(AddUserViewModelProfessional model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsUserInRoleAsync(UserProfessional user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false); //With try limits
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(UserProfessional user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserProfessional user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}
