using Microsoft.AspNetCore.SignalR;
using Moq;
using music_manager_starter.Server.Controllers;
using music_manager_starter.Server.Hubs;
using music_manager_starter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace music_manager_starter.Tests
{
    public class SongsControllerTests
    {
        [Fact]
        public async Task SendSongNotification()
        {

            // The controller requires a MockDBContext and a Mock SignalR hub.
            // I intentially decided to not include CRUD operations in the unit test, as I didn't
            // want to fill up the current dev DB, or spin up another one.
            var mockDbContext = new Mock<DataDbContext>(new DbContextOptions<DataDbContext>());

            var mockHubContext = new Mock<IHubContext<SongHub>>();
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            var mockLogger = new Mock<ILogger<SongsController>>();

            mockHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);


            // Post Setup
            var songsController = new SongsController(mockDbContext.Object, mockHubContext.Object, mockLogger.Object);
            await songsController.SendSongNotification("Test Song");


            // Verification
            mockClientProxy.Verify(
                client => client.SendCoreAsync(
                    "ReceiveSongNotification",
                    It.Is<object[]>(o => o[0].ToString() == "Test Song"),
                    default), Times.Once);
        }
    }
}
