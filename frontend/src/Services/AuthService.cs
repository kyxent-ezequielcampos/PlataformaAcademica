namespace frontend.Services;

using System;
using System.IO;
using System.Text.Json;
using frontend.Models;

public class AuthService
{
    private readonly string _sessionFile;
    private Usuario? _currentUser;

    public AuthService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appData, "SistemaAcademico");
        Directory.CreateDirectory(appFolder);
        _sessionFile = Path.Combine(appFolder, "session.json");
    }

    public Usuario? CurrentUser => _currentUser ??= LoadSession();

    public bool IsAuthenticated()
    {
        return CurrentUser != null && CurrentUser.Activo;
    }

    public void SaveSession(Usuario usuario)
    {
        _currentUser = usuario;
        var json = JsonSerializer.Serialize(usuario);
        File.WriteAllText(_sessionFile, json);
    }

    public Usuario? LoadSession()
    {
        if (!File.Exists(_sessionFile)) return null;

        try
        {
            var json = File.ReadAllText(_sessionFile);
            return JsonSerializer.Deserialize<Usuario>(json);
        }
        catch
        {
            return null;
        }
    }

    public void ClearSession()
    {
        _currentUser = null;
        if (File.Exists(_sessionFile))
        {
            File.Delete(_sessionFile);
        }
    }
}
