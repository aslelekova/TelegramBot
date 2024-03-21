namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Defines methods for reading and writing plant data to and from streams.
    /// </summary>
    public interface IFileProcessor
    {
        // Defines the Read method to read data from a stream and return it as an enumeration of PlantData objects.
        IEnumerable<PlantData> Read(Stream stream);

        // Defines the Write method to write plant data to a stream.
        Stream Write(IEnumerable<PlantData> plants);
    }
}