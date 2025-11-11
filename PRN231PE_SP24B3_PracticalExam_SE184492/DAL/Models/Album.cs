using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Album
{
    public int AlbumId { get; set; }

    public string Title { get; set; } = null!;

    public string Artist { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
