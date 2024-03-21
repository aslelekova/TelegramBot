using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;


namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Provides methods to process CSV files containing plant data.
    /// </summary>
    public class CsvFileProcessor : IFileProcessor
    {
        /// <summary>
        /// Writes plant data to a CSV file stream.
        /// </summary>
        /// <param name="plants">The collection of plant data to write.</param>
        /// <returns>A stream containing the CSV data.</returns>
        public Stream Write(IEnumerable<PlantData> plants)
        {
            // Create a memory stream to store the data.
            var memoryStream = new MemoryStream();

            // Create a StreamWriter to write data to the memory stream using UTF-8 encoding.
            using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(true), 1024, leaveOpen: true))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                   {
                       // Set the field delimiter as a semicolon.
                       Delimiter = ";",
                       // Set UTF-8 encoding.
                       Encoding = new UTF8Encoding(true),
                       // Specify that the file has no header because we'll add it manually.
                       HasHeaderRecord = false,
                       // Specify that all fields should be quoted.
                       ShouldQuote = args => true
                   }))
            {
                // Add the English header
                csv.WriteField("ID");
                csv.WriteField("Name");
                csv.WriteField("LatinName");
                csv.WriteField("Photo");
                csv.WriteField("LandscapingZone");
                csv.WriteField("ProsperityPeriod");
                csv.WriteField("Description");
                csv.WriteField("LocationPlace");
                csv.WriteField("ViewForm");
                csv.WriteField("global_id");
                csv.NextRecord();

                // Add the Russian header
                csv.WriteField("Код");
                csv.WriteField("Название");
                csv.WriteField("Латинское название");
                csv.WriteField("Фотография");
                csv.WriteField("Ландшафтная зона");
                csv.WriteField("Период цветения");
                csv.WriteField("Описание");
                csv.WriteField("Расположение в парке");
                csv.WriteField("Форма осмотра");
                csv.WriteField("global_id");
                csv.NextRecord();

                // Write plant data
                csv.WriteRecords(plants);
            }

            // Set the position of the stream to the beginning.
            memoryStream.Position = 0;
            return memoryStream;
        }
        
        /// <summary>
        /// Reads plant data from a CSV file stream.
        /// </summary>
        /// <param name="csvStream">The stream containing the CSV data.</param>
        /// <returns>The collection of plant data read from the CSV.</returns>
        public IEnumerable<PlantData> Read(Stream csvStream)
        {           
            // Create a StreamReader to read data from the memory stream using UTF-8 encoding.
            using var reader = new StreamReader(csvStream, new UTF8Encoding(true));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Set the field delimiter as a semicolon.
                Delimiter = ";",
                // Set UTF-8 encoding.
                Encoding = new UTF8Encoding(true),
            };

            // Create a CsvReader to read data from the CSV file with the specified configuration.
            using var csv = new CsvReader(reader, config);

            // Return the records skipping the header
            return csv.GetRecords<PlantData>().ToList();
        }
    }
}