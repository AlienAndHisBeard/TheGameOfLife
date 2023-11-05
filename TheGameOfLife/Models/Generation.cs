using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        public void SetCell(uint x, uint y, bool alive) 
        {
            Cell cell = GetCell(x, y) ?? throw new ArgumentOutOfRangeException("Out of map");
            if (alive) AliveCellsCount++;
            else AliveCellsCount--;

            cell.Alive = alive;
        }

        public void ToggleAlive(uint x, uint y)
        {
            Cell cell = GetCell(x, y) ?? throw new ArgumentOutOfRangeException("Out of map");

            if (!cell.Alive) AliveCellsCount++;
            else AliveCellsCount--;

            cell.Alive = !cell.Alive;
        }

        private void Init()
        {
            for (uint y = 0; y < MapSize.SizeY; y++) 
            {
                for (uint x = 0; x < MapSize.SizeX; x++)
                {
                    gameMap[x, y] = new Cell(x, y, false);
                }
            }
        }

        private void Init(List<Tuple<uint, uint, bool>> cells)
        {
            foreach( var cell in cells)
            {
                if (uint.MaxValue == cell.Item1 || uint.MaxValue == cell.Item2) throw new Exception("Wrong save file");
                if (cell.Item1 >= MapSize.SizeX || cell.Item2 >= MapSize.SizeY) throw new Exception("Wrong save file");
                gameMap[cell.Item1, cell.Item2] = new Cell(cell.Item1, cell.Item2, cell.Item3);
            }
        }

        public void Clear()
        {
            for (uint y = 0; y < MapSize.SizeY; y++)
            {
                for (uint x = 0; x < MapSize.SizeX; x++)
                {
                    SetCell(x, y, false);
                }
            }
            AliveCellsCount = 0;
        }

        public void Load(EvolutionResults res, GameSettings gameSettings)
        {
            AliveCellsCount = res.CellCount;
            GameSettings = gameSettings;
        }

        public void UpdateGameSettings(uint minPop, uint maxPop, uint reproducePop)
        {
            GameSettings.MinPop = minPop;
            GameSettings.MaxPop = maxPop;
            GameSettings.ReproducePop = reproducePop;
        }

        public void UpdateGameSettings(GameSettings gameSettings)
        {
            GameSettings = gameSettings;
        }
    }
}
