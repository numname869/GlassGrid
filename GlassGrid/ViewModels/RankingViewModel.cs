using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GlassGrid.Models;
using GlassGrid.Models.Enums;

using GlassGrid.ViewModels;

public class RankingViewModel : BaseViewModel
{
    private readonly IEFUserService _userService;
    private readonly string _currentUsername;

  
    public string FilterSeed1 { get; set; }
    public DifficultyEnums? FilterDifficulty1 { get; set; }

    public string FilterSeed2 { get; set; }
    public DifficultyEnums? FilterDifficulty2 { get; set; }

    public ObservableCollection<ScoreModel> UserBestScores { get; set; } = new();
    public ObservableCollection<ScoreModel> GlobalBestScores { get; set; } = new();
    public ObservableCollection<ScoreModel> AllUserScores { get; set; } = new();

    public IEnumerable<DifficultyEnums> Difficulties => Enum.GetValues(typeof(DifficultyEnums)).Cast<DifficultyEnums>();

    public ICommand LoadUserBestScoresCommand { get; }
    public ICommand LoadGlobalBestScoresCommand { get; }
    public ICommand LoadAllUserScoresCommand { get; }

    public RankingViewModel(IEFUserService userService, string currentUsername)
    {
        _userService = userService;
        _currentUsername = currentUsername;

        LoadUserBestScoresCommand = new RelayCommand(LoadUserBestScores);
        LoadGlobalBestScoresCommand = new RelayCommand(LoadGlobalBestScores);
        LoadAllUserScoresCommand = new RelayCommand(LoadAllUserScores);
    }

    private void LoadUserBestScores()
    {
        if (string.IsNullOrWhiteSpace(FilterSeed1) || FilterDifficulty1 == null) return;

        var results = _userService.GetUserBestScores(_currentUsername, FilterSeed1, FilterDifficulty1.Value);
        UserBestScores.Clear();
        foreach (var r in results)
            UserBestScores.Add(r);
    }

    private void LoadGlobalBestScores()
    {
        if (string.IsNullOrWhiteSpace(FilterSeed2) || FilterDifficulty2 == null) return;

        var results = _userService.GetGlobalBestScores(FilterSeed2, FilterDifficulty2.Value);
        GlobalBestScores.Clear();
        foreach (var r in results)
            GlobalBestScores.Add(r);
    }

    private void LoadAllUserScores()
    {
        var results = _userService.GetAllUserScores(_currentUsername);
        AllUserScores.Clear();
        foreach (var r in results)
            AllUserScores.Add(r);
    }

    public ICommand DeleteScoresBySeedAndDifficultyCommand => new RelayCommand(DeleteScoresBySeedAndDifficulty, CanDeleteScores);
    public string DeleteSeed { get; set; }
    public DifficultyEnums? DeleteDifficulty { get; set; }

    private bool CanDeleteScores()
    {
        return !string.IsNullOrWhiteSpace(DeleteSeed) && DeleteDifficulty.HasValue;
    }

    private void DeleteScoresBySeedAndDifficulty()
    {
        _userService.DeleteScoresBySeedAndDifficulty(_currentUsername, DeleteSeed, DeleteDifficulty.Value);
        LoadAllUserScores();
    }

}
