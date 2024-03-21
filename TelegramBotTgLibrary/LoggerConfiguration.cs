using Microsoft.Extensions.Logging;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Provides methods for configuring and creating loggers for the Telegram Bot.
    /// </summary>
    public static class LoggerConfiguration
    {
        /// <summary>
        /// Creates a logger factory with the specified configuration.
        /// </summary>
        /// <returns>An instance of ILoggerFactory configured for the Telegram Bot.</returns>
        public static ILoggerFactory CreateLogger()
        {
            // Specify the path to the log file.
            string logFilePath = "var/bot_log.txt";

            // Get the directory to store the log file.
            string logDirectory = Path.GetDirectoryName(logFilePath);

            if (!Directory.Exists(logDirectory))
            {         
                // Create the directory if it doesn't exist.
                Directory.CreateDirectory(logDirectory);
            }

            // Create a StreamWriter for writing logs to the file.
            StreamWriter logFileWriter = new StreamWriter(logFilePath, append: true);

            return LoggerFactory.Create(builder =>
            {
                // Set filters for different logging categories.
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("TelegramPlantBot.Program", LogLevel.Debug)

                    .AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "HH:mm:ss ";
                    })
                    
                    // Add a logger provider that writes logs to a file.
                    .AddProvider(new CustomFileLoggerProvider(logFileWriter));
            });
        }
    }

    /// <summary>
    /// Provides a custom logger provider that writes logs to a file.
    /// </summary>
    public class CustomFileLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Represents a StreamWriter used for writing logs to a file.
        /// </summary>
        private readonly StreamWriter _logFileWriter;

        /// <summary>
        /// Initializes a new instance of the CustomFileLoggerProvider class with the specified StreamWriter.
        /// </summary>
        /// <param name="logFileWriter">The StreamWriter used to write logs to the file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CustomFileLoggerProvider(StreamWriter logFileWriter)
        {
            _logFileWriter = logFileWriter ?? throw new ArgumentNullException(nameof(logFileWriter));
        }

        /// <summary>
        /// Creates a logger with the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name of the logger.</param>
        /// <returns>An ILogger instance for the specified category.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(categoryName, _logFileWriter);
        }

        /// <summary>
        /// Disposes the StreamWriter used for writing logs to the file.
        /// </summary>
        public void Dispose()
        {
            _logFileWriter.Dispose();
        }
    }

    /// <summary>
    /// Provides a custom logger that writes logs to a file.
    /// </summary>
    public class CustomFileLogger : ILogger
    {
        /// <summary>
        /// Represents the category name of the logger.
        /// </summary>
        private readonly string _categoryName;
        
        /// <summary>
        /// Represents a StreamWriter used for writing logs to a file.
        /// </summary>
        private readonly StreamWriter _logFileWriter;

        /// <summary>
        /// Initializes a new instance of the CustomFileLogger class with the specified category name and StreamWriter.
        /// </summary>
        /// <param name="categoryName">The category name of the logger.</param>
        /// <param name="logFileWriter">The StreamWriter used to write logs to the file.</param>
        public CustomFileLogger(string categoryName, StreamWriter logFileWriter)
        {
            _categoryName = categoryName;
            _logFileWriter = logFileWriter;
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state) => null;

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns>True if the log level is enabled; otherwise, false.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event Id.</param>
        /// <param name="state">The state object.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The message formatter.</param>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            
            // Format the message for logging.
            var message = formatter(state, exception);

            _logFileWriter.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{logLevel}] [{_categoryName}] {message}");

            _logFileWriter.Flush();
        }
    }
}
