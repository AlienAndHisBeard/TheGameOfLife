using System;
using System.Collections.Generic;

namespace TheGameOfLife.Models.Structs
{
    public class Structs
    {

        /// <summary>
        /// Stores results for a particular generation.
        /// </summary>
        [Serializable]
        public struct EvolutionResults
        {
            public int CellCount { get; set; }
            public int GenerationNumber { get; set; }
            public int DeathCount { get; set; }
            public int BirthCount { get; set; }

            /// <summary>
            /// Initialises a new instance of a EvolutionResults
            /// </summary>
            /// <param name="generationNumber"></param>
            /// <param name="cellCount"></param>
            /// <param name="birthCount"></param>
            /// <param name="deathCount"></param>
            public EvolutionResults(int generationNumber, int cellCount, int birthCount, int deathCount)
            {
                GenerationNumber = generationNumber;
                CellCount = cellCount;
                DeathCount = deathCount;
                BirthCount = birthCount;
            }
        }

        /// <summary>
        /// Stores rules of the game.
        /// </summary>
        [Serializable]
        public record GameSettings
        {
            public uint MinPop { get; set; }
            public uint MaxPop { get; set; }
            public uint ReproducePop { get; set; }

            /// <summary>
            /// Initialises a new instance of a GameSettings
            /// </summary>
            /// <param name="minPop"></param>
            /// <param name="maxPop"></param>
            /// <param name="reproducePop"></param>
            public GameSettings(uint minPop, uint maxPop, uint reproducePop)
            {
                MinPop = minPop;
                MaxPop = maxPop;
                ReproducePop = reproducePop;
            }

        }

        /// <summary>
        /// Stores dimensions of the map.
        /// </summary>
        [Serializable]
        public struct MapSize
        {
            public uint SizeX { get; set; }
            public uint SizeY { get; set; }

            /// <summary>
            /// Initialises a new instance of a MapSize
            /// </summary>
            /// <param name="sizeX"></param>
            /// <param name="sizeY"></param>
            public MapSize(uint sizeX, uint sizeY) 
            {  
                SizeX = sizeX; 
                SizeY = sizeY; 
            }
        }

        /// <summary>
        /// Stores all data needed to store a generation.
        /// </summary>
        [Serializable]
        public struct Evolution 
        {

            /// <summary>
            /// A struct representing results for the current generation
            /// </summary>
            public EvolutionResults Parameters { get; set; }

            /// <summary>
            /// A record representing rules of the game/current generation
            /// </summary>
            public GameSettings GameSettings { get; set; }

            /// <summary>
            /// A struct representing map dimensions
            /// </summary>
            public MapSize MapSize { get; set; }

            /// <summary>
            /// List of tuples representing a single cell
            /// </summary>
            public List<Tuple<uint, uint, int>> Cells { get; set; }

            /// <summary>
            /// Initialises a new instance of a Evolution
            /// </summary>
            /// <param name="parameters"></param>
            /// <param name="cells">List of tuples representing a single cell</param>
            /// <param name="gameSettings"></param>
            /// <param name="mapSize"></param>
            public Evolution(EvolutionResults parameters, List<Tuple<uint, uint, int>> cells, GameSettings gameSettings, MapSize mapSize)
            {
                Parameters = parameters;
                Cells = cells;
                GameSettings = gameSettings;
                MapSize = mapSize;
            }
        }
    }
}
