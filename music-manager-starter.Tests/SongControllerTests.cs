using Microsoft.AspNetCore.SignalR;
using Moq;
using music_manager_starter.Server.Controllers;
using music_manager_starter.Server.Hubs;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            mockHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);


            // Post Setup
            var songsController = new SongsController(mockDbContext.Object, mockHubContext.Object);
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
