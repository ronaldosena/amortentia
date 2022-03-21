using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Dtos;
using backend.Helpers;
using backend.Models;

namespace backend.Data
{
    public interface IUsersRepository
    {
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(string username);
        Task<bool> UpdateUser(string username, UserForUpdateDto userForUpdate);
        Task<bool> UpdateLastActive(string username);
        Task<bool> AddPhoto(string username, Photo photo);
        Task<Photo> GetPhoto(string username, string photoId);
        Task<bool> AnyMainPhoto(string username);
        Task<bool> SetMainPhoto(string username, string photoId);
        Task<bool> IsMainPhoto(string username, string photoId);
        Task<bool> DeletePhoto(string username, string photoId);
    }
}