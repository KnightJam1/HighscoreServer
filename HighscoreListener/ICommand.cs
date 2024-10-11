public interface ICommand
{
    string Name { get; }
    public void Execute(string[] args);
}