using GymAdmin.Data;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;

namespace GymAdmin.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            //Database creation and migrations execution
            await _context.Database.EnsureCreatedAsync();

            //Seeds
            await CheckRolesAsync();

            //User seeds
            await CheckUsersAsync("1001", DocumentType.CC, "Dairo", "Mosquera", "dairo@yopmail.com", "318 284 6418", "image-dairo.jpg", UserType.Admin);
            await CheckUsersAsync("1002", DocumentType.CC, "Lindsey", "Morgan", "lindsey@yopmail.com", "311 456 1885", "image-lindsey.jpg", UserType.Admin);
            await CheckUsersAsync("1003", DocumentType.CC, "Marie", "Avgeropoulos", "marie@yopmail.com", "311 456 9696", "image-marie.jpg", UserType.User);
            await CheckUsersAsync("1004", DocumentType.PAP, "Curtis", "Jackson", "curtis@yopmail.com", "311 456 7589", "image-curtis.jpg", UserType.User);
            await CheckUsersAsync("1005", DocumentType.CE, "Brian", "Henry", "brian@yopmail.com", "311 456 5012", "image-brian.jpg", UserType.User);
            await CheckUsersAsync("1006", DocumentType.PAP, "Dwayne", "Johnson", "dwayne@yopmail.com", "311 456 7898", "image-rock.jpg", UserType.User);
            await CheckUsersAsync("1007", DocumentType.TI, "Brett", "Gray", "brett@yopmail.com", "311 456 5018", "image-brett.jpg", UserType.User);
            await CheckUsersAsync("1008", DocumentType.TI, "Millie", "Brown", "millie@yopmail.com", "311 456 4921", "image-millie.jpg", UserType.User);
        }

        private async Task<User> CheckUsersAsync(
            string document,
            DocumentType documentType,
            string firstName,
            string lastName,
            string email,
            string phone,
            string imageName,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{imageName}", "users");
                user = new User
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
                };
                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }
    }
}
