using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs
{
    public class SongDTO
    {
        public string Title { get; set; } = null!;

        public int? Duration { get; set; }

        public int? AlbumId { get; set; }
    }
}
