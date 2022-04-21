using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
namespace GymAdmin.Data
{
    public class SeedDbProfessional
    {
        private readonly DataContext _context;
        private readonly IUserHelperProfessional _userProfessionalHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDbProfessional(DataContext context, IUserHelperProfessional userProfessionalHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userProfessionalHelper = userProfessionalHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            //Database creation and migrations execution
            await _context.Database.EnsureCreatedAsync();

            //Seeds
            await CheckRolesAsync();

            //User seeds
            await CheckUsersAsync("1001", DocumentType.CC, "Felipe", "Martinez", "felipe@yopmail.com", "318 284 6418", "image-dairo.jpg", UserType.Admin);
           
        }

        private async Task<UserProfessional> CheckUsersAsync(
            string document,
            DocumentType documentType,
            string firstName,
            string lastName,
            string email,
            string phone,
            string imageName,
            UserType userType)
        {
            UserProfessional user = await _userProfessionalHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{imageName}", "users");
                user = new UserProfessional
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Document = document,
                    DocumentType = documentType,
                    ImageId = imageId,
                    UserType = userType,
                    //ServiceId = 
                };
                await _userProfessionalHelper.AddUserAsync(user, "123456");
                await _userProfessionalHelper.AddUserToRoleAsync(user, userType.ToString());
                string token = await _userProfessionalHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userProfessionalHelper.ConfirmEmailAsync(user, token);
            }
            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userProfessionalHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userProfessionalHelper.CheckRoleAsync(UserType.User.ToString());
        }
    }
}
