using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using NuKeeper.Abstractions.Logging;

namespace NuKeeper.Abstractions.Configuration
{
    /// <summary>
    /// Converts string "true"/"false" to bool values
    /// </summary>
    public class BooleanStringConverter : JsonConverter<bool?>
    {
        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;
            if (reader.TokenType == JsonTokenType.False)
                return false;
            if (reader.TokenType == JsonTokenType.Null)
                return null;
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (bool.TryParse(stringValue, out var result))
                    return result;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteBooleanValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }

    public class FileSettingsReader : IFileSettingsReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter(), new BooleanStringConverter() }
        };

        private readonly INuKeeperLogger _logger;

        public FileSettingsReader(INuKeeperLogger logger)
        {
            _logger = logger;
        }

        public FileSettings Read(string folder)
        {
            const string fileName = "nukeeper.settings.json";

            var fullPath = Path.Combine(folder, fileName);

            if (File.Exists(fullPath))
            {
                return ReadFile(fullPath);
            }

            return FileSettings.Empty();
        }

        private FileSettings ReadFile(string fullPath)
        {
            try
            {
                var contents = File.ReadAllText(fullPath);
                var result = JsonSerializer.Deserialize<FileSettings>(contents, JsonOptions);
                _logger.Detailed($"Read settings file at {fullPath}");
                return result;
            }
            catch (IOException ex)
            {
                _logger.Error($"Cannot read settings file at {fullPath}", ex);
            }
            catch (JsonException ex)
            {
                _logger.Error($"Cannot read json from settings file at {fullPath}", ex);
            }

            return FileSettings.Empty();
        }
    }
}
