﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using music_manager_starter.Server.Hubs;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly IHubContext<SongHub> _hubContext;
        private readonly ILogger<SongsController> _logger;

        public SongsController(DataDbContext context, IHubContext<SongHub> hubContext, ILogger<SongsController> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs([FromQuery] string filter = "All", [FromQuery] string query = "")
        {
            _logger.LogInformation("Fetching songs with filter {Filter} and query {Query}", filter, query);

            try
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

                var result = await songsQuery.ToListAsync();
                _logger.LogInformation("{Count} songs found", result.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching songs with filter {Filter} and query {Query}", filter, query);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            _logger.LogInformation("Received request to add a new song");

            if (song == null)
            {
                _logger.LogWarning("Received a null song in the request");
                return BadRequest("Song cannot be null.");
            }

            try
            {
                _context.Songs.Add(song);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Song '{Title}' by '{Artist}' added to the database", song.Title, song.Artist);

                await SendSongNotification(song.Title);
                _logger.LogInformation("Notification sent for the song '{Title}'", song.Title);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding the song '{Title}'", song.Title);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpPost("{id}/upload-album-art")]
        public async Task<IActionResult> UploadAlbumArt(Guid id, IFormFile albumArt)
        {
            _logger.LogInformation("Received request to upload album cover art for song ID {SongId}", id);

            if (albumArt == null || albumArt.Length == 0)
            {
                _logger.LogWarning("No album art was uploaded for song ID {SongId}", id);
                return BadRequest("No file was uploaded.");
            }

            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                _logger.LogWarning("Song with ID {SongId} not found", id);
                return NotFound("Song not found.");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await albumArt.CopyToAsync(memoryStream);
                    song.AlbumCoverArt = memoryStream.ToArray();
                }

                _context.Songs.Update(song);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully uploaded album cover art for song '{Title}' by '{Artist}'", song.Title, song.Artist);

                return Ok("Album art uploaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading album art for song '{Title}'", song.Title);
                return StatusCode(500, "An error occurred while uploading the album art.");
            }
        }


        public async Task SendSongNotification(string songTitle)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveSongNotification", songTitle);
                _logger.LogInformation("Successfully sent song notification for '{Title}'", songTitle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notification for the song '{Title}'", songTitle);
            }
        }
    }
}
