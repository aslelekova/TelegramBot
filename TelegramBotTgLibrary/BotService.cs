using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotTgLibrary
{
    /// <summary>
    /// Represents the service responsible for handling bot functionalities.
    /// </summary>
    public class BotService
    {
        /// <summary>
        /// Logger instance for logging messages.
        /// </summary>
        private static ILogger _logger;
        
        /// <summary>
        /// Telegram bot client instance.
        /// </summary>
        private TelegramBotClient _bot;
        
        /// <summary>
        /// CancellationTokenSource instance for handling cancellation.
        /// </summary>
        private CancellationTokenSource _cts;
        
        /// <summary>
        /// List of plant data.
        /// </summary>
        private static List<PlantData> _plants = new();
        
        /// <summary>
        /// Dictionary to store awaiting filter inputs.
        /// </summary>
        private static Dictionary<long, string> _awaitingFilterInput = new();
        
        /// <summary>
        /// Dictionary to store filter values.
        /// </summary>
        private static Dictionary<long, List<string>> _filterValues = new Dictionary<long, List<string>>();

        /// <summary>
        /// Initializes a new instance of the BotService class.
        /// </summary>
        /// <param name="logger">Logger instance for logging bot service activities.</param>
        /// <param name="botToken">Token for accessing the Telegram Bot API.</param>
        public BotService(ILogger logger, string botToken)
        {
            _logger = logger;
            _bot = new TelegramBotClient(botToken);
        }

        /// <summary>
        /// Starts the bot service asynchronously.
        /// </summary>
        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();
            _logger.LogInformation("Бот запущен");
            _bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new(), _cts.Token);
        }

        /// <summary>
        /// Stops the bot service asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask; 
        }

        /// <summary>
        /// Handles incoming updates asynchronously.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="update">The incoming update.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the asynchronous operation.</param>
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received update of type {update.Type}");
            try
            {
                await (update.Type switch
                {
                    UpdateType.CallbackQuery => HandleCallbackQueryAsync(botClient, update.CallbackQuery),
                    _ when update.Message?.Type == MessageType.Text => HandleMessageUpdateAsync(botClient, update.Message),
                    _ when update.Message?.Type == MessageType.Document => HandleDocumentMessageAsync(botClient, update.Message),
                    _ => Task.CompletedTask
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling update");
            }
        }

        /// <summary>
        /// Handles the callback query asynchronously.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="callbackQuery">The callback query.</param>
        private static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            if (_plants.Count == 0)
            {
                await SendNoDataMessageAsync(botClient, chatId);
                return;
            }

            var data = callbackQuery.Data;
            if (data.StartsWith("select_"))
                HandleFilterSelection(botClient, chatId, data);
            else
                await HandleDataProcessingAsync(botClient, callbackQuery, data);
        }

        /// <summary>
        /// Handles errors that occur during update processing asynchronously. Logs the error message using the logger.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="ex">The exception that occurred.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            _logger.LogError($"Ошибка при обработке обновления: {ex.Message}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles message updates asynchronously. Routes the message to the appropriate handler based on its content.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="message">The message update received.</param>
        private static async Task HandleMessageUpdateAsync(ITelegramBotClient botClient, Message message)
        {
            var chatId = message.Chat.Id;

            await (message.Text switch
            {
                "/start" => SendStartMessageAsync(botClient, chatId),
                _ when _awaitingFilterInput.ContainsKey(chatId) => HandleFilterInputAsync(botClient, message),
                _ => SendUnknownCommandMessageAsync(botClient, chatId)
            });
        }

        /// <summary>
        /// Handles filter input asynchronously. Processes user input for filtering data.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="message">The message containing the filter input.</param>
        private static async Task HandleFilterInputAsync(ITelegramBotClient botClient, Message message)
        {
            var chatId = message.Chat.Id;
            var filterValue = message.Text;

            if (_awaitingFilterInput.ContainsKey(chatId))
            {
                var filterField = _awaitingFilterInput[chatId].Substring("select_".Length);
                string[] fields = filterField.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

                if (!_filterValues.ContainsKey(chatId))
                {
                    _filterValues[chatId] = new List<string>();
                }

                _filterValues[chatId].Add(filterValue);

                if (_filterValues[chatId].Count < fields.Length)
                {
                    // Requesting the next filter value if not all filter values are provided yet.
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Введите значение для поля {fields[_filterValues[chatId].Count]}:");
                }
                else
                {
                    // Applying the filter and sending confirmation message when all filter values are provided.
                    _plants = FilterPlants(filterField, _filterValues[chatId].ToArray());
                    _logger.LogInformation($"Выборка по полю {filterField} выполнена. Количество выбранных объектов: {_plants.Count}");
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"✅Выборка по полю {filterField} выполнена. Количество выбранных объектов: {_plants.Count}");

                    // Cleaning up filter-related data.
                    _awaitingFilterInput.Remove(chatId);
                    _filterValues.Remove(chatId);
                }
            }
        }

        /// <summary>
        /// Filters the list of plants based on the specified filter field and values.
        /// </summary>
        /// <param name="filterField">The field to filter by.</param>
        /// <param name="filterValues">The values to filter with.</param>
        /// <returns>The filtered list of plants.</returns>
        private static List<PlantData> FilterPlants(string filterField, string[] filterValues)
        {
            string[] fields = filterField.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            switch (fields.Length)
            {
                case 1:
                    return filterField switch
                    {
                        "LandscapingZone" => PlantFilter.FilterByLandscapingZone(_plants, filterValues[0]),
                        "LocationPlace" => PlantFilter.FilterByLocationPlace(_plants, filterValues[0]),
                        "ProsperityPeriod" => PlantFilter.FilterByProsperityPeriod(_plants, filterValues[0]),
                        _ => _plants
                    };
                case 2 when fields[0] == "LandscapingZone" && fields[1] == "ProsperityPeriod":
                    if (filterValues.Length == 2)
                    {
                        return PlantFilter.FilterByLandscapingZoneAndProsperityPeriod(_plants, filterValues[0], filterValues[1]);
                    }
                    return _plants;
                default:
                    return _plants;
            }
        }

        /// <summary>
        /// Sends a message indicating that the command is unknown.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task SendUnknownCommandMessageAsync(ITelegramBotClient botClient, long chatId) =>
            botClient.SendTextMessageAsync(chatId, "⚠️Извините, я не понимаю эту команду. Пожалуйста, загрузите файл или используйте команду /start.");

        /// <summary>
        /// Sends a welcome message with instructions to start.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The ID of the chat to send the message to.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task SendStartMessageAsync(ITelegramBotClient botClient, long chatId) =>
            botClient.SendTextMessageAsync(chatId, "✋Добро пожаловать!\nПожалуйста, загрузите ваш файл в формате CSV или JSON.");

        /// <summary>
        /// Handles the message containing a document sent by the user.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="message">The message containing the document.</param>
        private static async Task HandleDocumentMessageAsync(ITelegramBotClient botClient, Message message)
        {
            var chatId = message.Chat.Id;
            _logger.LogInformation($"Получен документ: {message.Document.FileName}");

            var tempFilePath = await FileProcessor.DownloadDocument(botClient, message);
            var fileExtension = Path.GetExtension(message.Document.FileName);

            _logger.LogInformation($"Документ скачан и сохранен как: {tempFilePath}");

            if (fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
                await ProcessCsvFileAsync(tempFilePath, botClient, chatId);
            else if (fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                await ProcessJsonFileAsync(tempFilePath, botClient, chatId);
            else
                await botClient.SendTextMessageAsync(chatId, "️❗️Формат файла не поддерживается. Пожалуйста, загрузите файл в формате CSV или JSON.");

            if (System.IO.File.Exists(tempFilePath))
                System.IO.File.Delete(tempFilePath);
        }

        /// <summary>
        /// Processes a CSV file containing plant data.
        /// </summary>
        /// <param name="tempFilePath">The temporary file path of the CSV file.</param>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The chat ID where the file was received.</param>
        private static async Task ProcessCsvFileAsync(string tempFilePath, ITelegramBotClient botClient, long chatId)
        {
            try
            {
                _plants = FileProcessor.ReadCsvFile(tempFilePath);
                foreach (var plant in _plants)
                    _logger.LogInformation($"Обработан объект: {plant.LatinName}");
                await SendActionKeyboardAsync(botClient, chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке CSV файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a JSON file containing plant data.
        /// </summary>
        /// <param name="tempFilePath">The temporary file path of the JSON file.</param>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The chat ID where the file was received.</param>
        private static async Task ProcessJsonFileAsync(string tempFilePath, ITelegramBotClient botClient, long chatId)
        {
            try
            {
                _plants = FileProcessor.ReadJsonFile(tempFilePath);
                foreach (var plant in _plants)
                    _logger.LogInformation($"Обработан объект: {plant.LatinName}");
                await SendActionKeyboardAsync(botClient, chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при обработке JSON файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends an action keyboard to the user with various options.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The chat ID where the keyboard will be sent.</param>
        private static async Task SendActionKeyboardAsync(ITelegramBotClient botClient, long chatId)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
            {
        new[] { InlineKeyboardButton.WithCallbackData("🔍Выборка по LandscapingZone", "select_LandscapingZone") },
        new[] { InlineKeyboardButton.WithCallbackData("🔍Выборка по LocationPlace", "select_LocationPlace") },
        new[] { InlineKeyboardButton.WithCallbackData("🔍Выборка по ProsperityPeriod", "select_ProsperityPeriod") },
        new[] { InlineKeyboardButton.WithCallbackData("🔍Выборка по LandscapingZone & ProsperityPeriod", "select_LandscapingZone+ProsperityPeriod") }, // Новая кнопка для выборки по двум полям
        new[] { InlineKeyboardButton.WithCallbackData("📊Сортировка LatinName ↑", "sort_LatinName_asc"), InlineKeyboardButton.WithCallbackData("📊Сортировка LatinName ↓", "sort_LatinName_desc") },
        new[] { InlineKeyboardButton.WithCallbackData("️⬇️Скачать CSV", "download_csv"), InlineKeyboardButton.WithCallbackData("⬇️Скачать JSON", "download_json") }
    });

            await botClient.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: keyboard);
        }

        /// <summary>
        /// Sends a message indicating that there is no data to process.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The chat ID where the message will be sent.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task SendNoDataMessageAsync(ITelegramBotClient botClient, long chatId) =>
            botClient.SendTextMessageAsync(chatId, "❗️️У вас нет данных для обработки. Пожалуйста, загрузите файл формата csv или json.");

        
        /// <summary>
        /// Handles the selection of a filter by sending a message to enter a value for the filter.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="chatId">The chat ID where the message will be sent.</param>
        /// <param name="data">The filter selection data.</param>
        private static void HandleFilterSelection(ITelegramBotClient botClient, long chatId, string data)
        {
            _awaitingFilterInput[chatId] = data;
            botClient.SendTextMessageAsync(chatId, "Введите значение для фильтрации:");
        }

        /// <summary>
        /// Handles the processing of callback query data.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="callbackQuery">The callback query containing the data to process.</param>
        /// <param name="data">The callback query data.</param>
        private static async Task HandleDataProcessingAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, string data)
        {
            Task taskToAwait;

            switch (data)
            {
                case "sort_LatinName_asc":
                    taskToAwait = HandleSortingAsync(botClient, callbackQuery, "LatinName", PlantSorter.SortByLatinNameAscending);
                    break;
                case "sort_LatinName_desc":
                    taskToAwait = HandleSortingAsync(botClient, callbackQuery, "LatinName", PlantSorter.SortByLatinNameDescending);
                    break;
                case "download_csv":
                    taskToAwait = DownloadFileAsync(botClient, callbackQuery, new CsvFileProcessor(), "plants.csv");
                    break;
                case "download_json":
                    taskToAwait = DownloadFileAsync(botClient, callbackQuery, new JsonFileProcessor(), "plants.json");
                    break;
                default:
                    _logger.LogWarning($"Received unknown callback query: {data}");
                    taskToAwait = Task.CompletedTask;
                    break;
            }
            await taskToAwait;
        }

        /// <summary>
        /// Handles sorting of plant data based on a specified field and sort action.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="callbackQuery">The callback query.</param>
        /// <param name="field">The field to sort by.</param>
        /// <param name="sortAction">The sort action to perform.</param>
        private static async Task HandleSortingAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, string field, Action<List<PlantData>> sortAction)
        {
            sortAction(_plants);
            await SendSortMessageAsync(botClient, callbackQuery, field, sortAction == PlantSorter.SortByLatinNameAscending ? "прямом" : "обратном");
        }

        /// <summary>
        /// Sends a message indicating that data has been sorted.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="callbackQuery">The callback query.</param>
        /// <param name="field">The field by which the data was sorted.</param>
        /// <param name="order">The sorting order (ascending or descending).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task SendSortMessageAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, string field, string order) =>
            botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"✅Данные отсортированы по полю {field} в {order} порядке");

        /// <summary>
        /// Downloads a file processed by the file processor and sends it to the user.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="callbackQuery">The callback query.</param>
        /// <param name="fileProcessor">The file processor.</param>
        /// <param name="fileName">The name of the file to be sent.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task DownloadFileAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, IFileProcessor fileProcessor, string fileName)
        {
            var stream = fileProcessor.Write(_plants);
            stream.Seek(0, SeekOrigin.Begin);
            await botClient.SendDocumentAsync(callbackQuery.Message.Chat.Id, new InputFileStream(stream, fileName));
        }
    }
}