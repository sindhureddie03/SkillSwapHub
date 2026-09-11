using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class UserRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public UserRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                SELECT COUNT(1)
                FROM Users
                WHERE Email = @Email;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", email);

            int count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        // STEP 15.2 - ADD THIS
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                SELECT
                    UserId,
                    Name,
                    Email,
                    PasswordHash,
                    Bio,
                    ExperienceLevel,
                    Availability,
                    PreferredSessionDuration,
                    CreatedAt
                FROM Users
                WHERE Email = @Email;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),

                    Bio = reader.IsDBNull(reader.GetOrdinal("Bio"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Bio")),

                    ExperienceLevel = reader.IsDBNull(reader.GetOrdinal("ExperienceLevel"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ExperienceLevel")),

                    Availability = reader.IsDBNull(reader.GetOrdinal("Availability"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Availability")),

                    PreferredSessionDuration = reader.IsDBNull(reader.GetOrdinal("PreferredSessionDuration"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("PreferredSessionDuration")),

                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }

            return null;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        SELECT
            UserId,
            Name,
            Email,
            PasswordHash,
            Bio,
            ExperienceLevel,
            Availability,
            PreferredSessionDuration,
            CreatedAt
        FROM Users
        WHERE UserId = @UserId;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),

                    Bio = reader.IsDBNull(reader.GetOrdinal("Bio"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Bio")),

                    ExperienceLevel = reader.IsDBNull(reader.GetOrdinal("ExperienceLevel"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ExperienceLevel")),

                    Availability = reader.IsDBNull(reader.GetOrdinal("Availability"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("Availability")),

                    PreferredSessionDuration = reader.IsDBNull(reader.GetOrdinal("PreferredSessionDuration"))
                        ? null
                        : reader.GetInt32(reader.GetOrdinal("PreferredSessionDuration")),

                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                };
            }

            return null;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                INSERT INTO Users
                (
                    Name,
                    Email,
                    PasswordHash,
                    Bio,
                    ExperienceLevel,
                    Availability,
                    PreferredSessionDuration
                )
                OUTPUT INSERTED.UserId
                VALUES
                (
                    @Name,
                    @Email,
                    @PasswordHash,
                    @Bio,
                    @ExperienceLevel,
                    @Availability,
                    @PreferredSessionDuration
                );";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@Bio", (object?)user.Bio ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceLevel", (object?)user.ExperienceLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@Availability", (object?)user.Availability ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@PreferredSessionDuration",
                (object?)user.PreferredSessionDuration ?? DBNull.Value
            );

            int userId = (int)await command.ExecuteScalarAsync();

            return userId;
        }

        public async Task UpdateProfileAsync(
    int userId,
    string? bio,
    string? experienceLevel,
    string? availability,
    int? preferredSessionDuration)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = @"
        UPDATE Users
        SET
            Bio = @Bio,
            ExperienceLevel = @ExperienceLevel,
            Availability = @Availability,
            PreferredSessionDuration = @PreferredSessionDuration
        WHERE UserId = @UserId";

            command.Parameters.AddWithValue("@Bio", (object?)bio ?? DBNull.Value);
            command.Parameters.AddWithValue("@ExperienceLevel", (object?)experienceLevel ?? DBNull.Value);
            command.Parameters.AddWithValue("@Availability", (object?)availability ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredSessionDuration",
                (object?)preferredSessionDuration ?? DBNull.Value);

            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }
    }
}