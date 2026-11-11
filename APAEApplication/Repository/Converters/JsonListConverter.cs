using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Infrastructure.Repository.Converters
{
    public static class JsonListConverter
    {
        private static readonly Dictionary<int, string> LegacyHelpTypeMap = new()
        {
            { 0, "Armazenamento" },
            { 1, "Mão de obra" },
            { 2, "Doação financeira" },
            { 3, "Outros" }
        };

        public static string SerializeList(List<string>? list)
        {
            if (list == null || !list.Any())
                return "[]";
            return JsonSerializer.Serialize(list);
        }

        public static List<string> DeserializeHelpTypes(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var result = new List<string>();
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.String)
                        {
                            var str = element.GetString();
                            if (!string.IsNullOrWhiteSpace(str))
                                result.Add(str);
                        }
                        else if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var intVal))
                        {
                            if (LegacyHelpTypeMap.TryGetValue(intVal, out var mapped))
                                result.Add(mapped);
                            else
                                result.Add($"Ajuda ({intVal})");
                        }
                    }
                    return result;
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.String)
                {
                    var str = doc.RootElement.GetString();
                    return string.IsNullOrWhiteSpace(str) ? new List<string>() : new List<string> { str };
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Number && doc.RootElement.TryGetInt32(out var singleInt))
                {
                    if (LegacyHelpTypeMap.TryGetValue(singleInt, out var mapped))
                        return new List<string> { mapped };
                    return new List<string> { singleInt.ToString() };
                }
            }
            catch
            {
                // Fallback resiliente caso venha texto simples separado por vírgula
                var clean = json.Trim('[', ']', ' ', '"', '\'');
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    return clean.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim(' ', '"', '\''))
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();
                }
            }

            return new List<string>();
        }

        public static List<string> DeserializeStringList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var result = new List<string>();
                    foreach (var element in doc.RootElement.EnumerateArray())
                    {
                        if (element.ValueKind == JsonValueKind.String)
                        {
                            var str = element.GetString();
                            if (!string.IsNullOrWhiteSpace(str))
                                result.Add(str);
                        }
                        else if (element.ValueKind == JsonValueKind.Number)
                        {
                            result.Add(element.ToString());
                        }
                    }
                    return result;
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.String)
                {
                    var str = doc.RootElement.GetString();
                    return string.IsNullOrWhiteSpace(str) ? new List<string>() : new List<string> { str };
                }
            }
            catch
            {
                var clean = json.Trim('[', ']', ' ', '"', '\'');
                if (!string.IsNullOrWhiteSpace(clean))
                {
                    return clean.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim(' ', '"', '\''))
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();
                }
            }

            return new List<string>();
        }
    }
}
