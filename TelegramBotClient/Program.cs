using Microsoft.Extensions.Logging;
using TelegramBotTgLibrary;

/// <summary>
/// Represents the entry point of the program.
/// </summary>
class Program
{
    /// <summary>
    /// The entry point of the program.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    static async Task Main(string[] args)
    {
        // Get the logger factory configured to output to the console and file.
        using var loggerFactory = LoggerConfiguration.CreateLogger();

        // Create a logger for the current class.
        var logger = loggerFactory.CreateLogger<Program>();

        var botToken = "6937057901:AAFuAshwwr1BFcs_HEUp2-UaDSeVul7RhBc";

        // Initialize the bot service with the logger and token.
        var botService = new BotService(logger, botToken);


        // Start the bot.
        await botService.StartAsync();
        logger.LogInformation("Бот запущен.");
        
        // Stop the bot.
        await botService.StopAsync();
        logger.LogInformation("Бот остановлен.");
    }
}
