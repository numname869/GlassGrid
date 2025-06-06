using System;
using System.Collections.Generic;



namespace GlassGrid.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ScoreModel> Scores { get; set; } = new List<ScoreModel>();
    }
}
