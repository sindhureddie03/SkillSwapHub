using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class SkillRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SkillRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            var skills = new List<Skill>();

            using var connection = _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                SELECT
                    SkillId,
                    SkillName,
                    Category,
                    Description,
                    CreatedAt
                FROM Skills
                ORDER BY SkillName;";

            using var command = new SqlCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                skills.Add(new Skill
                {
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

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt"))
                });
            }

            return skills;
        }

        public async Task<int> CreateSkillAsync(Skill skill)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
        INSERT INTO Skills
        (SkillName, Category, Description, CreatedAt)
        OUTPUT INSERTED.SkillId
        VALUES
        (@SkillName, @Category, @Description, GETUTCDATE());";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@SkillName", skill.SkillName);
            command.Parameters.AddWithValue("@Category",
                (object?)skill.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description",
                (object?)skill.Description ?? DBNull.Value);

            await connection.OpenAsync();

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public async Task<Skill?> GetSkillByIdAsync(int skillId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
        SELECT SkillId, SkillName, Category, Description, CreatedAt
        FROM Skills
        WHERE SkillId = @SkillId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@SkillId", skillId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Skill
            {
                SkillId = reader.GetInt32(reader.GetOrdinal("SkillId")),
                SkillName = reader.GetString(reader.GetOrdinal("SkillName")),
                Category = reader.IsDBNull(reader.GetOrdinal("Category"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Category")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        public async Task<bool> UpdateSkillAsync(Skill skill)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
        UPDATE Skills
        SET SkillName = @SkillName,
            Category = @Category,
            Description = @Description
        WHERE SkillId = @SkillId";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@SkillId", skill.SkillId);
            command.Parameters.AddWithValue("@SkillName", skill.SkillName);
            command.Parameters.AddWithValue("@Category",
                (object?)skill.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description",
                (object?)skill.Description ?? DBNull.Value);

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteSkillAsync(int skillId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
        DELETE FROM Skills
        WHERE SkillId = @SkillId";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@SkillId", skillId);

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> CompleteSessionAsync(
    int sessionId,
    int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
        UPDATE s
        SET
            TeacherCompleted =
                CASE
                    WHEN sr.ReceiverUserId = @UserId
                    THEN 1
                    ELSE s.TeacherCompleted
                END,

            LearnerCompleted =
                CASE
                    WHEN sr.RequesterUserId = @UserId
                    THEN 1
                    ELSE s.LearnerCompleted
                END,

            Status =
                CASE
                    WHEN
                        (
                            sr.ReceiverUserId = @UserId
                            AND s.LearnerCompleted = 1
                        )
                        OR
                        (
                            sr.RequesterUserId = @UserId
                            AND s.TeacherCompleted = 1
                        )
                    THEN 'COMPLETED'
                    ELSE s.Status
                END,

            UpdatedAt = GETDATE()

        FROM Sessions s

        INNER JOIN SessionRequests sr
            ON s.SessionRequestId =
               sr.SessionRequestId

        WHERE s.SessionId = @SessionId
          AND
          (
              sr.RequesterUserId = @UserId
              OR
              sr.ReceiverUserId = @UserId
          )";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SessionId",
                sessionId);

            command.Parameters.AddWithValue(
                "@UserId",
                userId);

            await connection.OpenAsync();

            int rows =
                await command.ExecuteNonQueryAsync();

            return rows > 0;
        }
    }
}