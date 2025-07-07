using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SQLUnitTest.Models
{
    /// <summary>
    /// JSON converter that materializes <see cref="BaseTestCase"/> instances based
    /// on the "type" discriminator property.
    /// </summary>
    public class BaseTestCaseJsonConverter : JsonConverter<BaseTestCase>
    {
        public override BaseTestCase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            if (!document.RootElement.TryGetProperty("type", out var typeProperty))
            {
                throw new JsonException("Missing type discriminator");
            }

            var typeName = typeProperty.GetString();
            BaseTestCase? result;
            switch (typeName)
            {
                case nameof(ExecutionTestCase):
                    result = document.RootElement.Deserialize<ExecutionTestCase>(options);
                    break;
                case nameof(OutputParameterTestCase):
                    result = document.RootElement.Deserialize<OutputParameterTestCase>(options);
                    break;
                case nameof(StoredProcedureCompareTestCase):
                    var compare = document.RootElement.Deserialize<StoredProcedureCompareTestCase>(options);
                    if (document.RootElement.TryGetProperty("expectedProcedure", out var expectedNode))
                    {
                        if (expectedNode.TryGetProperty("storedProcedure", out var sp))
                        {
                            compare!.ExpectedStoredProcedure = sp.GetString() ?? string.Empty;
                        }
                        if (expectedNode.TryGetProperty("connectionName", out var conn))
                        {
                            compare!.ExpectedConnection = conn.GetString();
                        }
                    }
                    result = compare;
                    break;
                default:
                    throw new JsonException($"Unknown test case type '{typeName}'.");
            }

            if (result != null)
            {
                result.Type = typeName!;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, BaseTestCase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
