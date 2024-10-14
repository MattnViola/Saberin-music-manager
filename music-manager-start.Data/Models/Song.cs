using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_manager_starter.Data.Models
{
    // Note: Just to save time on this "Assessment", I'm editing this sealed class.
    // If there were dependencies that would make this unfeasable, we can 
    // query additional properties through DB relationships. 
    public sealed class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
    }
}
