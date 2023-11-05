using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static TheGameOfLife.Models.Structs.Structs;

namespace TheGameOfLife.Utils
{
    public class SaveFileHandler
    {
        private static readonly uint _maxMapSizeX = 100;
        private static readonly uint _maxMapSizeY = 100;
        private static readonly uint _minMapSizeX = 5;
        private static readonly uint _minMapSizeY = 5;
        private static readonly uint _minMinPop = 0;
        private static readonly uint _minMaxPop = 1;
        private static readonly uint _minRepPop = 1;
        private static readonly uint _maxMinPop = 20;
        private static readonly uint _maxMaxPop = 25;
        private static readonly uint _maxRepPop = 25;

        public static string WriteToJsonFile(string saveFile, Evolution evolution)
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(evolution);
                writer = new StreamWriter(saveFile, false);
                writer.Write(contentsToWriteToFile);
                return $"Succesfully saved file to {saveFile}";
            }
            catch (IOException e)
            {
                return $"Error occured while saving: {e.Message}";
            }
            catch (ObjectDisposedException e)
            {
                return $"Error occured while saving: {e.Message}";
            }
            finally
            {
                writer?.Close();
            }
        }

        public static Tuple<string, Evolution?> ReadFromJsonFile(string saveFile)
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(saveFile);
                var fileContents = reader.ReadToEnd();
                Evolution evolution = JsonConvert.DeserializeObject<Evolution>(fileContents);
                if(ValidateSaveFile(evolution)) return new Tuple<string, Evolution?>($"Succesfully loaded {saveFile}", evolution);
                return new Tuple<string, Evolution?>($"Data in the savefile is invalid {saveFile}", null);
            }
            catch (JsonReaderException e)
            {
                return new Tuple<string, Evolution?>($"Invalid save file {saveFile}", null);
            }
            finally
            {
                reader?.Close();
            }
        }

        private static bool ValidateSaveFile(Evolution evolution)
        {
            // map size
            if (evolution.MapSize.SizeX > _maxMapSizeX || evolution.MapSize.SizeY > _maxMapSizeY) { return false; }
            if (evolution.MapSize.SizeX < _minMapSizeX || evolution.MapSize.SizeY < _minMapSizeY) { return false; }

            // game settings
            if (evolution.GameSettings.MaxPop > _maxMaxPop || evolution.GameSettings.MaxPop < _minMaxPop) { return false; }
            if (evolution.GameSettings.MinPop > _maxMinPop || evolution.GameSettings.MinPop < _minMinPop) { return false; }
            if (evolution.GameSettings.ReproducePop > _maxRepPop || evolution.GameSettings.ReproducePop < _minRepPop) { return false; }

            // cells and cell count
            if (evolution.Cells.Count != evolution.MapSize.SizeX * evolution.MapSize.SizeY) { return false; }
            uint aliveCount = 0;

            foreach (var cell in evolution.Cells)
            {
                if (cell.Item1 < 0 || cell.Item1 >= evolution.MapSize.SizeX) { return false; }
                if (cell.Item2 < 0 || cell.Item2 >= evolution.MapSize.SizeY) { return false; }
                if (cell.Item3 < 0 || cell.Item3 > 3) {  return false; }
                if (cell.Item3 >= 2) aliveCount++;
            }
            if (aliveCount != evolution.Parameters.CellCount) { return false; }
            if (evolution.Parameters.GenerationNumber < 1) { return false; }
            if (evolution.Parameters.BirthCount > evolution.Parameters.CellCount) { return false; }
            return true;
        }

        public static void WriteToPNG(string saveFile, Evolution evolution)
        {
            using (Bitmap b = new((int)evolution.MapSize.SizeX*16, (int)evolution.MapSize.SizeY * 16))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    foreach (var cell in evolution.Cells)
                    {
                        if(cell.Item3 >= 2)
                        {
                            g.FillRectangle(Brushes.Green, cell.Item1 * 16, cell.Item2 * 16, 16, 16);
                        }
                    }
                }
                b.Save(saveFile, ImageFormat.Png);
            }
        }
    }
}
