﻿namespace PhimMoi.Resources.Movie
{
    public class MovieResource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TranslateName { get; set; }
        public string? Description { get; set; }
        public DateTime? Releasedate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Length { get; set; }
        public int View { get; set; }
        public string? Image { get; set; }
        public string? Trailer { get; set; }
        public float Rating { get; set; }
        public string Type { get; set; }
        public int EpisodeCount { get; set; }
        public string Status { get; set; }
    }
}