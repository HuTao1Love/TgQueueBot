using Contracts.Repositories;
using Itmo.Dev.Platform.Postgres.Connection;
using Models;
using Npgsql;

namespace DataAccess.Repositories;

public class UserRepository(IPostgresConnectionProvider provider) : IUserRepository
{
    public async Task<User> FindOrCreate(long tgId, string name)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              WITH new_user AS (
                                  INSERT INTO users (tgId, name, isAdmin)
                                  VALUES (@tgId, @name, @isAdmin)
                                  ON CONFLICT (tgId)
                                  DO UPDATE SET name = EXCLUDED.name, isAdmin = EXCLUDED.isAdmin
                                  RETURNING id, tgId, name, isAdmin
                              )
                              SELECT id, tgId, name, isAdmin 
                              FROM new_user;
                              """;

        command.Parameters.AddWithValue("tgId", tgId);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("isAdmin", false);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new User(
            userId: reader.GetInt64(0),
            tgId: reader.GetInt64(1),
            name: reader.GetString(2),
            isAdmin: reader.GetBoolean(3));
    }

    public async Task SetAdmin(long tgId, bool isAdmin)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              UPDATE users
                              SET isadmin = @isAdmin
                              WHERE tgid = @tgId;
                              """;

        command.Parameters.AddWithValue("tgId", tgId);
        command.Parameters.AddWithValue("isAdmin", isAdmin);

        await command.ExecuteNonQueryAsync(default);
    }
}