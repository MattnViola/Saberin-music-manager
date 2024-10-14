namespace music_manager_starter.Shared
{
    // NOTE: See Model's Song.cs for update reasons.
    public sealed class Song
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public byte[]? AlbumCoverArt { get; set; }
    }
}
