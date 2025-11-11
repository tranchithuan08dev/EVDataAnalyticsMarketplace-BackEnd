using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IRepoService
    {
        Task<IEnumerable<Song>> GetAllAsync();

        Task<Song> GetByIdAsync(int id);

        Task<Song> AddAsync(Song entity);

        Task<Song> UpdateByIdAsync(int id, Song entity);

        Task<bool> DeleteByIdAsync(int id);
    }
}
