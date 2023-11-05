using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheGameOfLife.Models;
using TheGameOfLife.Models.Structs;
using TheGameOfLife.Utils;
using static TheGameOfLife.Models.Structs.Structs;
using EvolutionResults = TheGameOfLife.Models.Structs.Structs.EvolutionResults;

namespace TheGameOfLife.Managers
{
    public class EvolutionManager
    {
        private PastGenerations PastGenerations { get; set; }
        private Generation CurrentGen { get; set; }
        public EvolutionResults Results { get; set; }
        public GameSettings GameSettings { get; set; }
        public int CurrentGenNumber { get; private set; }

        public EvolutionManager(Generation currentGen)
        {
            CurrentGen = currentGen;
            GameSettings = currentGen.GameSettings;
            CurrentGenNumber = 1;
            PastGenerations = new PastGenerations();

            Results = new EvolutionResults(CurrentGenNumber, 0, 0, 0);
        }

        public string SaveGame(string path)
        {
            return SaveFileHandler.WriteToJsonFile(path, LastGen());
        }

        public void SaveGameToPNG(string path)
        {
            SaveFileHandler.WriteToPNG(path, LastGen());
        }

        #region Game commands

        public void Evolve()
        {
            GameSettings = CurrentGen.GameSettings;

            // if the game just started, fill the stats
            if (CurrentGenNumber == 1) Results = new EvolutionResults(CurrentGenNumber, CurrentGen.AliveCellsCount, 0, 0);

            List<Tuple<uint, uint, int>> updatedCells = new();
            List<Tuple<uint, uint, int>> allCells = new();

            int births = 0, deaths = 0;

            for (uint y = 0; y< CurrentGen.MapSize.SizeY; y++)
            {
                for (uint x = 0; x < CurrentGen.MapSize.SizeX; x++)
                {
                    // get current cell and add it to the list of all cells (to save evolution later)
                    Cell cell = CurrentGen.GetCell(x, y);
                    allCells.Add(new Tuple<uint, uint, int> (x, y, cell.Alive ));

                    // count alive neighbours of the cell
                    uint count = GetAliveNeighboursCount(cell);

                    // deaths on alive cells according to set under and over population thresholds
                    if (cell.Alive >=2 && (count < GameSettings.MinPop || count > GameSettings.MaxPop))
                    {
                        updatedCells.Add(new Tuple<uint, uint, int>(x, y, 1));
                        deaths++;
                        continue;
                    }

                    // births of new cells according to set reproduce population treshold
                    if (cell.Alive < 2 && count == GameSettings.ReproducePop)
                    {
                        updatedCells.Add(new Tuple<uint, uint, int>(x, y, 3));
                        births++;
                        continue;
                    }

                    if (cell.Alive == 3) updatedCells.Add(new Tuple<uint, uint, int>(x, y, 2));
                    if (cell.Alive == 1) updatedCells.Add(new Tuple<uint, uint, int>(x, y, 0));
                }
            }

            if (updatedCells.Any())
            {
                // add generation to past generations
                AddGen(new Structs.Evolution(Results, allCells, GameSettings, CurrentGen.MapSize));

                // update all cells changed by the game rules
                Parallel.ForEach(updatedCells, x => CurrentGen.SetCell(x.Item1, x.Item2, x.Item3));

                // update current stats of the game
                Results = new EvolutionResults(CurrentGenNumber++, CurrentGen.AliveCellsCount, births, deaths);
            }
        }

        public void Devolve()
        {
            // load and remove last evolution
            Evolution previousEvolution = LastGen();
            RemoveGen(previousEvolution);

            // set all stats and settings
            Results = previousEvolution.Parameters;
            CurrentGenNumber = Results.GenerationNumber;
            UpdateGameSettings(previousEvolution.GameSettings, Results.CellCount);
            GameSettings = previousEvolution.GameSettings;

            // set cells on grid
            Parallel.ForEach(previousEvolution.Cells, x => CurrentGen.SetCell(x.Item1, x.Item2, x.Item3));
        }

        public void ResetGen()
        {
            // clear all cells
            CurrentGen.Clear();

            // clear stats
            CurrentGenNumber = 1;
            Results = new EvolutionResults(CurrentGenNumber, 0, 0, 0);

            // make a new list of past generations
            PastGenerations = new PastGenerations();   
        }

        #endregion Game commands

        #region Utils
        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop, int aliveCellCount)
        {
            CurrentGen.UpdateGameSettings(minPop, maxPop, reproducePop, aliveCellCount);
        }
        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop)
        {
            CurrentGen.UpdateGameSettings(minPop, maxPop, reproducePop);
        }
        public void UpdateGameSettings(GameSettings gameSettings, int aliveCellCount)
        {
            CurrentGen.UpdateGameSettings(gameSettings, aliveCellCount);
        }
        public void UpdateGameSettings(GameSettings gameSettings)
        {
            CurrentGen.UpdateGameSettings(gameSettings);
        }
        #endregion Utils

        #region Cell utils
        public uint GetSizeX() { return CurrentGen.MapSize.SizeX; }
        public uint GetSizeY() { return CurrentGen.MapSize.SizeY; }

        public Cell GetCell(uint x, uint y) { return CurrentGen.GetCell(x, y); }
        public void SetCell(uint x, uint y, int alive) { CurrentGen.SetCell(x, y, alive); }
        public int ToggleCell(uint x, uint y) { CurrentGen.ToggleAlive(x, y);  return CurrentGen.AliveCellsCount; }

        private uint GetAliveNeighboursCount(Cell cell)
        {
            uint count = 0;

            List<Cell> cells = new List<Cell>
            {
                CurrentGen.GetCell(cell.X - 1, cell.Y - 1), // -1, -1
                CurrentGen.GetCell(cell.X - 1, cell.Y), // -1, 0
                CurrentGen.GetCell(cell.X - 1, cell.Y + 1), // -1, 1
                CurrentGen.GetCell(cell.X, cell.Y + 1), // 0, 1
                CurrentGen.GetCell(cell.X, cell.Y - 1), // 0, -1
                CurrentGen.GetCell(cell.X + 1, cell.Y - 1), // 1, -1
                CurrentGen.GetCell(cell.X + 1, cell.Y), // 1, 0
                CurrentGen.GetCell(cell.X + 1, cell.Y + 1), // 1, 1
            };

            cells.ForEach(cell => count += (cell != null && cell.Alive >= 2) ? (uint)1 : (uint)0);
            return count;
        }
        #endregion Cell utils

        #region Generations utils
        public bool IsGensEmpty() { return PastGenerations.IsEmpty(); }
        public void AddGen(Evolution evolution) { PastGenerations.Add(evolution); }
        public bool RemoveGen(Evolution evolution) { return PastGenerations.Remove(evolution); }
        public Evolution LastGen() { return PastGenerations.GetLast(); }
        #endregion Generations utils

    }
}
