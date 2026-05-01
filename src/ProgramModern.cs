namespace VoidVideoGenerator;

/// <summary>
/// Entry point for the modern UI version of VOID VIDEO GENERATOR
/// </summary>
static class ProgramModern
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable visual styles for modern appearance
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Set high DPI awareness for better scaling on modern displays
        try
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        }
        catch
        {
            // Fallback for older .NET versions
        }
        
        // Run the modern UI version
        Application.Run(new MainFormModern());
    }
}
