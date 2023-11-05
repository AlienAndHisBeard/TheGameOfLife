using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        private Generation CurrentGen {  get; set; }
        private PastGenerations Generations { get; set; }

        public EvolutionResults Results { get; set; }
        public GameSettings GameSettings { get; set; }
        public int CurrentGenNumber { get; private set; }

        public EvolutionManager(Generation currentGen)
        {
            CurrentGen = currentGen;
            GameSettings = currentGen.GameSettings;
            CurrentGenNumber = 1;
            Generations = new PastGenerations();

            Results = new EvolutionResults(CurrentGenNumber, 0, 0, 0);
        }

        public void SaveGame(string path)
        {
            SaveFileHandler.WriteToJsonFile(path, LastGen());
        }

        #region Game commands

        public void Evolve()
        {
            GameSettings = CurrentGen.GameSettings;

            if (CurrentGenNumber == 1)
            {
                Results = new EvolutionResults(CurrentGenNumber, CurrentGen.AliveCellsCount, 0, 0);
            }

            List<Tuple<uint, uint, bool>> updatedCells = new();
            List<Tuple<uint, uint, bool>> allCells = new();

            int births = 0, deaths = 0;

            for (uint y = 0; y< CurrentGen.MapSize.SizeY; y++)
            {
                for (uint x = 0; x < CurrentGen.MapSize.SizeX; x++)
                {
                    Cell cell = CurrentGen.GetCell(x, y);
                    allCells.Add(new Tuple<uint, uint, bool> (x, y, cell.Alive ));
                    uint count = GetAliveNeighboursCount(cell);

                    if (cell.Alive && (count < GameSettings.MinPop || count > GameSettings.MaxPop))
                    {
                        updatedCells.Add(new Tuple<uint, uint, bool>(x, y, false));
                        deaths++;
                    }

                    if (!cell.Alive && count == GameSettings.ReproducePop)
                    {
                        updatedCells.Add(new Tuple<uint, uint, bool>(x, y, true));
                        births++;
                    }
                }
            }

            if (updatedCells.Any())
            {
                AddGen(new Structs.Evolution(Results, allCells, GameSettings, CurrentGen.MapSize));
                Parallel.ForEach(updatedCells, x => CurrentGen.SetCell(x.Item1, x.Item2, x.Item3));
                CurrentGenNumber++;
                Results = new EvolutionResults(CurrentGenNumber, CurrentGen.AliveCellsCount, births, deaths);
            }
        }

        public void Devolve()
        {
            Evolution previousEvolution = LastGen();
            RemoveGen(previousEvolution);
            Results = previousEvolution.Parameters;
            GameSettings = previousEvolution.GameSettings;

            List<Tuple<uint, uint, bool>> allCells = previousEvolution.Cells;

            Parallel.ForEach(allCells, x => CurrentGen.SetCell(x.Item1, x.Item2, x.Item3));
            CurrentGen.Load(Results, GameSettings);
            CurrentGenNumber = Results.GenerationNumber;
        }

        public void ResetGen()
        {
            CurrentGen.Clear();
            CurrentGenNumber = 1;
            Generations = new PastGenerations();
            Results = new EvolutionResults(CurrentGenNumber, 0, 0, 0);
        }

        #endregion Game commands

        #region Utils
        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop)
        {
            CurrentGen.UpdateGameSettings(minPop, maxPop, reproducePop);
        }

        public void UpdateGameSettings(GameSettings gameSettings)
        {
            CurrentGen.UpdateGameSettings(gameSettings);
        }
        #endregion Utils

        #region Cell utils
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

            cells.ForEach(cell => count += (cell != null && cell.Alive) ? (uint)1 : (uint)0);
            return count;
        }

        public uint GetSizeX() { return CurrentGen.MapSize.SizeX; }
        public uint GetSizeY() { return CurrentGen.MapSize.SizeY; }

        public Cell GetCell(uint x, uint y) { return CurrentGen.GetCell(x, y); }
        public void SetCell(uint x, uint y, bool alive) { CurrentGen.SetCell(x, y, alive); }
        public int ToggleCell(uint x, uint y) { CurrentGen.ToggleAlive(x, y);  return CurrentGen.AliveCellsCount; }

        #endregion Cell utils

        #region Generations utils
        public bool IsGensEmpty() { return Generations.IsEmpty(); }
        public void AddGen(Evolution evolution) { Generations.Add(evolution); }
        public bool RemoveGen(Evolution evolution) { return Generations.Remove(evolution); }
        public Evolution LastGen() { return Generations.GetLast(); }
        #endregion Generations utils

    }
}
