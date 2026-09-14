using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class ReviewRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public ReviewRepository(
            DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> AddReviewAsync(
            Review review)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
                INSERT INTO Reviews
                (
                    SessionId,
                    ReviewerId,
                    ReviewedUserId,
                    Rating,
                    Comment,
                    CreatedAt
                )
                OUTPUT INSERTED.ReviewId
                VALUES
                (
                    @SessionId,
                    @ReviewerId,
                    @ReviewedUserId,
                    @Rating,
                    @Comment,
                    GETDATE()
                )";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SessionId",
                review.SessionId);

            command.Parameters.AddWithValue(
                "@ReviewerId",
                review.ReviewerId);

            command.Parameters.AddWithValue(
                "@ReviewedUserId",
                review.ReviewedUserId);

            command.Parameters.AddWithValue(
                "@Rating",
                review.Rating);

            command.Parameters.AddWithValue(
                "@Comment",
                (object?)review.Comment ?? DBNull.Value);

            await connection.OpenAsync();

            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<List<Review>> GetReviewsForUserAsync(
    int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
        SELECT
            ReviewId,
            SessionId,
            ReviewerId,
            ReviewedUserId,
            Rating,
            Comment,
            CreatedAt
        FROM Reviews
        WHERE ReviewedUserId = @UserId
        ORDER BY CreatedAt DESC";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@UserId",
                userId);

            await connection.OpenAsync();

            using var reader =
                await command.ExecuteReaderAsync();

            var reviews = new List<Review>();

            while (await reader.ReadAsync())
            {
                reviews.Add(new Review
                {
                    ReviewId = reader.GetInt32(0),
                    SessionId = reader.GetInt32(1),
                    ReviewerId = reader.GetInt32(2),
                    ReviewedUserId = reader.GetInt32(3),
                    Rating = reader.GetInt32(4),
                    Comment = reader.IsDBNull(5)
                        ? null
                        : reader.GetString(5),
                    CreatedAt = reader.GetDateTime(6)
                });
            }

            return reviews;
        }
    }
}