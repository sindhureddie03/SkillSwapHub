using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;

namespace SkillSwapHub.API.Repositories
{
    public class MatchRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public MatchRepository(
            DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<List<object>> GetMatchesAsync(int userId)
        {
            var matches = new List<object>();

            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                SELECT
                    OtherUser.UserId,
                    OtherUser.Name,
                    OtherUser.Email,
                    Skills.SkillId,
                    Skills.SkillName,
                    OtherUserSkill.SkillLevel
                FROM UserSkills MySkill

                INNER JOIN UserSkills OtherUserSkill
                    ON MySkill.SkillId = OtherUserSkill.SkillId

                INNER JOIN Users OtherUser
                    ON OtherUserSkill.UserId = OtherUser.UserId

                INNER JOIN Skills
                    ON MySkill.SkillId = Skills.SkillId

                WHERE
                    MySkill.UserId = @UserId
                    AND MySkill.SkillType = 'WANT'

                    AND OtherUserSkill.SkillType = 'OFFER'

                    AND OtherUser.UserId <> @UserId

                ORDER BY
                    Skills.SkillName,
                    OtherUser.Name;";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@UserId",
                userId
            );

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                matches.Add(new
                {
                    UserId = reader.GetInt32(
                        reader.GetOrdinal("UserId")
                    ),

                    Name = reader.GetString(
                        reader.GetOrdinal("Name")
                    ),

                    Email = reader.GetString(
                        reader.GetOrdinal("Email")
                    ),

                    SkillId = reader.GetInt32(
                        reader.GetOrdinal("SkillId")
                    ),

                    SkillName = reader.GetString(
                        reader.GetOrdinal("SkillName")
                    ),

                    SkillLevel = reader.IsDBNull(
                        reader.GetOrdinal("SkillLevel")
                    )
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("SkillLevel")
                    )
                });
            }

            return matches;
        }
    }
}