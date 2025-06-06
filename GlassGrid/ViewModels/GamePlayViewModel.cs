using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GlassGrid.Models;
using GlassGrid.Models.Enums;
using GlassGrid.Models.Data;
using GlassGrid.Views;
using Microsoft.EntityFrameworkCore;


namespace GlassGrid.ViewModels
{
    public class GamePlayViewModel : BaseViewModel
    {
        private Dispatcher _dispatcher;
        private string _currentseed;
        private GridModel _currentGrid;
        private ObservableCollection<CellViewModel> _gridCells;
        private string _user = "Guest";
        private DifficultyEnums _currentDifficulty;
    
        public IEnumerable<DifficultyEnums> Difficulties => Enum.GetValues(typeof(DifficultyEnums)).Cast<DifficultyEnums>();

        private readonly IEFUserService _userService;
        private readonly GlassGridContext _context;

        public GamePlayViewModel(IEFUserService userService, GlassGridContext context)
        {
            _userService = userService;
            _context = context;
        }


        public DifficultyEnums CurrentDifficulty
        {
            get => _currentDifficulty;
            set
            {
                if (_currentDifficulty != value)
                {
                    _currentDifficulty = value;
                    OnPropertyChanged(nameof(CurrentDifficulty));
                }
            }
        }
        public string Seed
        {
            get => _currentseed;
            set
            {
                if (_currentseed != value)
                {
                    _currentseed = value;
                    OnPropertyChanged(nameof(Seed));
                }
            }
        }
        public string CurrentUser
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged(nameof(CurrentUser));
                }
            }
        }

        #region . Grid
        public SolidColorBrush FilterColor
        {
            get => _gridCells[0].FilterColor;
        }

        public GridModel CurrentGrid
        {
            get => _currentGrid;
            set
            {
                if (_currentGrid != value)
                {
                    _currentGrid = value;
                    OnPropertyChanged(nameof(CurrentGrid));
                    UpdateGridCells(_currentGrid);
                }
            }
        }

        public ObservableCollection<CellViewModel> GridCells
        {
            get => _gridCells;
            set
            {
                _gridCells = value;
                OnPropertyChanged(nameof(GridCells));
            }
        }

        public void UpdateGridCells(GridModel grid)
        {
            var list = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < grid.Cells.Count; i++)
            {
                var (number, color) = grid.Cells[i];
                var cell = new CellViewModel(number, color, i);
                list.Add(cell);
            }

            GridCells = list;
            OnPropertyChanged(nameof(GridCells));
            OnPropertyChanged(nameof(FilterColor));

            StartTimer();
            GameStatus = "Game in progress";
        }


        #endregion


        #region Commands

        public ICommand ChangeFilterColorCommand => new RelayCommand(ChangeFilterForAllCells, () => true);
        public ICommand OpenNewGameWindowCommand => new RelayCommand( () =>
            {
                var newGameWindow = new NewGame();
                newGameWindow.ShowDialog();
            }, () => true);


        public ICommand OpenLoginWindowCommand => new RelayCommand(() =>
        {
            var loginWindow = new LoginWindow(_userService);

        
            loginWindow.UserLoggedIn += username =>
            {
                CurrentUser = username;
            };

            loginWindow.ShowDialog();
        });




        public ICommand GenerateSeedForNewGame => new RelayCommand(() => GenerateSeed(), () => true);

        public ICommand CellCLicked => new RelayCommand<CellViewModel>(OneCellClicked);
        public ICommand NewGame => new RelayCommand(() =>
        {
            CurrentGrid = new GridModel(CurrentDifficulty, Seed);
            UpdateGridCells(CurrentGrid);


        });

        public ICommand OpenRankingWindowCommand => new RelayCommand(() =>
        {
            var rankingWindow = new RankingWindow(_userService, CurrentUser);
            rankingWindow.ShowDialog();
        });



        public void GenerateSeed()
        {
            string seed = Guid.NewGuid().ToString();
            Seed = seed;
          
        }
        private void ToggleFilter()
        {
            foreach (var cell in GridCells)
            {
                cell.FilterColorEnabled = !cell.FilterColorEnabled;
            }
            OnPropertyChanged(nameof(GridCells));
        }

        private void ChangeFilterForAllCells()
        {
            foreach (var cell in GridCells)
            {
                cell.ChangeFilterColor();
            }
            OnPropertyChanged(nameof(FilterColor));
        }


        private async void OneCellClicked(object obj)
        {
            if (obj is CellViewModel cell)
            {
                cell.Number = (cell.Number + 1) % 10;
                var index = cell.IndexInGrid;
                var oldColor = CurrentGrid.Cells[index].Item2;
                CurrentGrid.Cells[index] = (cell.Number, oldColor);

                CurrentGrid.IsGridComplete = CurrentGrid.CheckColorGroups();
                if (CurrentGrid.IsGridComplete)
                {
                    StopTimer();
                    GameStatus = "Grid is complete!";

                    var elapsedTime = TimeSpan.Parse(TimerText);
                   _userService.SaveScore(CurrentUser, elapsedTime, Seed, CurrentDifficulty);
                }
            }
        }




        #endregion



        #region . Timer

        private DispatcherTimer _timer;
        private DateTime _startTime;
        private string _timerText = "00:00";
        private string _gameStatus = "Game in progress";

        public string TimerText
        {
            get => _timerText;
            set
            {
                _timerText = value;
                OnPropertyChanged(nameof(TimerText));
            }
        }

        public string GameStatus
        {
            get => _gameStatus;
            set
            {
                _gameStatus = value;
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        private void StartTimer()
        {
            _startTime = DateTime.Now;

            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += (s, e) =>
                {
                    var elapsed = DateTime.Now - _startTime;
                    TimerText = elapsed.ToString(@"mm\:ss");
                };
            }

            _timer.Start();
        }

        private void StopTimer()
        {
            _timer?.Stop();
        }

        #endregion 
    }
}
