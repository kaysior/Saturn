class Program
{
    static void Main()
    {
        using (var window = new SaturnSimulation())
        {
            window.Run();
        }
    }
}