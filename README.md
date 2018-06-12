# cr-logging
A wrapper for initialising Serilog consistently.

## Configuration

### Parameters

All of the appsettings are optional; any settings which are missing from the configuration, or are empty/whitespace will default to the values specified below.

If an invalid value (not empty/whitespace) is entered in any of these fields, however, the `Logging.SetupLogger` method will throw an argument exception which details the issue.

    <add key="CR.Logging.Json.Enabled" value="true"/> <!--Default: 'false' -->
    <add key="CR.Logging.Json.RotateOnFileSizeLimit" value="true"/> <!--Default: 'false' -->
    <add key="CR.Logging.Text.Enabled" value="true"/> <!--Default: 'false' -->
    <add key="CR.Logging.Text.RotateOnFileSizeLimit" value="true"/> <!--Default: 'false' -->
    <add key="CR.Logging.Console.Enabled" value="true"/> <!--Default: 'false' -->
    <add key="CR.Logging.Json.FilePath" value=""/>  <!-- Default: './logs/log.json' -->
    <add key="CR.Logging.Json.MinLogLevel" value="Information"/> <!-- Default: 'Debug' -->
    <add key="CR.Logging.Json.FileRotationTime" value="Month"/> <!-- Default: 'Day' -->
    <add key="CR.Logging.Json.FileRotationSizeLimit" value="10000000"/> <!-- Default: '26214400' -->
    <add key="CR.Logging.Text.FilePath" value=""/>  <!-- Default: './logs/log.log' -->
    <add key="CR.Logging.Text.MinLogLevel" value=""/> <!-- Default: 'Debug' -->
    <add key="CR.Logging.Text.FileRotationTime" value="Month"/> <!-- Default: 'Day' -->
    <add key="CR.Logging.Text.FileRotationSizeLimit" value="10000000"/> <!-- Default: '26214400' -->
    <add key="CR.Logging.Console.MinLogLevel" value="Warning"/> <!-- Default: 'Debug' -->

### Log Levels (and corresponding enum integer values)
- Verbose (0)
- Debug (1)
- Information (2)
- Warning (3)
- Error (4)
- Fatal (5)

### Rotation Times (and corresponding enum integer values)
- Infinite (0)
- Year (1)
- Month (2)
- Day (3)
- Hour (4)
- Minute (5)

### File Size Limits
File size limits are specified in bytes.

## Usage

A Serilog Logger can be created using the configuration specified in the app configuration by calling `Logging.SetupLogger`. An `IConfiguration` can be passed to the method to read the configuration from, though if an `IConfiguration` is not provided, the configuration will be read from the app.config.

For more information on how to use the configured logger, visit [the Serilog documentation](https://github.com/serilog/serilog/wiki/Writing-Log-Events).

JSON layouts will store the template string as it appears above, as well as each of the parameters specified after the template string, while text layouts will replace the properties in the template string, and output the result.

When specifying a property in the template string, `{property}` will result in `.ToString()` being called on the provided object, while `{@property}` will result in the object being serialized.
