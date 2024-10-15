using HighscoreListener.Loggers;

namespace HighscoreListener.Commands;

public class LoadCommand : ICommand
{
    public string Name => "load";
    public void Execute(CommandContext context, string[] args)
    {
        Console.WriteLine("Do you want to save current data before loading new data? (yes/no)");
        string response = Console.ReadLine() ?? "yes";
        if (response.Trim().ToLower() == "yes")
        {
            try
            {
                context.DataService.Save(context.DefaultFileName, context.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Logger.Log($"{ex.GetType()}: Access denied.", LoggerBase.SeverityLevel.Error);
            }
            catch (ArgumentException ex)
            {
                context.Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }
            catch (PathTooLongException ex)
            {
                context.Logger.Log($"{ex.GetType()}: The specified path is too long.", LoggerBase.SeverityLevel.Error);
            }
            catch (IOException ex)
            {
                context.Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
            }
        }
        // Load new data. Only assign data if the new data is not null.
        try
        {
            Game? newData = context.DataService.Load(args[0]);
            context.UpdateData(newData!);
        }
        catch (FileNotFoundException ex)
        {
            context.Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Logger.Log($"{ex.GetType()}: Access denied.", LoggerBase.SeverityLevel.Error);
        }
        catch (PathTooLongException ex)
        {
            context.Logger.Log($"{ex.GetType()}: The specified path is too long.", LoggerBase.SeverityLevel.Error);
        }
        catch (ArgumentException ex)
        {
            context.Logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.Error);
        }
        catch (IOException ex)
        {
            context.Logger.Log($"{ex.GetType()}: IO error.", LoggerBase.SeverityLevel.Error);
        }
    }
}