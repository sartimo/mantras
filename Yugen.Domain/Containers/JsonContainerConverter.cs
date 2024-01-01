using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Yugen.Domain.Monitors;
using Yugen.Domain.Windows;
using Yugen.Domain.Workspaces;
using Yugen.Infrastructure.Serialization;

namespace Yugen.Domain.Containers
{
  public class JsonContainerConverter : JsonConverter<Container>
  {
    public override bool CanConvert(Type typeToConvert)
    {
      return typeof(Container).IsAssignableFrom(typeToConvert);
    }

    public override Container Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      throw new NotSupportedException(
        $"Deserializing {typeToConvert.Name} from JSON is not supported."
      );
    }

    public override void Write(
      Utf8JsonWriter writer,
      Container value,
      JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      switch (value)
      {
        case Monitor monitor:
          WriteMonitorProperties(writer, monitor, options);
          break;
        case Workspace workspace:
          WriteSplitContainerProperties(writer, workspace, options);
          WriteWorkspaceProperties(writer, workspace, options);
          break;
        case SplitContainer splitContainer:
          WriteSplitContainerProperties(writer, splitContainer, options);
          break;
        case MinimizedWindow minimizedWindow:
          WriteWindowProperties(writer, minimizedWindow, options);
          WriteMinimizedWindowProperties(writer, minimizedWindow, options);
          break;
        case FloatingWindow floatingWindow:
          WriteWindowProperties(writer, floatingWindow, options);
          break;
        case TilingWindow tilingWindow:
          WriteWindowProperties(writer, tilingWindow, options);
          WriteTilingWindowProperties(writer, tilingWindow, options);
          break;
      }

      // The following properties are required for all container types.
      WriteCommonProperties(writer, value, options);

      writer.WriteEndObject();
    }

    private void WriteCommonProperties(
      Utf8JsonWriter writer,
      Container value,
      JsonSerializerOptions options)
    {
      writer.WriteString(JsonParser.ChangeCasing("Id", options), value.Id);
      writer.WriteNumber(JsonParser.ChangeCasing("X", options), value.X);
      writer.WriteNumber(JsonParser.ChangeCasing("Y", options), value.Y);
      writer.WriteNumber(JsonParser.ChangeCasing("Width", options), value.Width);
      writer.WriteNumber(JsonParser.ChangeCasing("Height", options), value.Height);
      writer.WritePropertyName(JsonParser.ChangeCasing("Type", options));
      JsonSerializer.Serialize(writer, value.Type, options);
      writer.WriteNumber(
        JsonParser.ChangeCasing("FocusIndex", options),
        value.FocusIndex
      );

      // Recursively serialize child containers.
      writer.WriteStartArray(JsonParser.ChangeCasing("Children", options));
      foreach (var child in value.Children)
        Write(writer, child, options);

      writer.WriteEndArray();
    }

    private static void WriteMonitorProperties(
      Utf8JsonWriter writer,
      Monitor monitor,
      JsonSerializerOptions options)
    {
      writer.WriteString(
        JsonParser.ChangeCasing("DeviceName", options),
        monitor.DeviceName
      );
    }

    private static void WriteWorkspaceProperties(
      Utf8JsonWriter writer,
      Workspace workspace,
      JsonSerializerOptions options)
    {
      writer.WriteString(JsonParser.ChangeCasing("Name", options), workspace.Name);
    }

    private static void WriteSplitContainerProperties(
      Utf8JsonWriter writer,
      SplitContainer splitContainer,
      JsonSerializerOptions options)
    {
      writer.WritePropertyName(JsonParser.ChangeCasing("TilingDirection", options));
      JsonSerializer.Serialize(writer, splitContainer.TilingDirection, options);

      writer.WriteNumber(
        JsonParser.ChangeCasing("SizePercentage", options),
        splitContainer.SizePercentage
      );
    }

    private static void WriteWindowProperties(
      Utf8JsonWriter writer,
      Window window,
      JsonSerializerOptions options)
    {
      writer.WritePropertyName(JsonParser.ChangeCasing("FloatingPlacement", options));
      JsonSerializer.Serialize(writer, window.FloatingPlacement, options);
      writer.WritePropertyName(JsonParser.ChangeCasing("BorderDelta", options));
      JsonSerializer.Serialize(writer, window.BorderDelta, options);
      writer.WriteNumber(
        JsonParser.ChangeCasing("Handle", options),
        window.Handle.ToInt64()
      );
    }

    private static void WriteMinimizedWindowProperties(
      Utf8JsonWriter writer,
      MinimizedWindow minimizedWindow,
      JsonSerializerOptions options)
    {

      writer.WritePropertyName(JsonParser.ChangeCasing("PreviousState", options));
      JsonSerializer.Serialize(writer, minimizedWindow.PreviousState, options);
    }

    private static void WriteTilingWindowProperties(
      Utf8JsonWriter writer,
      TilingWindow tilingWindow,
      JsonSerializerOptions options)
    {
      writer.WriteNumber(
        JsonParser.ChangeCasing("SizePercentage", options),
        tilingWindow.SizePercentage
      );
    }
  }
}
