using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /// <summary>
    /// Mediates between the domain and data mapping layers using a collection-like interface for accessing domain objects.
    /// https://martinfowler.com/eaaCatalog/repository.html
    /// 
    /// A Repository mediates between the domain and data mapping layers, acting like an in-memory domain object collection. 
    /// Client objects construct query specifications declaratively and submit them to Repository for satisfaction.
    /// 
    /// Repository also supports the objective of achieving a clean separation and one-way dependency between the domain and data mapping layers.
    /// You can also find a good write-up of this pattern in Domain Driven Design.
    /// </summary>
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId);
    }
}