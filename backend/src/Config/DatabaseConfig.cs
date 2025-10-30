namespace SistemaAcademico.Config;

using Npgsql;

public class DatabaseConfig
{
    private readonly string _connectionString;

    public DatabaseConfig(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Host=localhost;Database=sistema_academico;Username=kyxent;Password=kyxent";
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}