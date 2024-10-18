using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Client;
using music_manager_starter.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;

namespace music_manager_starter.Tests
{
    public class SongsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly HubConnection _connection;
        private string? _receivedSongTitle;
        private bool _notificationReceived;

        public SongsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<DataDbContext>(options => options.UseInMemoryDatabase("InMemoryTestDb"));
                    services.AddSignalR();
                });
            });

            _client = _factory.CreateClient();
            var server = _factory.Server;

            // Note: Had a ton of issues getting SignalR to mock correctly, mostly failing to get a connection.
            // Solution taken from https://stackoverflow.com/questions/78469425/how-to-write-integration-test-for-signalr
            var signalRHubUrl = "ws://localhost/songhub";

            _connection = new HubConnectionBuilder()
                .WithUrl(signalRHubUrl, options =>
                {
                    options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                })
                .Build();

            _connection.On<string>("ReceiveSongNotification", (songTitle) =>
            {
                _notificationReceived = true;
                _receivedSongTitle = songTitle;
            });

            _connection.StartAsync().Wait();
        }

        [Fact]
        public async Task PostSong_ShouldAddSongAndSendNotification()
        {
            var newSong = new
            {
                Title = "Test Song",
                Artist = "Test Artist",
                Album = "Test Album",
                Genre = "Test Genre"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(newSong), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/songs", jsonContent);

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

            var timeout = 5000;
            var cancellationTokenSource = new CancellationTokenSource(timeout);

            while (!_notificationReceived && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                await Task.Delay(100);
            }

            Assert.True(_notificationReceived, "SignalR notification was not received.");
            Assert.Equal("Test Song", _receivedSongTitle);

            await _connection.StopAsync();
        }
    }


}