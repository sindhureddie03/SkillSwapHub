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
    }
}