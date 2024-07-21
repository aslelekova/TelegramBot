# Telegram CSV and JSON Processor Bot

## Overview

The **Telegram CSV and JSON Processor Bot** is a C# application designed to handle and manipulate CSV and JSON files via a Telegram bot. The bot allows users to upload files, perform data operations such as sorting and filtering, and download processed files. The project incorporates knowledge of serialization, LINQ, and Telegram Bot API integration.

## Class Library

### Components

1. **Data Models**

   - **`MyType1` Class**: Represents objects described in the CSV file. The class includes fields based on the CSV schema, following Microsoft naming conventions. Each class should have appropriate constructors for initialization.

2. **CSV Processing**

   - **`CSVProcessing` Class**: Contains methods for handling CSV files.
     - **`Write` Method**: Accepts a collection of `MyType1` objects and returns a `Stream` for sending a CSV document via the Telegram bot.
     - **`Read` Method**: Accepts a `Stream` with a CSV file from the Telegram bot and returns a collection of `MyType1` objects.

3. **JSON Processing**

   - **`JSONProcessing` Class**: Handles JSON file operations.
     - **`Write` Method**: Accepts a collection of `MyType1` objects and returns a `Stream` for sending a JSON file via the Telegram bot.
     - **`Read` Method**: Accepts a `Stream` with a JSON file from the Telegram bot and returns a collection of `MyType1` objects.

## Telegram Bot

### Features

1. **File Upload and Processing**

   - **Upload CSV File**: Users can upload a CSV file for processing.
   - **Process JSON Files**: The bot can handle JSON files in the same way as CSV files, ensuring the same functionality for both formats.

2. **Data Operations**

   - **Filtering**: Allow users to filter data based on specified fields using LINQ queries.
   - **Sorting**: Provide sorting functionality based on fields, with options for ascending and descending order.

3. **File Download**

   - **Download Processed Files**: Users can download processed files in CSV or JSON formats.

4. **Error Handling and Validation**

   - **Robust Error Handling**: The bot should handle errors gracefully, including issues with file uploads and data processing.
   - **Input Validation**: Validate user inputs and provide meaningful error messages when necessary.

## How to Use

1. **Start the Bot**

   - Deploy the bot using a hosting service (e.g., Yandex Cloud) and obtain the bot's token.
   - Run the bot and interact with it via Telegram.

2. **Upload a CSV File**

   - Use the `/upload` command to send a CSV file to the bot for processing.

3. **Perform Data Operations**

   - Use commands to filter and sort data as specified in the project requirements.

4. **Download Processed Files**

   - Request processed files in CSV or JSON format using the appropriate commands.

5. **Upload and Process JSON Files**

   - Use the `/upload` command to send JSON files for processing, similar to CSV files.

## Development and Deployment

1. **Code Quality**

   - Adhere to C# best practices and .NET 6.0 conventions.
   - Include meaningful comments and documentation in the code.

2. **Testing and Debugging**

   - Ensure the bot handles all edge cases and exceptions effectively.
   - Test the application thoroughly to validate functionality.

3. **Deployment**

   - Publish the bot on a cloud hosting platform (e.g., Yandex Cloud) and provide the bot's URL for access.
