using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _data;

        public AuthRepository(DataContext dataContext)
        {
            _data = dataContext;
        }

        /// <summary>
        /// Verifies if input password matches stored password.
        /// </summary>
        /// <param name="username">User name</param>
        /// <param name="password">User password</param>
        /// <returns>User from database</returns>
        public async Task<User> Login(string username, string password)
        {
            // ORM Access -> FirstOrDefaultAsync
            var user = await _data.Users.FirstOrDefaultAsync(x => x.Username == username);

            // Business logic
            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Security cryptography
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            // Business logic
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // ORM Access -> AddAsync, SaveChangesAsync
            await _data.Users.AddAsync(user);
            await _data.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            // ORM Access -> AnyAsync
            if (await _data.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}