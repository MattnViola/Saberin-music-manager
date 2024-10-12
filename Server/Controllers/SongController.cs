using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using music_manager_starter.Server.Hubs;
using System;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IHubContext<SongHub> _hubContext;

        public SongsController(DataDbContext context, IHubContext<SongHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;            
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            if (song == null)
            {
                return BadRequest("Song cannot be null.");
            }


            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("SongNotification", song.Title);

            return Ok();
        }
    }
}
