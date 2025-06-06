using System;
using GlassGrid.Models.Enums;
using GlassGrid.Models.Data;

namespace GlassGrid.Models
{
    public class ScoreModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public DateTime DateAchieved { get; set; }
        public string Seed { get; set; } = string.Empty;
        public DifficultyEnums Difficulty { get; set; }

        public UserModel User { get; set; }  // nawigacja do użytkownika
    }
}
