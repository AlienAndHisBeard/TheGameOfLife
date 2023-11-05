using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using TheGameOfLife.Managers;
using TheGameOfLife.Models;
using TheGameOfLife.Utils;
using static TheGameOfLife.Models.Structs.Structs;

namespace TheGameOfLife.ViewModels
{
    public class GenerationViewModel : INotifyPropertyChanged
    {
        private static readonly string saveFilePath = "./save.json";
        public event PropertyChangedEventHandler? PropertyChanged;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private Task task;

        protected void OnPropertyChanged([CallerMemberName] string memberName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }

        private EvolutionManager? _manager;

        private uint gamesizeX;
        public uint GameSizeX { get { return gamesizeX; } private set { gamesizeX = value; OnPropertyChanged(); } }

        private uint gamesizeY;
        public uint GameSizeY { get { return gamesizeY; } private set { gamesizeY = value; OnPropertyChanged(); } }

        #region Display Bindings
        private int cellCount;
        public int CellCount { get { return cellCount; } private set { cellCount = value; OnPropertyChanged(); } }


        private int genNumber;
        public int GenNumber { get {  return genNumber; } private set {  genNumber = value; OnPropertyChanged(); } }


        private int birthCount;
        public int BirthCount { get { return birthCount; } private set { birthCount = value; OnPropertyChanged(); } }

        private int deathCount;
        public int DeathCount { get { return deathCount; } private set { deathCount = value; OnPropertyChanged(); } }

        private bool busy;
        public bool Busy { get { return busy; } private set { busy = value; OnPropertyChanged(); } }

        private bool gameOn = false;
        public bool GameOn { get { return gameOn; } private set {  gameOn = value; OnPropertyChanged(); } }
        #endregion Display Bindings

        #region Params Bindings
        private uint sizeX = 10;
        public uint SizeX { get {  return sizeX; }  set { sizeX = value; OnPropertyChanged(); } }

        private uint sizeY = 10;
        public uint SizeY { get { return sizeY; }  set { sizeY = value; OnPropertyChanged(); } }

        private uint minPop = 2;
        public uint MinPop { get { return minPop; }  set { minPop = value; OnPropertyChanged(); } }

        private uint maxPop = 3;
        public uint MaxPop { get { return maxPop; }  set { maxPop = value; OnPropertyChanged(); } }

        private uint reproducePop = 3;
        public uint ReproducePop { get { return reproducePop; }  set { reproducePop = value; OnPropertyChanged(); } }

        #endregion

        #region Commands
        public RelayCommand<object> DevolveCommand { get; private set; }
        public RelayCommand<object> EvolveCommand { get; private set; }
        public RelayCommand<string> ToggleCellCommand { get; private set; }
        public RelayCommand<object> ResetCommand { get; private set; }
        public RelayCommand<object> StartEvolvingCommand { get; private set; }
        public RelayCommand<object> StopEvolvingCommand { get; private set; }
        public RelayCommand<object> NewGameCommand { get; private set; }
        public RelayCommand<object> SaveCommand { get; private set; }
        public RelayCommand<object> LoadCommand { get; private set; }


        #endregion Commands

        public GenerationViewModel() 
        {
            task = new(() => { });
            
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            DevolveCommand = new RelayCommand<object>(_ => Devolve(), _ => CanDevolve());
            EvolveCommand = new RelayCommand<object>(_ => Evolve(), _ => CanEvolve());
            ToggleCellCommand = new RelayCommand<string>((cellGrid) => ToggleCell(cellGrid), _ => CanToggle());
            ResetCommand = new RelayCommand<object>(_ => ResetGame(), _ => CanReset());
            StartEvolvingCommand = new RelayCommand<object>(_ => StartEvolving(), _ => CanStartEvolving());
            StopEvolvingCommand = new RelayCommand<object>(_ => StopEvolving(), _ => CanStopEvolving());
            NewGameCommand = new RelayCommand<object>(_ => CreateNewGame(), _ => CanCreateNewGame());
            LoadCommand = new RelayCommand<object>(_ => LoadGame(), _ => CanLoadGame());
            SaveCommand = new RelayCommand<object>(_ => SaveGame(), _ => CanSaveGame());

        }

        #region Save and Load commands
        private void SaveGame()
        {
            Busy = true;
            _manager.SaveGame(saveFilePath);
            Busy = false;
        }

        private bool CanSaveGame()
        {
            return CellCount > 0 && !task.Status.Equals(TaskStatus.Running) && _manager != null;
        }
        private void LoadGame()
        {
            Busy = true;
            Evolution evolution = SaveFileHandler.ReadFromJsonFile(saveFilePath);
            _manager = new EvolutionManager(new Generation(evolution));
            StopEvolving();
            _manager.Results = evolution.Parameters;
            GenNumber = evolution.Parameters.GenerationNumber;
            UpdateResults();
            GameSizeX = _manager.GetSizeX();
            GameSizeY = _manager.GetSizeY();
            BuildGrid(ClearGameGrid());
            GameOn = true;
            Busy = false;
        }

        private bool CanLoadGame()
        {
            return !task.Status.Equals(TaskStatus.Running);
        }
        #endregion Save and Load commands

        #region Game flow commands
        public void StartEvolving()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            task = Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        Busy = true;
                        break;
                    }

                    Evolve();

                    Thread.Sleep(1000);
                }
            }, cancellationTokenSource.Token);
        }
        private bool CanStartEvolving()
        {
            return CellCount > 0 && !task.Status.Equals(TaskStatus.Running) && !Busy && _manager != null;
        }

        public async void StopEvolving()
        {
            cancellationTokenSource.Cancel();
            CommandManager.InvalidateRequerySuggested();
            await task;
            task.Dispose();
            
            Busy = false;
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanStopEvolving()
        {
            return task.Status.Equals(TaskStatus.Running) && !Busy && _manager != null;
        }

        private void Devolve()
        {
            StopEvolving();

            _manager.Devolve();

            UpdateResults();
            UpdateGameSettings();
        }

        private bool CanDevolve()
        {
            return _manager != null && !_manager.IsGensEmpty();
        }

        private void Evolve()
        {
            UpdateGameSettingsInManager();
            _manager.Evolve();
            UpdateResults();
        }

        private bool CanEvolve()
        {
            return CellCount > 0 && !Busy && !task.Status.Equals(TaskStatus.Running) && _manager!= null;
        }

        private void ResetGame()
        {
            StopEvolving();
            _manager.ResetGen();
            UpdateResults();
        }

        private bool CanReset()
        {
            return GenNumber > 1 || CellCount > 0 && _manager != null;
        }
        #endregion Game flow commands

        #region Cell grid commands
        private void ToggleCell(string cellInGrid)
        {
            string[] gridSplit = cellInGrid.Split(',');

            uint x = uint.Parse(gridSplit[0]);
            uint y = uint.Parse(gridSplit[1]);

            CellCount = _manager.ToggleCell(x, y);
        }

        private bool CanToggle()
        {
            return _manager != null;
        }
        #endregion Cell grid commands

        #region New game
        public void CreateNewGame()
        {
            StopEvolving();

            GameSettings gameSettings = new(MinPop, MaxPop, ReproducePop);
            _manager = new EvolutionManager(new Generation(gameSettings, new MapSize(SizeX, SizeY)));
            GenNumber = _manager.CurrentGenNumber;
            GameSizeX = _manager.GetSizeX();
            GameSizeY = _manager.GetSizeY();

            BuildGrid(ClearGameGrid());
            GameOn = true;
        }

        public bool CanCreateNewGame()
        {
            return !Busy;
        }

        private void BuildGrid(Grid GameGrid)
        {
            for (uint y = 0; y < GameSizeY; y++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                
                for (uint x = 0; x < GameSizeX; x++)
                {
                    if (y == 0) GameGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    TextBlock cell = CreateCellTB(GetCell(x, y));

                    Grid.SetRow(cell, (int)y);
                    Grid.SetColumn(cell, (int)x);

                    GameGrid.Children.Add(cell);
                }
            }
        }

        private TextBlock CreateCellTB(Cell cell)
        {

            InputBinding cmdBind = new(ToggleCellCommand, new MouseGesture(MouseAction.LeftClick))
            {
                CommandParameter = $"{cell.X},{cell.Y}"
            };

            TextBlock textBlock = new() { DataContext = cell };
            textBlock.InputBindings.Add(cmdBind);

            Binding binding = new()
            {
                Path = new PropertyPath("Alive"),
                Mode = BindingMode.TwoWay,
                Converter = new ColourConverter(aliveColour: Brushes.Green, deadColour: Brushes.White)
            };
            textBlock.SetBinding(TextBlock.BackgroundProperty, binding);
            return textBlock;
        }
        #endregion New game

        #region Utils
        private void UpdateResults()
        {
            GenNumber = _manager.Results.GenerationNumber;
            CellCount = _manager.Results.CellCount;
            BirthCount = _manager.Results.BirthCount;
            DeathCount = _manager.Results.DeathCount;
        }

        private void UpdateGameSettingsInManager()
        {
            if (_manager.GameSettings.ReproducePop != ReproducePop ||
                _manager.GameSettings.MaxPop != MaxPop ||
                _manager.GameSettings.MinPop != MinPop)
            {
                _manager.UpdateGameSettings(MinPop, MaxPop, ReproducePop);
            }
        }

        private void UpdateGameSettings()
        {
            MinPop = _manager.GameSettings.MinPop;
            MaxPop = _manager.GameSettings.MaxPop;
            ReproducePop = _manager.GameSettings.ReproducePop;
        }

        public Cell GetCell(uint x, uint y) { return _manager.GetCell(x, y); }

        private Grid ClearGameGrid()
        {
            if (Application.Current.MainWindow.FindName("GameGrid") is not Grid gameGrid) { throw new Exception("The view is not compatible with View Model"); }
            gameGrid.RowDefinitions.Clear();
            gameGrid.ColumnDefinitions.Clear();
            gameGrid.Children.Clear();
            return gameGrid;
        }
        #endregion Utils
    }
}
