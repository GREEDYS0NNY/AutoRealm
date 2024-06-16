using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AutoRealmProject.Backend.Data;
using AutoRealmProject.Backend.Entities;
using Microsoft.AspNetCore.Identity;
using AutoRealmProject.Frontend;
using System.Net.Http.Json;

namespace AutoRealmProject.Tests
{
    [FeatureDescription("Car Ads Management")]
    public class CarAdsTests : FeatureFixture
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CarAdsTests()
        {
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AppDbContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                    }
                });
            });

            _client = _factory.CreateClient();
        }

        [Scenario]
        public async Task Create_new_car_ad()
        {
            await Runner.RunScenarioAsync(
                given => GivenUserIsAnAuthenticatedUser(),
                when => WhenUserNavigatesToTheCreateCarAdPage(),
                and => WhenUserSubmitsANewCarAdWithValidData(),
                then => ThenTheCarAdIsCreatedSuccessfully());
        }

        [Scenario]
        public async Task Edit_existing_car_ad()
        {
            await Runner.RunScenarioAsync(
                given => GivenUserIsAnAuthenticatedUser(),
                and => GivenUserHasAnExistingCarAd(),
                when => WhenUserNavigatesToTheEditCarAdPage(),
                and => WhenUserSubmitsUpdatedDataForTheCarAd(),
                then => ThenTheCarAdIsUpdatedSuccessfully());
        }

        [Scenario]
        public async Task Delete_existing_car_ad()
        {
            await Runner.RunScenarioAsync(
                given => GivenUserIsAnAuthenticatedUser(),
                and => GivenUserHasAnExistingCarAd(),
                when => WhenUserNavigatesToTheDeleteCarAdPage(),
                and => WhenUserConfirmsTheDeletion(),
                then => ThenTheCarAdIsDeletedSuccessfully());
        }

        private async Task GivenUserIsAnAuthenticatedUser()
        {
            var user = await CreateUserAsync();

            var loginData = new Dictionary<string, string>
            {
                { "Email", "testuser@gmail.com" },
                { "Password", "test12345" }
            };

            var loginContent = new FormUrlEncodedContent(loginData);
            var response = await _client.PostAsync("/Identity/Account/Login", loginContent);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to log in: {response.StatusCode}, {responseContent}");
            }

            response.EnsureSuccessStatusCode();
        }

        private async Task GivenUserHasAnExistingCarAd()
        {
            var user = await CreateUserAsync();
            var carAd = new CarAd
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2020,
                Price = 20000,
                Mileage = 5000,
                City = "New York",
                OwnerId = user.Id
            };
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.CarAds.Add(carAd);
            await db.SaveChangesAsync();
        }

        private async Task WhenUserNavigatesToTheCreateCarAdPage()
        {
            var response = await _client.GetAsync("/CarAds/Create");
            response.EnsureSuccessStatusCode();
        }

        private async Task WhenUserSubmitsANewCarAdWithValidData()
        {
            var carAd = new CarAd
            {
                Brand = "Toyota",
                Model = "Corolla",
                Year = 2020,
                Price = 20000,
                Mileage = 5000,
                City = "New York"
            };
            var response = await _client.PostAsJsonAsync("/CarAds/Create", carAd);
            response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheCarAdIsCreatedSuccessfully()
        {
            var response = await _client.GetAsync("/CarAds");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Toyota", responseString);
        }

        private async Task WhenUserNavigatesToTheEditCarAdPage()
        {
            var response = await _client.GetAsync("/CarAds/Edit/5");
            response.EnsureSuccessStatusCode();
        }

        private async Task WhenUserSubmitsUpdatedDataForTheCarAd()
        {
            var carAd = new CarAd
            {
                AdId = 1,
                Brand = "Honda",
                Model = "Civic",
                Year = 2021,
                Price = 22000,
                Mileage = 3000,
                City = "San Francisco"
            };
            var response = await _client.PostAsJsonAsync("/CarAds/Edit/5", carAd);
            response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheCarAdIsUpdatedSuccessfully()
        {
            var response = await _client.GetAsync("/CarAds/Details/5");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Honda", responseString);
            Assert.Contains("Civic", responseString);
        }

        private async Task WhenUserNavigatesToTheDeleteCarAdPage()
        {
            var response = await _client.GetAsync("/CarAds/Delete/5");
            response.EnsureSuccessStatusCode();
        }

        private async Task WhenUserConfirmsTheDeletion()
        {
            var response = await _client.PostAsync("/CarAds/Delete/5", null);
            response.EnsureSuccessStatusCode();
        }

        private async Task ThenTheCarAdIsDeletedSuccessfully()
        {
            var response = await _client.GetAsync("/CarAds/Details/5");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        private async Task<AppUser> CreateUserAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var user = new AppUser
            {
                UserName = "testuser@gmail.com",
                Email = "testuser@gmail.com",
                FirstName = "Test",
                LastName = "User"
            };
            var result = await userManager.CreateAsync(user, "test12345");

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return user;
        }
    }
}
