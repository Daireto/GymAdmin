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
                        Description = "Los ejercicios de Core  entrenan los músculos de la pelvis, la baja espalda, la cadera y " +
                        "el abdomen para que trabajen en armonía. Esto produce un mejor equilibrio y estabilidad. Los problemas " +
                        "de espalda suelen ser muy comunes en las mujeres, por lo tanto, es necesario realizar buenos ejercicios " +
                        "para aliviar dichos problemas.\n\nInscríbete y aprende los mejores ejercicios de Core para mejorar " +
                        "tu equilibrio y estabilidad.",
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
                        Description = "La práctica de la gimnasia es importante porque disciplina al individuo en todos los sentidos. " +
                        "Ayuda a desarrollar la estabilidad emocional, la cual viene dada por la concentración, velocidad de reflejos " +
                        "y seguridad que debe preceder a cada actuación. Constituye una disciplina que ayuda a mantener la elasticidad " +
                        "y fuerza natural.\n\nInscríbete y aprende sobre gimnasia con nuestros directores más experimentados.",
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
                        Description = "El beneficio fundamental del trabajo de cardio es, entre otros, ayudar al corazón a que " +
                        "funcione de una mejor manera. También contribuye a mejorar la capacidad respiratoria. El ejercicio de " +
                        "cardio hace que se nivelen algunos aspectos fundamentales en el cuerpo.\n\nVen a hacer cardio con " +
                        "nosotros y aprende a mejorar tu salud cardiaca bajo la orientación de nuestros directores más experimentados.",
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
                        Description = "Para muchos atletas y deportistas, uno de las habilidades más importantes es el salto. " +
                        "Un buen salto puede marcar la diferencia en el deporte que se está practicando, ya sea atletismo, baloncesto, " +
                        "fútbol, etc.\n\nSi quieres mejorar tu salto y aumentar la fuerza de tus piernas, inscríbete y entrena con " +
                        "nosotros.",
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
                        Description = "Bailar Ballet es una de las grandes pasiones de toda una serie de hombres y mujeres de a lo largo " +
                        "de todo el mundo. Los profesionales que lo practican pasan mucho tiempo desarrollando su técnica, logrando " +
                        "resultados admirables.\n\nSi quieres ampliar tus conocimientos y aprender todo sobre el Ballet, no dudes " +
                        "en inscribirte y unirte a nosotros.",
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
                        Description = "El Breakdance o el Bboying es una danza social que forma parte de la cultura del hip hop, junto " +
                        "con el grafiti, rap y djing. Este elemento nace en las comunidades de los barrios neoyorquinos como Bronx y " +
                        "Brooklyn, en Estados Unidos, en la década de 1960.\n\nSi quieres aprender a bailar Breakdance no dudes " +
                        "en inscribirte y unirte a nosotros.",
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
                        Description = "El Breakdance o el Bboying es una danza social que forma parte de la cultura del hip hop, junto " +
                        "con el grafiti, rap y djing. Este elemento nace en las comunidades de los barrios neoyorquinos como Bronx y " +
                        "Brooklyn, en Estados Unidos, en la década de 1960.\n\nLos niños y niñas también pueden disfrutar de este " +
                        "estilo de baile con nostros.",
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
                        Description = "El baile urbano o baile callejero es un estilo de baile, independientemente del país de origen, " +
                        "el cual evolucionó fuera de los estudios de baile en cualquier espacio abierto disponible como calles, fiestas " +
                        "de baile, fiestas de barrio, parques, patios escolares, raves y clubes nocturnos.\n\nSi quieres aprender " +
                        "los fundamentos del baile urbano, este es tu lugar. Únete a nosotros y aprende todo sobre este movimiento.",
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
                        Description = "La danza folklórica es la danza tradicional de carácter social que se practica en grupos y que " +
                        "forma parte de patrimonio cultural de una región o sociedad. Este tipo de baile también se practica en parejas " +
                        "y es el más emocionante en las fiestas y reuniones familiares.\n\nVen con nosotros para aprender todos los " +
                        "fundamentos del folclor y conviértete en un experto o experta.",
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
                        Description = "El boxeo y la lucha son dos de los deportes de combate más antiguos de la historia. " +
                        "En la actualidad es reconocido como uno de los sistemas de entrenamientos cardiovasculares más efectivos " +
                        "y puede ser practicado por mujeres, hombres y niños.\n\nLas clases de boxeo en GymAdmin están dirigidas a " +
                        "mujeres, hombres y niños a partir de los 9 años.",
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
                        Description = "La lucha libre es un deporte físico popular en el que se puede competir en la escuela secundaria, " +
                        "la universidad, a nivel de aficionado o profesional. Si estás interesado en aprender algunos fundamentos de la " +
                        "lucha libre, unirte a nuestro gimnasio es la mejor manera de hacerlo.\n\nRecibirás asesorías y acompañamiento " +
                        "continuo de nuestros directores más experimentados y profesionales.",
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
                        Description = "El Karate es un arte marcial japonés que se considera, sobre todo, un arte defensivo. " +
                        "No necesitas inscribirte en el Dojo más caro para aprender esta grandiosa arte marcial. En GymAdmin " +
                        "aprenderás todo sobre el Karate, desde las técnicas de defensa más básicas hasta las más avanzadas. " +
                        "\n\nInscríbete y demuestra tu poder entrenando " +
                        "Karate con nosotros.",
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
                        Description = "Ven a hacer Yoga con nosotros para equilibrar cuerpo y mente, reducir el estrés y el agotamiento," +
                        " para despejar tu mente de cualquier dificultad por la que estés pasando, y para que tengas más claridad de lo " +
                        "que quieres hacer y lo que quieres lograr.\n\nEn GymAdmin te ofrecemos las mejores terapias complementarias a la" +
                        " práctica de Yoga con la orientación de nuestros mejores directores.",
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
                    "victoria@yopmail.com",
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "vanessa@yopmail.com",
                    "rihanna@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                    "madison@yopmail.com",
                    "aliya@yopmail.com",
                    "mariana@yopmail.com",
                    "greeicy@yopmail.com",
                    "chris@yopmail.com",
                };

                var Events = await _context.Events.ToListAsync();

                bool exit;
                foreach (string email in EmailsList)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        exit = false;
                        while (exit == false)
                        {
                            var random = new Random();
                            int index = random.Next(Events.Count);

                            User user = await _userHelper.GetUserAsync(email);

                            var ei = await _context.EventInscriptions
                                .Where(ei => ei.User == user && ei.Event == Events[index] && ei.EventStatus == EventStatus.SignedUp)
                                .FirstOrDefaultAsync();

                            if (ei == null)
                            {
                                EventInscription eventInscription = new()
                                {
                                    Event = Events[index],
                                    User = user,
                                    EventStatus = EventStatus.SignedUp,
                                    InscriptionDate = DateTime.Now,
                                };
                                _context.Add(eventInscription);
                                exit = true;
                            }
                        }
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
                    "victoria@yopmail.com",
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "vanessa@yopmail.com",
                    "rihanna@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                    "madison@yopmail.com",
                    "aliya@yopmail.com",
                    "mariana@yopmail.com",
                    "greeicy@yopmail.com",
                    "chris@yopmail.com",
                };

                int randomHour = 13;
                int randomDay = 1;
                bool notExist = false;

                Enums.ServiceStatus status = Enums.ServiceStatus.Pending;
                foreach (string email in EmailsList)
                {
                    for (int i = 0; i < 5; i++)
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

                            var random = new Random();
                            int index = random.Next(ServicesList.Count);

                            DateTime searchDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour);

                            var serviceAccesses = await _context.ServiceAccesses
                                .Where(sa =>
                                    sa.Service.Id == ServicesList[index].Id &&
                                    sa.AccessDate == searchDate &&
                                    sa.ServiceStatus == status)
                                .ToListAsync();

                            if (serviceAccesses.Count == 0 || serviceAccesses == null)
                            {
                                var professionals = await _context.Professionals
                                    .Include(p => p.Service)
                                    .Include(p => p.ProfessionalSchedules)
                                    .ThenInclude(ps => ps.Schedule)
                                    .Where(p => p.Service.Id == ServicesList[index].Id)
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
                                        Service = ServicesList[index],
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue(planInscription.Plan.PlanType),
                                        TotalPrice = ServicesList[index].Price - (ServicesList[index].Price * (decimal)DiscountValues.GetDiscountValue(planInscription.Plan.PlanType)),
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
                    for (int i = 0; i < 5; i++)
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

                            var random = new Random();
                            int index = random.Next(ServicesList.Count);

                            DateTime searchDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour);

                            var serviceAccesses = await _context.ServiceAccesses
                                .Where(sa =>
                                    sa.Service.Id == ServicesList[index].Id &&
                                    sa.AccessDate == searchDate &&
                                    sa.ServiceStatus == status)
                                .ToListAsync();

                            if (serviceAccesses.Count == 0 || serviceAccesses == null)
                            {
                                var professionals = await _context.Professionals
                                    .Include(p => p.Service)
                                    .Include(p => p.ProfessionalSchedules)
                                    .ThenInclude(ps => ps.Schedule)
                                    .Where(p => p.Service.Id == ServicesList[index].Id)
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
                                        Service = ServicesList[index],
                                        AccessDate = DateTime.Today.AddDays(randomDay).AddHours(randomHour),
                                        ServiceStatus = status,
                                        Discount = DiscountValues.GetDiscountValue(planInscription.Plan.PlanType),
                                        TotalPrice = ServicesList[index].Price - (ServicesList[index].Price * (decimal)DiscountValues.GetDiscountValue(planInscription.Plan.PlanType)),
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
                    Price = 49900
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
                List<string> EmailsList = new()
                {
                    "victoria@yopmail.com",
                    "millie@yopmail.com",
                    "brett@yopmail.com",
                    "brian@yopmail.com",
                    "vanessa@yopmail.com",
                    "rihanna@yopmail.com",
                    "lamar@yopmail.com",
                    "peyton@yopmail.com",
                    "madison@yopmail.com",
                    "aliya@yopmail.com",
                    "mariana@yopmail.com",
                    "greeicy@yopmail.com",
                    "chris@yopmail.com",
                };

                var plans = await _context.Plans.ToListAsync();

                foreach (string email in EmailsList)
                {
                    var random = new Random();
                    int index = random.Next(plans.Count);

                    User user = await _userHelper.GetUserAsync(email);

                    int Duration = random.Next(12);

                    PlanInscription pl = new()
                    {
                        InscriptionDate = DateTime.Today,
                        ActivationDate = DateTime.Today,
                        User = user,
                        Plan = plans[index],
                        PlanStatus = PlanStatus.Active,
                        Discount = DiscountValues.GetDiscountValue(plans[index].PlanType)
                    };

                    if (plans[index].PlanType == PlanType.TicketHolder)
                    {
                        pl.Duration = Duration;
                        pl.ExpirationDate = DateTime.Today.AddDays(90);
                        pl.RemainingDays = Duration;
                    }
                    else
                    {
                        pl.Duration = Duration * 30;
                        pl.ExpirationDate = DateTime.Today.AddDays(5);
                        pl.RemainingDays = Duration * 30;
                    }
                    pl.TotalPrice = plans[index].Price * Duration;
                    _context.Add(pl);
                }
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
