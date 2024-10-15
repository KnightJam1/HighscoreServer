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
                context.dataService.Save(context.defaultFileName, context.data);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.logger.Log($"{ex.GetType()}: Access denied.", LoggerBase.SeverityLevel.ERROR);
            }
            catch (ArgumentException ex)
            {
                context.logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.ERROR);
            }
            catch (PathTooLongException ex)
            {
                context.logger.Log($"{ex.GetType()}: The specified path is too long.", LoggerBase.SeverityLevel.ERROR);
            }
            catch (IOException ex)
            {
                context.logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.ERROR);
            }
        }
        // Load new data. Only assign data if the new data is not null.
        try
        {
            Game? newData = context.dataService.Load(args[0]);
            context.UpdateData(newData!);
        }
        catch (FileNotFoundException ex)
        {
            context.logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.ERROR);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.logger.Log($"{ex.GetType()}: Access denied.", LoggerBase.SeverityLevel.ERROR);
        }
        catch (PathTooLongException ex)
        {
            context.logger.Log($"{ex.GetType()}: The specified path is too long.", LoggerBase.SeverityLevel.ERROR);
        }
        catch (ArgumentException ex)
        {
            context.logger.Log($"{ex.GetType()}: {ex.Message}", LoggerBase.SeverityLevel.ERROR);
        }
        catch (IOException ex)
        {
            context.logger.Log($"{ex.GetType()}: IO error.", LoggerBase.SeverityLevel.ERROR);
        }
    }
}