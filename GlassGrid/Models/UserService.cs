using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using GlassGrid.Models.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using GlassGrid.Models.Data;
using Org.BouncyCastle.Crypto.Generators;



namespace GlassGrid.Models
{
    public interface IEFUserService
    {
        bool IsUsernameAvailable(string username);
        bool ValidateUser(string username, string password);
        void RegisterUser(string username, string password);

        void SaveScore(string username, TimeSpan timeElapsed, string seed, DifficultyEnums difficulty);



        IEnumerable<ScoreModel> GetUserBestScores(string username, string seed, DifficultyEnums difficulty);

        IEnumerable<ScoreModel> GetGlobalBestScores(string seed, DifficultyEnums difficulty);

        IEnumerable<ScoreModel> GetAllUserScores(string username);
        void DeleteScoresBySeedAndDifficulty(string currentUsername, string deleteSeed, DifficultyEnums value);

        bool UpdateUsername(string oldUsername, string newUsername);
    }



    public class EfUserService : IEFUserService
    {
        private readonly GlassGridContext _context;

        public EfUserService(GlassGridContext context)
        {
            _context = context;
        }

        public bool IsUsernameAvailable(string username)
        {
            return !_context.Users.Any(u => u.Username == username);
        }

        public bool ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }


        public void RegisterUser(string username, string password)
        {
            if (!IsUsernameAvailable(username)) return;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new UserModel
            {
                Username = username,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }


        public void SaveScore(string username, TimeSpan timeElapsed, string seed, DifficultyEnums difficulty)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                var score = new ScoreModel
                {
                    UserId = user.Id,
                    TimeElapsed = timeElapsed,
                    DateAchieved = DateTime.UtcNow,
                    Seed = seed,
                    Difficulty = difficulty
                };

                _context.Scores.Add(score);
                _context.SaveChanges();
            }
        }

        public IEnumerable<ScoreModel> GetUserBestScores(string username, string seed, DifficultyEnums difficulty)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Enumerable.Empty<ScoreModel>();

            return _context.Scores
                .Where(s => s.UserId == user.Id && s.Seed == seed && s.Difficulty == difficulty)
                .OrderBy(s => s.TimeElapsed)
                .Take(10)
                .ToList();
        }

        public IEnumerable<ScoreModel> GetGlobalBestScores(string seed, DifficultyEnums difficulty)
        {
            return _context.Scores
                .Where(s => s.Seed == seed && s.Difficulty == difficulty)
                .OrderBy(s => s.TimeElapsed)
                .Take(10)
                .ToList();
        }

        public IEnumerable<ScoreModel> GetAllUserScores(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Enumerable.Empty<ScoreModel>();

            return _context.Scores
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.DateAchieved)
                .ToList();
        }
        public void DeleteScoresBySeedAndDifficulty(string username, string seed, DifficultyEnums difficulty)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return;

            var scoresToDelete = _context.Scores
                .Where(s => s.UserId == user.Id && s.Seed == seed && s.Difficulty == difficulty);

            _context.Scores.RemoveRange(scoresToDelete);
            _context.SaveChanges();
        }

        public bool UpdateUsername(string oldUsername, string newUsername)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == oldUsername);
            if (user == null)
                return false; // użytkownik nie znaleziony

            // Sprawdź, czy nowy login nie jest już zajęty
            if (_context.Users.Any(u => u.Username == newUsername))
                return false; // nowy login jest zajęty

            user.Username = newUsername;
            _context.SaveChanges();
            return true;
        }

    }

}



