using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class Repo : IRepo
    {
        private readonly Prn231Sp24AlbumDbContext _context;

        public Repo(Prn231Sp24AlbumDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Song>> GetAllAsync()
        {
            return await _context.Songs
                                 .Include(l => l.Album)
                                 .ToListAsync();
        }

        public async Task<Song> GetByIdAsync(int id)
        {
            return await _context.Songs
                                 .Include(l => l.Album)
                                 .FirstOrDefaultAsync(l => l.SongId == id);
        }

        public async Task<Song> AddAsync(Song entity)
        {
            await _context.Songs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Song> UpdateByIdAsync(int id, Song entity)
        {
            var existing = await _context.Songs.FindAsync(id);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var existing = await _context.Songs.FindAsync(id);
            if (existing == null) return false;

            _context.Songs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
