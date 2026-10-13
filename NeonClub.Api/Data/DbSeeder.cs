using Microsoft.EntityFrameworkCore;
using neon_club_api.Models;
using NeonClub.Shared.Models;

namespace NeonClub.Api.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(AppDbContext db)
        {
            if (await db.Roles.AnyAsync()) return;
            db.Roles.AddRange(
                new Role { Name = "user" },
                new Role { Name = "admin" }
            );
            await db.SaveChangesAsync();
        }

        public static async Task SeedAdminUserAsync(AppDbContext db)
        {
            if (await db.Users.AnyAsync(u => u.Email == "admin@neon.club")) return;

            var adminRole = await db.Roles.FirstAsync(r => r.Name == "admin");

            db.Users.Add(new User
            {
                Name = "Admin",
                Email = "admin@neon.club",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                RoleId = adminRole.Id
            });

            await db.SaveChangesAsync();
        }

        public static async Task SeedStatusesAsync(AppDbContext db)
        {
            if (await db.Statuses.AnyAsync()) return;
            db.Statuses.AddRange(
                new Status { Name = "новый", Description = "Заявка создана" },
                new Status { Name = "подтверждён", Description = "Заявка подтверждена" },
                new Status { Name = "отменён", Description = "Заявка отменена" }
            );
            await db.SaveChangesAsync();
        }

        public static async Task SeedZonesAsync(AppDbContext db)
        {
            if (await db.Zones.AnyAsync()) return;

            var zones = new List<Zone>
        {
            new()
            {
                Title = "Standard", Subtitle = "Классическое игровое место",
                Category = "pc", Price = 150, Emoji = "🖥️", Tag = "Хит", Popular = true,
                Cpu = "Intel Core i5-12400F", Gpu = "NVIDIA RTX 3060 12 GB",
                Ram = "16 GB DDR4 3200", Monitor = "24\" / 144 Гц",
                Description = "Универсальное место для большинства сетевых игр: CS2, Dota 2, Valorant, Fortnite.",
                Features = ["Мышь Logitech G102", "Механическая клавиатура", "Наушники HyperX"]
            },
            new()
            {
                Title = "Advanced", Subtitle = "Для тех, кому мало стандарта",
                Category = "pc", Price = 220, Emoji = "⚡", Popular = false,
                Cpu = "Intel Core i5-13600KF", Gpu = "NVIDIA RTX 4060 Ti",
                Ram = "32 GB DDR5", Monitor = "27\" / 180 Гц",
                Description = "Промежуточный вариант для тех, кто хочет больше FPS и комфортное кресло.",
                Features = ["Мышь Razer DeathAdder", "Клавиатура Razer BlackWidow", "Игровое кресло DXRacer"]
            },
            new()
            {
                Title = "Pro Gaming", Subtitle = "Турнирная конфигурация",
                Category = "pc", Price = 300, Emoji = "🚀", Tag = "Топ", Popular = false,
                Cpu = "AMD Ryzen 7 7800X3D", Gpu = "NVIDIA RTX 4070 Super",
                Ram = "32 GB DDR5 6000", Monitor = "27\" / 240 Гц",
                Description = "Места для внутренних турниров. Максимальная частота обновления.",
                Features = ["Мышь Logitech G Pro X Superlight", "Клавиатура Wooting 60HE", "Гарнитура SteelSeries"]
            },
            new()
            {
                Title = "VIP Lounge", Subtitle = "Отдельный зал на 2 места",
                Category = "vip", Price = 450, Emoji = "👑", Tag = "Хит", Popular = true,
                Cpu = "Intel Core i7-14700K", Gpu = "NVIDIA RTX 4080 Super",
                Ram = "64 GB DDR5", Monitor = "2× 27\" / 240 Гц",
                Description = "Приватная комната для двоих.",
                Features = ["Два места в отдельной комнате", "Диван и мини-бар", "Кондиционер"]
            },
            new()
            {
                Title = "Bootcamp", Subtitle = "Зона для команды из 5 человек",
                Category = "vip", Price = 600, Emoji = "🛡️", Tag = "Командам", Popular = false,
                Cpu = "AMD Ryzen 7 7700X", Gpu = "NVIDIA RTX 4070",
                Ram = "32 GB DDR5", Monitor = "24\" / 240 Гц",
                Description = "Отдельная зона для тренировок состава.",
                Features = ["5 мест в ряд", "Отдельная доска для тактики", "Чай и кофе включены"]
            },
            new()
            {
                Title = "PlayStation 5", Subtitle = "Диван, 4 геймпада, 4K-телевизор",
                Category = "console", Price = 250, Emoji = "🎮", Popular = false,
                Cpu = "AMD Zen 2 (8 ядер)", Gpu = "AMD RDNA 2, 10.3 TFLOPS",
                Ram = "16 GB GDDR6", Monitor = "55\" 4K / 120 Гц",
                Description = "Консольная зона для FIFA, Mortal Kombat, Tekken.",
                Features = ["4 геймпада DualSense", "Диван на 5 человек", "Подписка PS Plus"]
            },
            new()
            {
                Title = "Xbox Series X", Subtitle = "Game Pass и большой экран",
                Category = "console", Price = 250, Emoji = "🕹️", Popular = false,
                Cpu = "AMD Zen 2 (8 ядер)", Gpu = "AMD RDNA 2, 12 TFLOPS",
                Ram = "16 GB GDDR6", Monitor = "55\" 4K / 120 Гц",
                Description = "Более сотни игр по подписке.",
                Features = ["Подписка Game Pass Ultimate", "3 геймпада", "Беспроводные наушники"]
            },
            new()
            {
                Title = "VR Arena", Subtitle = "Свободная площадка 4×4 метра",
                Category = "vr", Price = 400, Emoji = "🥽", Tag = "Новинка", Popular = true,
                Cpu = "Intel Core i7-13700F", Gpu = "NVIDIA RTX 4080",
                Ram = "32 GB DDR5", Monitor = "Meta Quest 3 + трекинг",
                Description = "Полноценный VR с трекингом по комнате.",
                Features = ["Площадка 4×4 м", "Инструктор рядом", "Гигиенические маски"]
            },
        };

            db.Zones.AddRange(zones);
            await db.SaveChangesAsync();
        }
    }
}
