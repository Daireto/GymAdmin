using GymAdmin.Common;
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
                await _blobHelper.DeleteBlobsAsync("events");
            }

            //Roles seed
            await CheckRolesAsync();

            //User seeds
            await CheckUsersAsync("1001", DocumentType.CC, "Lindsey", "Morgan", "lindsey@yopmail.com", "311 456 1885", "image-lindsey.jpg", UserType.Admin);
            await CheckUsersAsync("1002", DocumentType.CC, "Marie", "Avgeropoulos", "marie@yopmail.com", "311 456 9696", "image-marie.jpg", UserType.Admin);
            await CheckUsersAsync("1003", DocumentType.CC, "Victoria", "Justice", "victoria@yopmail.com", "311 456 6418", "image-victoria.jpg", UserType.User);
            await CheckUsersAsync("1004", DocumentType.CE, "Curtis", "Jackson", "curtis@yopmail.com", "311 456 7589", "image-curtis.jpg", UserType.User);
            await CheckUsersAsync("1005", DocumentType.CE, "Dwayne", "Johnson", "dwayne@yopmail.com", "311 456 2498", "image-rock.jpg", UserType.User);
            await CheckUsersAsync("1006", DocumentType.TI, "Millie", "Brown", "millie@yopmail.com", "311 456 7892", "image-millie.jpg", UserType.User);
            await CheckUsersAsync("1007", DocumentType.TI, "Brett", "Gray", "brett@yopmail.com", "311 456 6498", "image-brett.jpg", UserType.User);
            await CheckUsersAsync("1008", DocumentType.PAP, "Brian", "Henry", "brian@yopmail.com", "311 456 3794", "image-brian.jpg", UserType.User);
            await CheckUsersAsync("1009", DocumentType.CE, "Andy", "Allo", "andy@yopmail.com", "311 456 8002", "image-andy.jpg", UserType.User);
            await CheckUsersAsync("1010", DocumentType.CE, "Vanessa", "Hudgens", "vanessa@yopmail.com", "311 456 2841", "image-hudgens.jpg", UserType.User);
            await CheckUsersAsync("1011", DocumentType.PAP, "Rihanna", "Fenty", "rihanna@yopmail.com", "311 456 7945", "image-riri.jpg", UserType.User);
            await CheckUsersAsync("1012", DocumentType.PAP, "Lamar", "Hill", "lamar@yopmail.com", "311 456 3628", "image-lamar.jpg", UserType.User);
            await CheckUsersAsync("1013", DocumentType.TI, "Peyton", "List", "peyton@yopmail.com", "311 456 4124", "image-peyton.jpg", UserType.User);
            await CheckUsersAsync("1014", DocumentType.CC, "Maia", "Mitchell", "maia@yopmail.com", "311 456 3009", "image-maia.jpg", UserType.User);
            await CheckUsersAsync("1015", DocumentType.TI, "Madison", "Pettis", "madison@yopmail.com", "311 456 0071", "image-madison.jpg", UserType.User);
            await CheckUsersAsync("1016", DocumentType.CE, "Aliya", "Mustafina", "aliya@yopmail.com", "311 456 6271", "image-aliya.jpg", UserType.User);
            await CheckUsersAsync("1017", DocumentType.CC, "Mariana", "Pajón", "mariana@yopmail.com", "311 456 6272", "image-mariana.jpg", UserType.User);
            await CheckUsersAsync("1018", DocumentType.CC, "Greeicy", "Rendón", "greeicy@yopmail.com", "311 456 6273", "image-greeicy.jpg", UserType.User);
            await CheckUsersAsync("1019", DocumentType.CE, "Chris", "Brown", "chris@yopmail.com", "311 456 6274", "image-chris.jpg", UserType.User);
            await CheckUsersAsync("1020", DocumentType.CC, "Tamara", "Rojo", "tamara@yopmail.com", "311 456 6275", "image-tamara.jpg", UserType.User);
            await CheckUsersAsync("1021", DocumentType.CE, "Jackie", "Chan", "jackie@yopmail.com", "311 456 6276", "image-jackie.jpg", UserType.User);
            await CheckUsersAsync("1022", DocumentType.CE, "Alicia", "Keys", "alicia@yopmail.com", "311 456 6277", "image-alicia.jpg", UserType.User);

            //Plan Seeds
            await CheckPlansAsync();
            await CheckPlanInscriptionAsync();

            //Services seeds
            await CheckServicesAsync();
            await CheckProfessionalsAsync();
            await CheckServicesAccessesAsync();

            //Attendance seed
            await CheckAttendancesAsync();

            //Events seeds
            await CheckDirectorsAsync();
            await CheckEventInscription();
        }

        private async Task CheckDirectorsAsync()
        {
            if (!_context.Directors.Any())
            {
                Director director1 = new()
                {
                    User = await _userHelper.GetUserAsync("aliya@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director2 = new()
                {
                    User = await _userHelper.GetUserAsync("mariana@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director3 = new()
                {
                    User = await _userHelper.GetUserAsync("tamara@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director4 = new()
                {
                    User = await _userHelper.GetUserAsync("chris@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director5 = new()
                {
                    User = await _userHelper.GetUserAsync("greeicy@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director6 = new()
                {
                    User = await _userHelper.GetUserAsync("jackie@yopmail.com"),
                    Events = new List<Event>(),
                };
                Director director7 = new()
                {
                    User = await _userHelper.GetUserAsync("alicia@yopmail.com"),
                    Events = new List<Event>(),
                };
                director1.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Monday,
                        StartHour = new TimeSpan(7, 0, 0),
                        FinishHour = new TimeSpan(9, 0, 0),
                        Name = "Body Balance",
                        EventType = EventType.Balance,
                        Director = director1,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-women-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-women1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-women2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-women3.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Wednesday,
                        StartHour = new TimeSpan(7, 0, 0),
                        FinishHour = new TimeSpan(9, 0, 0),
                        Name = "Gimnasia",
                        EventType = EventType.Balance,
                        Director = director1,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-gymnastics-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-gymnastics1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-gymnastics2.jpg", "events"),
                            },
                        }
                    },
                };
                director2.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Tuesday,
                        StartHour = new TimeSpan(8, 0, 0),
                        FinishHour = new TimeSpan(10, 0, 0),
                        Name = "Cardio",
                        EventType = EventType.Crossfit,
                        Director = director2,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-cardio-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-cardio1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-cardio2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-cardio3.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Thursday,
                        StartHour = new TimeSpan(8, 0, 0),
                        FinishHour = new TimeSpan(10, 0, 0),
                        Name = "Salto alto",
                        EventType = EventType.Crossfit,
                        Director = director2,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-jump-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-jump1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-jump2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-jump3.jpg", "events"),
                            },
                        }
                    },
                };
                director3.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Monday,
                        StartHour = new TimeSpan(7, 0, 0),
                        FinishHour = new TimeSpan(9, 0, 0),
                        Name = "Ballet",
                        EventType = EventType.Dance,
                        Director = director3,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-ballet-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-ballet1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-ballet2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-ballet3.jpg", "events"),
                            },
                        }
                    },
                };
                director4.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Tuesday,
                        StartHour = new TimeSpan(16, 0, 0),
                        FinishHour = new TimeSpan(18, 0, 0),
                        Name = "Breakdance",
                        EventType = EventType.Dance,
                        Director = director4,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-breakdance-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-breakdance1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-breakdance2.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Thursday,
                        StartHour = new TimeSpan(16, 0, 0),
                        FinishHour = new TimeSpan(18, 0, 0),
                        Name = "Breakdance infantil",
                        EventType = EventType.Dance,
                        Director = director4,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-children-header.jpg", "events"),
                            },
                        }
                    },
                };
                director5.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Wednesday,
                        StartHour = new TimeSpan(14, 0, 0),
                        FinishHour = new TimeSpan(16, 0, 0),
                        Name = "Baile urbano",
                        EventType = EventType.Dance,
                        Director = director5,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-dance-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-dance1.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Friday,
                        StartHour = new TimeSpan(16, 0, 0),
                        FinishHour = new TimeSpan(18, 0, 0),
                        Name = "Folclor en parejas",
                        EventType = EventType.Dance,
                        Director = director5,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-danceinpairs-header.jpg", "events"),
                            },
                        }
                    },
                };
                director6.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Monday,
                        StartHour = new TimeSpan(16, 0, 0),
                        FinishHour = new TimeSpan(18, 0, 0),
                        Name = "Boxeo",
                        EventType = EventType.Martial,
                        Director = director6,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-box-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-box1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-box2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-box3.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Wednesday,
                        StartHour = new TimeSpan(16, 0, 0),
                        FinishHour = new TimeSpan(18, 0, 0),
                        Name = "Lucha libre",
                        EventType = EventType.Martial,
                        Director = director6,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-fight-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-fight1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-fight2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-fight3.jpg", "events"),
                            },
                        }
                    },
                    new Event()
                    {
                        Day = DayOfWeek.Saturday,
                        StartHour = new TimeSpan(8, 0, 0),
                        FinishHour = new TimeSpan(10, 0, 0),
                        Name = "Karate",
                        EventType = EventType.Martial,
                        Director = director6,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-karate-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-karate1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-karate2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-karate3.jpg", "events"),
                            },
                        }
                    },
                };
                director7.Events = new List<Event>()
                {
                    new Event()
                    {
                        Day = DayOfWeek.Saturday,
                        StartHour = new TimeSpan(7, 0, 0),
                        FinishHour = new TimeSpan(9, 0, 0),
                        Name = "Yoga",
                        EventType = EventType.Yoga,
                        Director = director7,
                        Description = "",
                        EventImages = new List<EventImage>()
                        {
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-yoga-header.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-yoga1.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-yoga2.jpg", "events"),
                            },
                            new EventImage()
                            {
                                ImageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\home\\events\\events-yoga3.jpg", "events"),
                            },
                        }
                    },
                };
                _context.Add(director1);
                _context.Add(director2);
                _context.Add(director3);
                _context.Add(director4);
                _context.Add(director5);
                _context.Add(director6);
                _context.Add(director7);
            }
            await _context.SaveChangesAsync();
        }

        //EventInscriptions
        private async Task CheckEventInscription()
        {
            if (!_context.EventInscriptions.Any())
            {
                List<string> EmailsList = new()
                {
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                };

                var Events = await _context.Events.ToListAsync();

                foreach(Event objectEvent in Events)
                {
                    foreach(string email in EmailsList)
                    {
                        User user = await _userHelper.GetUserAsync(email);
                        EventInscription eventInscription = new()
                        {
                            Event = objectEvent,
                            User = user,
                            EventStatus = EventStatus.SignedUp,
                            InscriptionDate = DateTime.Now,
                        };
                        _context.Add(eventInscription);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        //Professionals
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

                Professional professional6 = new()
                {
                    User = await _userHelper.GetUserAsync("maia@yopmail.com"),
                    ProfessionalType = ProfessionalType.Nutritionist,
                    Service = await _context.Services.FindAsync(3),
                    ProfessionalSchedules = new List<ProfessionalSchedule>()
                };
                professional6.ProfessionalSchedules = new List<ProfessionalSchedule>()
                {
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(1),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(2),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(3),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(4),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(5),
                    },
                    new ProfessionalSchedule()
                    {
                        Professional = professional6,
                        Schedule = await _context.Schedules.FindAsync(6),
                    },
                };
                _context.Add(professional6);
                await _context.SaveChangesAsync();
            }
        }

        //Services
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

        //ServiceAccesses
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
                                    User user = await _userHelper.GetUserAsync(email);
                                    PlanInscription planInscription = await _context.PlanInscriptions
                                        .Include(pI => pI.User)
                                        .Include(pI => pI.Plan)
                                        .FirstOrDefaultAsync(
                                            pI => pI.User == user &&
                                            pI.PlanStatus == PlanStatus.Active
                                        );
                                    _context.Add(new ServiceAccess
                                    {
                                        User = user,
                                        Service = service,
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue(planInscription.Plan.PlanType),
                                        TotalPrice = service.Price - (service.Price * (decimal)DiscountValues.GetDiscountValue(planInscription.Plan.PlanType)),
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
                                    User user = await _userHelper.GetUserAsync(email);
                                    PlanInscription planInscription = await _context.PlanInscriptions
                                        .Include(pI => pI.User)
                                        .Include(pI => pI.Plan)
                                        .FirstOrDefaultAsync(
                                            pI => pI.User == user &&
                                            pI.PlanStatus == PlanStatus.Active
                                        );
                                    _context.Add(new ServiceAccess
                                    {
                                        User = user,
                                        Service = service,
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue(planInscription.Plan.PlanType),
                                        TotalPrice = service.Price - (service.Price * (decimal)DiscountValues.GetDiscountValue(planInscription.Plan.PlanType)),
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

        //Users
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

        //Roles
        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        //Plans
        private async Task CheckPlansAsync()
        {
            if (!_context.Plans.Any())
            {
                _context.Add(new Plan
                {
                    Name = "Tiquetera",
                    PlanType = PlanType.TicketHolder,
                    Price = 5900

                });

                _context.Add(new Plan
                {
                    Name = "Regular",
                    PlanType = PlanType.Regular,
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
        }

        //PlanInscriptions
        private async Task CheckPlanInscriptionAsync()
        {
            if (!_context.PlanInscriptions.Any())
            {
                User user = await _userHelper.GetUserAsync("brett@yopmail.com");
                User user2 = await _userHelper.GetUserAsync("brian@yopmail.com");
                User user3 = await _userHelper.GetUserAsync("millie@yopmail.com");
                User user4 = await _userHelper.GetUserAsync("lamar@yopmail.com");
                User user5 = await _userHelper.GetUserAsync("peyton@yopmail.com");
                Plan plan = await _context.Plans.FindAsync(1); //TicketHolder
                Plan plan2 = await _context.Plans.FindAsync(2); //Regular
                Plan plan3 = await _context.Plans.FindAsync(3); //Black
                PlanInscription pl1 = new()
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user,
                    Plan = plan,
                    Duration = 5,
                    ExpirationDate = DateTime.Today.AddDays(5),
                    PlanStatus = PlanStatus.Active,
                    TotalPrice = plan.Price * 5,
                    RemainingDays = 5,
                    Discount = DiscountValues.GetDiscountValue(plan.PlanType)
                };
                PlanInscription pl2 = new()
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user2,
                    Plan = plan2,
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Active,
                    TotalPrice = plan2.Price,
                    RemainingDays = 30,
                    Discount = 0
                };
                PlanInscription pl3 = new()
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user3,
                    Plan = plan3,
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Active,
                    TotalPrice = plan3.Price,
                    RemainingDays = 30,
                    Discount = 0
                };
                PlanInscription pl4 = new()
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user4,
                    Plan = plan2,
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Active,
                    TotalPrice = plan2.Price,
                    RemainingDays = 30,
                    Discount = 0
                };
                PlanInscription pl5 = new()
                {
                    InscriptionDate = DateTime.Today,
                    ActivationDate = DateTime.Today,
                    User = user5,
                    Plan = plan3,
                    Duration = 30,
                    ExpirationDate = DateTime.Today.AddDays(30),
                    PlanStatus = PlanStatus.Active,
                    TotalPrice = plan3.Price,
                    RemainingDays = 30,
                    Discount = 0
                };
                _context.Add(pl1);
                _context.Add(pl2);
                _context.Add(pl3);
                _context.Add(pl4);
                _context.Add(pl5);
                await _context.SaveChangesAsync();
            }
        }

        //Attendances
        private async Task CheckAttendancesAsync()
        {
            if (!_context.Attendances.Any())
            {
                List<string> EmailsList = new()
                {
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                };

                foreach (string email in EmailsList)
                {
                    var user = await _userHelper.GetUserAsync(email);
                    var serviceAccesses = await _context.ServiceAccesses.Where(sa => sa.User == user).ToListAsync();
                    foreach (ServiceAccess serviceAccess in serviceAccesses)
                    {
                        _context.Add(new Attendance
                        {
                            User = user,
                            AttendanceDate = serviceAccess.AccessDate
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
