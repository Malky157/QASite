using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework5._1.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connection)
        {
            _connectionString = connection;
        }
        public void Add(User user, string password)
        {
            using var context = new QADbContext(_connectionString);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Users.Add(user);
            context.SaveChanges();
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (isValidPassword)
            {
                return user;
            }

            return null;
        }

        public User GetByEmail(string email)
        {
            using var context = new QADbContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool IsEmailAvailable(string email)
        {
            using var context = new QADbContext(_connectionString);
            return !context.Users.Any(u => u.Email == email);
        }
    }
}
