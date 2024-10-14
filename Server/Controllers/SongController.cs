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
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs([FromQuery] string filter = "All", [FromQuery] string query = "")
        {
            IQueryable<Song> songsQuery = _context.Songs;

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                switch (filter)
                {
                    case "Title":
                        songsQuery = songsQuery.Where(s => s.Title.ToLower().Contains(query));
                        break;
                    case "Artist":
                        songsQuery = songsQuery.Where(s => s.Artist.ToLower().Contains(query));
                        break;
                    case "Album":
                        songsQuery = songsQuery.Where(s => s.Album.ToLower().Contains(query));
                        break;
                    default:
                        songsQuery = songsQuery.Where(s => s.Title.ToLower().Contains(query)
                                                        || s.Artist.ToLower().Contains(query)
                                                        || s.Album.ToLower().Contains(query));
                        break;
                }
            }

            return Ok(await songsQuery.ToListAsync());
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

            await SendSongNotification(song.Title);

            return Ok();
        }

        public async Task SendSongNotification(string songTitle)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveSongNotification", songTitle);
        }
    }
}
