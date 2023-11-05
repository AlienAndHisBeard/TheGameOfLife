using System;
using System.Collections.Generic;
using static TheGameOfLife.Models.Structs.Structs;

namespace TheGameOfLife.Models
{
    public class Generation
    {
        private readonly Cell[,] gameMap;

        public int AliveCellsCount { get; private set; }
        public GameSettings GameSettings { get; private set; }
        public MapSize MapSize { get; private set; }

        public Generation(GameSettings gameSettings, MapSize mapSize)
        {
            gameMap = new Cell[mapSize.SizeX, mapSize.SizeY];
            MapSize = mapSize;
            GameSettings = gameSettings;
            AliveCellsCount = 0;
            Init();
        }

        public Generation(Evolution evolution)
        {
            MapSize = evolution.MapSize;
            GameSettings = evolution.GameSettings;
            gameMap = new Cell[MapSize.SizeX, MapSize.SizeY];
            AliveCellsCount = evolution.Parameters.CellCount;
            Init(evolution.Cells);
        }

        public Cell GetCell(uint x, uint y)
        {
            if (uint.MaxValue == x || uint.MaxValue == y) return null;
            if (x >= MapSize.SizeX || y >= MapSize.SizeY) return null;
            return gameMap[x, y];
        }

        public void SetCell(uint x, uint y, int alive) 
        {
            Cell cell = GetCell(x, y) ?? throw new ArgumentOutOfRangeException("Out of map");
            if (alive == 3) AliveCellsCount++;
            else if (alive == 1) AliveCellsCount--;

            cell.Alive = alive;
        }

        public void ToggleAlive(uint x, uint y)
        {
            Cell cell = GetCell(x, y) ?? throw new ArgumentOutOfRangeException("Out of map");

            if (cell.Alive < 2 ) AliveCellsCount++;
            else AliveCellsCount--;

            if (cell.Alive >= 2) cell.Alive = 0;
            else cell.Alive = 2;
        }

        private void Init()
        {
            for (uint y = 0; y < MapSize.SizeY; y++) 
            {
                for (uint x = 0; x < MapSize.SizeX; x++)
                {
                    gameMap[x, y] = new Cell(x, y, 0);
                }
            }
        }

        private void Init(List<Tuple<uint, uint, int>> cells)
        {
            foreach( var cell in cells)
            {
                if (uint.MaxValue == cell.Item1 || uint.MaxValue == cell.Item2) 
                    throw new Exception("Wrong save file");
                if (cell.Item1 >= MapSize.SizeX || cell.Item2 >= MapSize.SizeY) 
                    throw new Exception("Wrong save file");
                gameMap[cell.Item1, cell.Item2] = new Cell(cell.Item1, cell.Item2, cell.Item3);
            }
        }

        public void Clear()
        {
            for (uint y = 0; y < MapSize.SizeY; y++)
            {
                for (uint x = 0; x < MapSize.SizeX; x++)
                {
                    SetCell(x, y, 0);
                }
            }
            AliveCellsCount = 0;
        }

        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop, int aliveCellsCount)
        {
            GameSettings.MinPop = minPop;
            GameSettings.MaxPop = maxPop;
            GameSettings.ReproducePop = reproducePop;
            AliveCellsCount = aliveCellsCount;
        }
        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop)
        {
            GameSettings.MinPop = minPop;
            GameSettings.MaxPop = maxPop;
            GameSettings.ReproducePop = reproducePop;
        }

        public void UpdateGameSettings(GameSettings gameSettings, int aliveCellsCount)
        {
            GameSettings = gameSettings;
            AliveCellsCount = aliveCellsCount;
        }
        public void UpdateGameSettings(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
        }
    }
}
