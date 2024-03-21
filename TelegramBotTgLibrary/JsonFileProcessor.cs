using Newtonsoft.Json;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Implementation of the IFileProcessor interface for handling JSON files.
    /// </summary>
    public class JsonFileProcessor : IFileProcessor 
    {
        /// <summary>
        /// Reads plant data from a JSON stream and returns it as an enumerable collection of PlantData objects.
        /// </summary>
        /// <param name="jsonStream">The JSON stream to read from.</param>
        /// <returns>An enumerable collection of PlantData objects.</returns>
        public IEnumerable<PlantData> Read(Stream jsonStream)
        {
            // Set the position of the stream to the beginning.
            jsonStream.Seek(0, SeekOrigin.Begin);

            // Create a StreamReader to read data from the JSON stream.
            using var sr = new StreamReader(jsonStream);

            // Create a JsonTextReader to read JSON text from the StreamReader.
            using var jsonTextReader = new JsonTextReader(sr);

            // Create a JsonSerializer for deserializing JSON.
            var serializer = new JsonSerializer();

            // Deserialize the JSON and return the result as a list of PlantData objects.
            return serializer.Deserialize<List<PlantData>>(jsonTextReader); 
        }

        /// <summary>
        /// Writes plant data to a stream in JSON format.
        /// </summary>
        /// <param name="plants">The collection of PlantData objects to write.</param>
        /// <returns>A stream containing the JSON representation of the plant data.</returns>
        public Stream Write(IEnumerable<PlantData> plants)
        {
            // Create a memory stream to store the data.
            var memoryStream = new MemoryStream();

            // Create a StreamWriter to write data to the memory stream.
            using var sw = new StreamWriter(memoryStream, leaveOpen: true);

            var serializer = new JsonSerializer();

            // Set formatting for indented JSON.
            serializer.Formatting = Formatting.Indented;

            // Serialize the plant data to JSON.
            serializer.Serialize(sw, plants);

            // Flush the StreamWriter buffer.
            sw.Flush();
            
            // Set the position of the stream to the beginning.
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
