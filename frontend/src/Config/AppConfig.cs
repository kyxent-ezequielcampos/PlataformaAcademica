namespace frontend.Config;

public static class AppConfig
{
    public static class Api
    {
        public const string BaseUrl = "http://localhost:5030/api";
        public const int TimeoutSeconds = 30;
    }

    public static class App
    {
        public const string Name = "Sistema de Gestión Académica";
        public const string Version = "1.0.0";
        public const string SessionFileName = "session.json";
        public const string AppFolderName = "SistemaAcademico";
    }

    public static class UI
    {
        public const int DefaultWindowWidth = 1400;
        public const int DefaultWindowHeight = 900;
        public const int LoginWindowWidth = 600;
        public const int LoginWindowHeight = 700;
        public const int SidebarWidth = 250;
        public const int HeaderHeight = 70;
    }
}