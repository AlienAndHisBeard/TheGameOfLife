using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TheGameOfLife.Models.Structs.Structs;

namespace TheGameOfLife.Utils
{
    public class SaveFileHandler
    {
        public static void WriteToJsonFile(string saveFile, Evolution evolution)
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(evolution);
                writer = new StreamWriter(saveFile, false);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static Evolution ReadFromJsonFile(string saveFile)
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(saveFile);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Evolution>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        // TODO: SaveFile validation
    }
}
