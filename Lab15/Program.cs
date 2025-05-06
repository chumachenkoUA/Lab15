using Avalonia;
using System;

namespace Lab15
{
    internal class Program
    {
        // Вхідна точка
        [STAThread]
        public static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

        // Конфігурація Avalonia
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}