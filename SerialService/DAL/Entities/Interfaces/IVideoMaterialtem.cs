namespace SerialService.DAL.Entities
{
    using System;
    using System.Collections.Generic;

    public interface IVideoMaterialtem : IMaterialItem
    {
        string OriginalTitle { get; set; }
        string KinopoiskID { get; set; }
        float IDMB { get; set; }
        int? Duration { get; set; } 
        int? ReleaseDate { get; set; }
        DateTime? MoonWalkAddDate { get; set; }
        List<Country> Countries { get; set; }
        List<Genre> Genres { get; set; }
        List<Picture> Pictures { get; set; }
        List<Person> FilmMakers { get; set; }
        List<Person> Actors { get; set; }
        List<VideoMark> VideoMarks { get; set; }
        List<SerialSeason> SerialSeasons { get; set; }
    }
}
