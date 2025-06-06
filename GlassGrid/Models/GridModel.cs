using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using GlassGrid.Models.Enums;
using GlassGrid.Views;
using GlassGrid.ViewModels;
using System.Windows.Media;
using GlassGrid.Models.Data;


namespace GlassGrid.Models
{
    public class GridModel : BaseViewModel
    {



        private int _userAnswer;
        private List<(int, Color)> _cells;
        private bool _isGridComplete;
        private string _seed;
        DifficultyEnums _difficulty;




        internal GridModel(DifficultyEnums difficulty, string seed)
        {

            _isGridComplete = false;
            _difficulty = difficulty;
            _seed = seed;
            GenerateGrid();

        }

        internal GridModel(List<(int, Color)> cells)
        {
            _cells = cells;
            _isGridComplete = false;
        }

        public string Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                OnPropertyChanged(Seed);
            }
        }

        public List<(int, Color)> Cells
        {
            get => _cells;
            set
            {
                if (_cells != value)
                {
                    _cells = value;
                    OnPropertyChanged(nameof(Cells));
                }
            }
        }

        public bool IsGridComplete
        {
            get => _isGridComplete;
            set
            {
                if (_isGridComplete != value)
                {
                    _isGridComplete = value;
                    OnPropertyChanged(nameof(IsGridComplete));
                }
            }
        }

        public List<(int, Color)> GridData
        {
            get => _cells;
            set
            {
                if (_cells != value)
                {
                    _cells = value;
                    OnPropertyChanged(nameof(GridData));
                }
            }
        }

        public int UserAnswer
        {
            get => _userAnswer;
            set
            {
                if (_userAnswer != value)
                {
                    _userAnswer = value;
                    OnPropertyChanged(nameof(UserAnswer));
                }
            }
        }


        public string CheckColor(Color color)
        {

            double r_norm = color.R / 255.0;
            double g_norm = color.G / 255.0;
            double b_norm = color.B / 255.0;


            double score = r_norm - b_norm;


            if (score > 0.1)
                return "warm";
            else if (score < -0.1)
                return "cold";
            else
                return "neutral";
        }


        public bool CheckNumber(int number, Color color)
        {
            string value = CheckColor(color);

            if (value == "warm" && number % 2 != 0) return true;
            if (value == "cold" && number % 2 == 0) return true;
            if (value == "neutral" && number == 0) return true;

            return false;

        }
        public bool CheckColorGroups()
        {
            if (Cells == null || !Cells.Any())
                return false;

            // Kolor → liczba
            Dictionary<Color, int> colorToNumber = new();
            // Liczba → kolor
            Dictionary<int, Color> numberToColor = new();

            foreach (var (number, color) in Cells)
            {
                if (!CheckNumber(number, color))
                    return false; // sprawdzenie czy liczba jest poprawna dla danego koloru


                if (colorToNumber.TryGetValue(color, out var existingNumber))
                {

                    if (existingNumber != number)
                        return false; // kolor ma różne liczby
                }
                else
                {
                    colorToNumber[color] = number;
                }

                if (numberToColor.TryGetValue(number, out var existingColor))
                {
                    if (existingColor != color)
                        return false; // liczba przypisana do innego koloru
                }
                else
                {
                    numberToColor[number] = color;
                }
            }

            return true;
        }


        int GetIntSeedFromString(string seed)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in seed)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        private int GetGridSizeByDifficulty(DifficultyEnums difficulty)
        {
            return difficulty switch
            {
                DifficultyEnums.easy => 3 * 3,
                DifficultyEnums.medium => 3 * 4,
                DifficultyEnums.hard => 5 * 3,
                _ => 3 * 3
            };
        }


        public void GenerateGrid()
        {
            int gridSize = GetGridSizeByDifficulty(_difficulty);
            int numericSeed = GetIntSeedFromString(_seed);
            Random rng = new Random(numericSeed);

          
            int groupCount = _difficulty switch
            {
                DifficultyEnums.easy => 2,
                DifficultyEnums.medium => 4,
                DifficultyEnums.hard => 6,
                _ => 2
            };


            int colorVariance = _difficulty switch
            {
                DifficultyEnums.easy => 100,   
                DifficultyEnums.medium => 60,  
                DifficultyEnums.hard => 30,   
                _ => 80
            };

         
            List<Color> groupColors = new();
            for (int i = 0; i < groupCount; i++)
            {
                byte baseR = (byte)rng.Next(0, 256 - colorVariance);
                byte baseG = (byte)rng.Next(0, 256 - colorVariance);
                byte baseB = (byte)rng.Next(0, 256 - colorVariance);

                Color color = Color.FromRgb(
                    (byte)(baseR + rng.Next(colorVariance)),
                    (byte)(baseG + rng.Next(colorVariance)),
                    (byte)(baseB + rng.Next(colorVariance))
                );

                groupColors.Add(color);
            }

            
            var cells = new List<(int, Color)>();
            for (int i = 0; i < gridSize; i++)
            {
                Color color = groupColors[rng.Next(groupColors.Count)];
                cells.Add((0, color));
            }

            Cells = cells.OrderBy(_ => rng.Next()).ToList();
        }


    }
}