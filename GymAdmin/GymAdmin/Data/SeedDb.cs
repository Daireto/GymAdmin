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

            //Roles seed
            await CheckRolesAsync();

            //User seeds
            await CheckUsersAsync("1001", DocumentType.CC, "Dairo", "Mosquera", "dairo@yopmail.com", "318 284 6418", "image-dairo.jpg", UserType.Admin);
            await CheckUsersAsync("1002", DocumentType.CC, "Lindsey", "Morgan", "lindsey@yopmail.com", "311 456 1885", "image-lindsey.jpg", UserType.Admin);
            await CheckUsersAsync("1003", DocumentType.CC, "Marie", "Avgeropoulos", "marie@yopmail.com", "311 456 9696", "image-marie.jpg", UserType.User);
            await CheckUsersAsync("1004", DocumentType.PAP, "Curtis", "Jackson", "curtis@yopmail.com", "311 456 7589", "image-curtis.jpg", UserType.User);
            await CheckUsersAsync("1005", DocumentType.PAP, "Dwayne", "Johnson", "dwayne@yopmail.com", "311 456 7898", "image-rock.jpg", UserType.User);

            //Services seeds
            await CheckSchedulesAsync();
            await CheckProfessionalsAsync();
            await CheckServicesAsync();
        }

        private async Task CheckSchedulesAsync()
        {
            if (!_context.Schedules.Any())
            {
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Monday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Tuesday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Wednesday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Thursday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Friday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
                _context.Add(new Schedule
                {
                    Day = DayOfWeek.Saturday,
                    StartHour = "07:00",
                    FinishHour = "12:00",
                });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckProfessionalsAsync()
        {
            if (!_context.Professionals.Any())
            {
                _context.Add(new Professional
                {
                    User = await _userHelper.GetUserAsync("lindsey@yopmail.com"),
                    ProfessionalType = ProfessionalType.Physiotherapist,
                    Schedule = await _context.Schedules.FindAsync(1)
                });
                _context.Add(new Professional
                {
                    User = await _userHelper.GetUserAsync("marie@yopmail.com"),
                    ProfessionalType = ProfessionalType.Nutritionist,
                    Schedule = await _context.Schedules.FindAsync(2)
                });
                _context.Add(new Professional
                {
                    User = await _userHelper.GetUserAsync("dwayne@yopmail.com"),
                    ProfessionalType = ProfessionalType.Instructor,
                    Schedule = await _context.Schedules.FindAsync(3)
                });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckServicesAsync()
        {
            if (!_context.Services.Any())
            {
                _context.Add(new Service
                {
                    Name = "Fortalecimiento muscular",
                    Price = 120000,
                    Professional = await _context.Professionals.FindAsync(1)
                });
                _context.Add(new Service
                {
                    Name = "Evaluación física",
                    Price = 90000,
                    Professional = await _context.Professionals.FindAsync(2)
                });
                _context.Add(new Service
                {
                    Name = "Instrucción",
                    Price = 140000,
                    Professional = await _context.Professionals.FindAsync(3)
                });
            }
            await _context.SaveChangesAsync();
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
