using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

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
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        /// <summary>
        /// Gets users as paged list via EF ORM
        /// </summary>
        /// <param name="userParams">Request model that includes paging, filtering and sorting information.</param>
        /// <returns>Paged list of users</returns>
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // EF ORM: [INCLUDE] : Specifies related entities to include in the query results. 
            // The navigation property to be included is specified starting with the type of entity
            // being queried (TEntity ). If you wish to include additional types based on the navigation 
            // properties of the type being included, then chain a call to Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ThenInclude
            //
            // LINQ: [AsQueryable] Converts a generic System.Collections.Generic.IEnumerable 
            // to a generic System.Linq.IQueryable.
            var users = _context.Users
                .Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive)
                .AsQueryable();

            // TODO: Filtering and sorting users on the level of ORM... Is this business logic?
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            // Logic to handle paged collection collection is inside PagedList
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// This method will automatically call Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges 
        /// to discover any changes to entity instances before saving to the underlying database. 
        /// This can be disabled via Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}