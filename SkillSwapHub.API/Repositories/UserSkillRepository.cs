using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class UserSkillRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public UserSkillRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task AddUserSkillAsync(UserSkill userSkill)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                INSERT INTO UserSkills
                (
                    UserId,
                    SkillId,
                    SkillType,
                    SkillLevel
                )
                VALUES
                (
                    @UserId,
                    @SkillId,
                    @SkillType,
                    @SkillLevel
                );";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userSkill.UserId);
            command.Parameters.AddWithValue("@SkillId", userSkill.SkillId);
            command.Parameters.AddWithValue("@SkillType", userSkill.SkillType);
            command.Parameters.AddWithValue(
                "@SkillLevel",
                (object?)userSkill.SkillLevel ?? DBNull.Value
            );

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<object>> GetMySkillsAsync(int userId)
        {
            var skills = new List<object>();

            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        SELECT
            US.UserSkillId,
            US.SkillId,
            S.SkillName,
            S.Category,
            S.Description,
            US.SkillType,
            US.SkillLevel,
            US.CreatedAt
        FROM UserSkills US
        INNER JOIN Skills S
            ON US.SkillId = S.SkillId
        WHERE US.UserId = @UserId
        ORDER BY S.SkillName;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                skills.Add(new
                {
                    UserSkillId = reader.GetInt32(
                        reader.GetOrdinal("UserSkillId")),

                    SkillId = reader.GetInt32(
                        reader.GetOrdinal("SkillId")),

                    SkillName = reader.GetString(
                        reader.GetOrdinal("SkillName")),

                    Category = reader.IsDBNull(
                        reader.GetOrdinal("Category"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Category")),

                    Description = reader.IsDBNull(
                        reader.GetOrdinal("Description"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("Description")),

                    SkillType = reader.GetString(
                        reader.GetOrdinal("SkillType")),

                    SkillLevel = reader.IsDBNull(
                        reader.GetOrdinal("SkillLevel"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("SkillLevel")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt"))
                });
            }

            return skills;
        }

        public async Task<bool> UpdateUserSkillAsync(
    int userSkillId,
    int userId,
    string skillType,
    string? skillLevel)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        UPDATE UserSkills
        SET
            SkillType = @SkillType,
            SkillLevel = @SkillLevel
        WHERE
            UserSkillId = @UserSkillId
            AND UserId = @UserId;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserSkillId", userSkillId);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SkillType", skillType);
            command.Parameters.AddWithValue(
                "@SkillLevel",
                (object?)skillLevel ?? DBNull.Value
            );

            int rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }


        public async Task<bool> DeleteUserSkillAsync(
            int userSkillId,
            int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        DELETE FROM UserSkills
        WHERE
            UserSkillId = @UserSkillId
            AND UserId = @UserId;";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserSkillId", userSkillId);
            command.Parameters.AddWithValue("@UserId", userId);

            int rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}