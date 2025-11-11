using BLL.Interface;
using DAL.Interface;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class RepoService : IRepoService
    {


        private readonly IRepo _repository;

        public RepoService(IRepo repository)
        {
            _repository = repository;
        }
        public Task<Song> AddAsync(Song entity)
        {
            return _repository.AddAsync(entity);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            return _repository.DeleteByIdAsync(id);

        }

        public Task<IEnumerable<Song>> GetAllAsync()
        {

            return _repository.GetAllAsync();
        }

        public Task<Song> GetByIdAsync(int id)
        {

            return _repository.GetByIdAsync(id);
        }

        public Task<Song> UpdateByIdAsync(int id, Song entity)
        {
            return _repository.UpdateByIdAsync(id, entity);
        }
    }
}
