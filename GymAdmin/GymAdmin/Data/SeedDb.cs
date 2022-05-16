﻿using GymAdmin.Common;
using GymAdmin.Data.Entities;
using GymAdmin.Enums;
using GymAdmin.Helpers;
using Microsoft.EntityFrameworkCore;

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
            bool result = await _context.Database.EnsureCreatedAsync();
            if (result == true)
            {
                //Get Azure blob empty
                await _blobHelper.DeleteBlobsAsync("users");
            }

            //Roles seed
            await CheckRolesAsync();

            //User seeds
            await CheckUsersAsync("1001", DocumentType.CC, "Dairo", "Mosquera", "dairo@yopmail.com", "318 284 6418", "image-dairo.jpg", UserType.Admin);
            await CheckUsersAsync("1002", DocumentType.CC, "Lindsey", "Morgan", "lindsey@yopmail.com", "311 456 1885", "image-lindsey.jpg", UserType.Admin);
            await CheckUsersAsync("1003", DocumentType.CC, "Marie", "Avgeropoulos", "marie@yopmail.com", "311 456 9696", "image-marie.jpg", UserType.User);
            await CheckUsersAsync("1004", DocumentType.PAP, "Curtis", "Jackson", "curtis@yopmail.com", "311 456 7589", "image-curtis.jpg", UserType.User);
            await CheckUsersAsync("1005", DocumentType.PAP, "Dwayne", "Johnson", "dwayne@yopmail.com", "311 456 2498", "image-rock.jpg", UserType.User);
            await CheckUsersAsync("1006", DocumentType.TI, "Millie", "Brown", "millie@yopmail.com", "311 456 7892", "image-millie.jpg", UserType.User);
            await CheckUsersAsync("1007", DocumentType.TI, "Brett", "Gray", "brett@yopmail.com", "311 456 6498", "image-brett.jpg", UserType.User);
            await CheckUsersAsync("1008", DocumentType.CE, "Brian", "Henry", "brian@yopmail.com", "311 456 3794", "image-brian.jpg", UserType.User);
            await CheckUsersAsync("1009", DocumentType.CE, "Andy", "Allo", "andy@yopmail.com", "311 456 8002", "image-andy.jpg", UserType.User);
            await CheckUsersAsync("1010", DocumentType.CE, "Vanessa", "Hudgens", "vanessa@yopmail.com", "311 456 2841", "image-hudgens.jpg", UserType.User);
            await CheckUsersAsync("1011", DocumentType.CE, "Rihanna", "Fenty", "rihanna@yopmail.com", "311 456 7945", "image-riri.jpg", UserType.User);
            await CheckUsersAsync("1012", DocumentType.PAP, "Lamar", "Hill", "lamar@yopmail.com", "311 456 3628", "image-lamar.jpg", UserType.User);
            await CheckUsersAsync("1013", DocumentType.TI, "Peyton", "List", "peyton@yopmail.com", "311 456 4124", "image-peyton.jpg", UserType.User);

            //Services seeds
            await CheckServicesAsync();
            await CheckProfessionalsAsync();
            await CheckServicesAccessesAsync();

            //Plan Seeds

            await CheckPlansAsync();
            await CheckPlanInscription();
        }

        private async Task CheckProfessionalsAsync()
        {
            if (!_context.Professionals.Any())
            {
                Professional professional = new()
                {
                    User = await _userHelper.GetUserAsync("dwayne@yopmail.com"),
                    ProfessionalType = ProfessionalType.Instructor,
                    Service = await _context.Services.FindAsync(1),
                    ProfessionalSchedules = new List<ProfessionalSchedule>(),
                };
                professional.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Monday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Wednesday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Friday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Tuesday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Thursday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Saturday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    }
                };
                _context.Add(professional);
                await _context.SaveChangesAsync();

                Professional professional2 = new()
                {
                    User = await _userHelper.GetUserAsync("lindsey@yopmail.com"),
                    ProfessionalType = ProfessionalType.Physiotherapist,
                    Service = await _context.Services.FindAsync(2),
                    ProfessionalSchedules = new List<ProfessionalSchedule>()
                };
                professional2.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional2,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Monday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional2,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Wednesday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional2,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Friday,
                            StartHour = new TimeSpan(7, 0, 0),
                            FinishHour = new TimeSpan(12, 0, 0),
                        }
                    }
                };
                _context.Add(professional2);
                await _context.SaveChangesAsync();

                Professional professional3 = new()
                {
                    User = await _userHelper.GetUserAsync("andy@yopmail.com"),
                    ProfessionalType = ProfessionalType.Nutritionist,
                    Service = await _context.Services.FindAsync(3),
                    ProfessionalSchedules = new List<ProfessionalSchedule>()
                };
                professional3.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Tuesday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Thursday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = new Schedule()
                        {
                            Day = DayOfWeek.Saturday,
                            StartHour = new TimeSpan(14, 0, 0),
                            FinishHour = new TimeSpan(19, 0, 0),
                        }
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = await _context.Schedules.FindAsync(7),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = await _context.Schedules.FindAsync(8),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional3,
                        Schedule = await _context.Schedules.FindAsync(9),
                    },
                };
                _context.Add(professional3);
                await _context.SaveChangesAsync();

                Professional professional4 = new()
                {
                    User = await _userHelper.GetUserAsync("curtis@yopmail.com"),
                    ProfessionalType = ProfessionalType.Instructor,
                    Service = await _context.Services.FindAsync(1),
                    ProfessionalSchedules = new List<ProfessionalSchedule>(),
                };
                professional4.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(7),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(8),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(9),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(10),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(11),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional4,
                        Schedule = await _context.Schedules.FindAsync(12),
                    },
                };
                _context.Add(professional4);
                await _context.SaveChangesAsync();

                Professional professional5 = new()
                {
                    User = await _userHelper.GetUserAsync("marie@yopmail.com"),
                    ProfessionalType = ProfessionalType.Physiotherapist,
                    Service = await _context.Services.FindAsync(2),
                    ProfessionalSchedules = new List<ProfessionalSchedule>()
                };
                professional5.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional5,
                        Schedule = await _context.Schedules.FindAsync(4),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional5,
                        Schedule = await _context.Schedules.FindAsync(5),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional5,
                        Schedule = await _context.Schedules.FindAsync(6),
                    }
                };
                _context.Add(professional5);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckServicesAsync()
        {
            if (!_context.Services.Any())
            {
                _context.Add(new Service
                {
                    Name = "Instrucción",
                    Price = 140000,
                });
                _context.Add(new Service
                {
                    Name = "Fisioterapia",
                    Price = 120000,
                });
                _context.Add(new Service
                {
                    Name = "Nutricionismo",
                    Price = 90000,
                });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckServicesAccessesAsync()
        {
            if (!_context.ServiceAccesses.Any())
            {
                Service instru = await _context.Services.FindAsync(1);
                Service physio = await _context.Services.FindAsync(2);
                Service nutri = await _context.Services.FindAsync(3);

                List<Service> ServicesList = new() { instru, physio, nutri };

                List<string> EmailsList = new()
                {
                    "dairo@yopmail.com",
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                };

                int randomHour = 13;
                int randomDay = 1;
                bool notExist = false;

                Enums.ServiceStatus status = Enums.ServiceStatus.Pending;
                foreach (string email in EmailsList)
                {
                    foreach (Service service in ServicesList)
                    {
                        notExist = false;
                        while (notExist == false)
                        {
                            randomHour = 13;
                            while (randomHour == 12 || randomHour == 13)
                            {
                                randomHour = new Random().Next(7, 18);
                            }
                            randomDay = new Random().Next(1, 12);

                            DateTime searchDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour);

                            var serviceAccesses = await _context.ServiceAccesses
                                .Where(sa =>
                                    sa.Service.Id == service.Id &&
                                    sa.AccessDate == searchDate &&
                                    sa.ServiceStatus == status)
                                .ToListAsync();

                            if (serviceAccesses.Count == 0 || serviceAccesses == null)
                            {
                                var professionals = await _context.Professionals
                                    .Include(p => p.Service)
                                    .Include(p => p.ProfessionalSchedules)
                                    .ThenInclude(ps => ps.Schedule)
                                    .Where(p => p.Service.Id == service.Id)
                                    .ToListAsync();

                                bool result = professionals.Any(p => p.ProfessionalSchedules.Any(ps =>
                                    ps.Schedule.Day == searchDate.DayOfWeek &&
                                    ps.Schedule.StartHour.Ticks <= searchDate.TimeOfDay.Ticks &&
                                    ps.Schedule.FinishHour.Ticks > searchDate.TimeOfDay.Ticks));

                                if (result)
                                {
                                    _context.Add(new ServiceAccess
                                    {
                                        User = await _userHelper.GetUserAsync(email),
                                        Service = service,
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue("Regular"),
                                        TotalPrice = service.Price - (service.Price * (decimal)DiscountValues.GetDiscountValue("Regular")),
                                    });
                                    await _context.SaveChangesAsync();
                                    notExist = true;
                                }
                            }
                        }
                    }
                }

                status = Enums.ServiceStatus.Taken;
                foreach (string email in EmailsList)
                {
                    foreach (Service service in ServicesList)
                    {
                        notExist = false;
                        while (notExist == false)
                        {
                            randomHour = 13;
                            while (randomHour == 12 || randomHour == 13)
                            {
                                randomHour = new Random().Next(7, 18);
                            }
                            randomDay = new Random().Next(-12, -1);

                            DateTime searchDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour);

                            var serviceAccesses = await _context.ServiceAccesses
                                .Where(sa =>
                                    sa.Service.Id == service.Id &&
                                    sa.AccessDate == searchDate &&
                                    sa.ServiceStatus == status)
                                .ToListAsync();

                            if (serviceAccesses.Count == 0 || serviceAccesses == null)
                            {
                                var professionals = await _context.Professionals
                                    .Include(p => p.Service)
                                    .Include(p => p.ProfessionalSchedules)
                                    .ThenInclude(ps => ps.Schedule)
                                    .Where(p => p.Service.Id == service.Id)
                                    .ToListAsync();

                                bool result = professionals.Any(p => p.ProfessionalSchedules.Any(ps =>
                                    ps.Schedule.Day == searchDate.DayOfWeek &&
                                    ps.Schedule.StartHour.Ticks <= searchDate.TimeOfDay.Ticks &&
                                    ps.Schedule.FinishHour.Ticks > searchDate.TimeOfDay.Ticks));

                                if (result)
                                {
                                    _context.Add(new ServiceAccess
                                    {
                                        User = await _userHelper.GetUserAsync(email),
                                        Service = service,
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue("Regular"),
                                        TotalPrice = service.Price - (service.Price * (decimal)DiscountValues.GetDiscountValue("Regular")),
                                    });
                                    await _context.SaveChangesAsync();
                                    notExist = true;
                                }
                            }
                        }
                    }
                }
            }
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

        private async Task CheckPlansAsync()
        {
            if (!_context.Plans.Any())
            {
                _context.Add(new Plan
                {
                    Name = "TicketHolder",
                    PlanType = PlanType.TicketHolder,
                    Price = 5900

                });

                _context.Add(new Plan
                {
                    Name = "Simple",
                    PlanType = PlanType.Simple,
                    Price = 49000
                });


                _context.Add(new Plan
                {
                    Name = "Black",
                    PlanType = PlanType.Black,
                    Price = 69900
                });

                await _context.SaveChangesAsync();
            }
        }//There are just 3 types of plans, ticketholder,simple and black


        private async Task CheckPlanInscription() 
        {
            //i will create a plan inscription for every type of plan to make the methods
            if (!_context.PlanInscriptions.Any())     
            {
                User user = await _userHelper.GetUserAsync("lindsey@yopmail.com");
                User user2 = await _userHelper.GetUserAsync("marie@yopmail.com");
                User user3 = await _userHelper.GetUserAsync("curtis@yopmail.com");
                Plan plan = await _context.Plans.FindAsync(1);
                Plan plan2 = await _context.Plans.FindAsync(2);
                Plan plan3 = await _context.Plans.FindAsync(3);
                PlanInscription pl1 = new PlanInscription 
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user,
                    Plan = plan, // the user will have the plan ticketholder
                    Duration = 5,
                    ExpirationDate = DateTime.Today.AddDays(5),
                    PlanStatus = PlanStatus.Paid,
                    TotalPrice = plan.Price * 5,
                    RemainingDays = 5,
                    Discount = DiscountValues.GetDiscountValue("TicketHolder")
                };
                plan.PlansInscriptions = new List<PlanInscription>();
                plan.PlansInscriptions.Add(pl1);
                _context.Add(pl1);
                user.PlanInscriptions = new List<PlanInscription>();
                user.PlanInscriptions.Add(pl1);


                PlanInscription pl2 = new PlanInscription
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user2,
                    Plan = plan2, // the user will have the plan ticketholder
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Paid,
                    TotalPrice = plan2.Price,
                    RemainingDays = 30,
                    Discount = DiscountValues.GetDiscountValue("Regular")
                };
                plan2.PlansInscriptions = new List<PlanInscription>();
                plan2.PlansInscriptions.Add(pl2);
                _context.Add(pl2);
                user2.PlanInscriptions = new List<PlanInscription>();
                user2.PlanInscriptions.Add(pl2);

                PlanInscription pl3 = new PlanInscription
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user3,
                    Plan = plan3, // the user will have the plan ticketholder
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Paid,
                    TotalPrice = plan3.Price,
                    RemainingDays = 30,
                    Discount = DiscountValues.GetDiscountValue("Black")

                };
                plan3.PlansInscriptions = new List<PlanInscription>();
                plan3.PlansInscriptions.Add(pl3);
                _context.Add(pl3);
                user3.PlanInscriptions = new List<PlanInscription>();
                user3.PlanInscriptions.Add(pl3);

                _context.Update(user);
                _context.Update(user2);
                _context.Update(user3);
                _context.Update(plan);
                _context.Update(plan2);
                _context.Update(plan3);
                await _context.SaveChangesAsync();
            } 
        }
    }
}
