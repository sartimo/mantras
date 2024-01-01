using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yugen.Domain.UserConfigs
{
  public class BarComponentConfigConverter : JsonConverter<BarComponentConfig>
  {
    public override BarComponentConfig Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options)
    {
      using var jsonObject = JsonDocument.ParseValue(ref reader);

      // Get the type of bar component (eg. "workspaces").
      var typeDiscriminator = jsonObject.RootElement.GetProperty("type").ToString();

      return typeDiscriminator switch
      {
        "battery" =>
          JsonSerializer.Deserialize<BatteryComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "binding mode" =>
          JsonSerializer.Deserialize<BindingModeComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "clock" =>
          JsonSerializer.Deserialize<ClockComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "text" =>
          JsonSerializer.Deserialize<TextComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "network" =>
          JsonSerializer.Deserialize<NetworkComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "volume" =>
          JsonSerializer.Deserialize<VolumeComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "tiling direction" =>
          JsonSerializer.Deserialize<TilingDirectionComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "window title" =>
        JsonSerializer.Deserialize<WindowTitleComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "system tray" =>
          JsonSerializer.Deserialize<SystemTrayComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "image" =>
          JsonSerializer.Deserialize<ImageComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "weather" =>
          JsonSerializer.Deserialize<WeatherComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "workspaces" =>
          JsonSerializer.Deserialize<WorkspacesComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "cpu" =>
          JsonSerializer.Deserialize<CpuComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "gpu" =>
          JsonSerializer.Deserialize<GpuComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "memory" =>
          JsonSerializer.Deserialize<MemoryComponentConfig>(
            jsonObject.RootElement.ToString(),
            options
          ),
        "text file" => JsonSerializer.Deserialize<TextFileComponentConfig>(
          jsonObject.RootElement.ToString(),
          options
        ),
        "music" => JsonSerializer.Deserialize<MusicComponentConfig>(
          jsonObject.RootElement.ToString(),
          options
        ),
        _ => throw new ArgumentException($"Invalid component type '{typeDiscriminator}'."),
      };
    }

    /// <summary>
    /// Serializing is not needed, so it's fine to leave it unimplemented.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void Write(
      Utf8JsonWriter writer,
      BarComponentConfig value,
      JsonSerializerOptions options)
    {
      throw new NotImplementedException();
    }
  }
}
