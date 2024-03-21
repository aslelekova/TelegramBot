using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Handles processing of files, including CSV and JSON formats.
    /// </summary>
    public class FileProcessor
    {
        /// <summary>
        /// Reads plant data from a CSV file.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        /// <returns>A list of plant data read from the CSV file.</returns>
        public static List<PlantData> ReadCsvFile(string filePath)
        {
            List<PlantData> plants = new List<PlantData>();
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", HasHeaderRecord = true }))
            {
                plants = csv.GetRecords<PlantData>().ToList();
            }
    
            // Remove records where ID is "Код".
            plants.RemoveAll(plant => plant.ID == "Код");

            return plants;
        }
        
        /// <summary>
        /// Reads plant data from a JSON file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>A list of plant data read from the JSON file.</returns>
        public static List<PlantData> ReadJsonFile(string filePath)
        {
            using (var jsonStream = System.IO.File.OpenRead(filePath))
            {
                var jsonProcessing = new JsonFileProcessor();
                return jsonProcessing.Read(jsonStream).ToList();
            }
        }

        /// <summary>
        /// Downloads a document from Telegram and returns its temporary file path.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="message">The message containing the document to download.</param>
        /// <returns>The temporary file path where the document is saved.</returns>
        public static async Task<string> DownloadDocument(ITelegramBotClient botClient, Message message)
        {
            var fileInfo = await botClient.GetFileAsync(message.Document.FileId);
            var filePath = fileInfo.FilePath;
            var fileExtension = Path.GetExtension(message.Document.FileName);
            var tempFilePath = Path.GetTempFileName() + fileExtension;

            using (var saveFileStream = System.IO.File.OpenWrite(tempFilePath))
            {
                await botClient.DownloadFileAsync(filePath, saveFileStream);
            }
            return tempFilePath;
        }
    }
}
