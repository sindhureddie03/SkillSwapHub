using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class SwapRequestRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SwapRequestRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<int> CreateSwapRequestAsync(SwapRequest request)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
                INSERT INTO SwapRequests
                (
                    SenderId,
                    ReceiverId,
                    OfferedSkillId,
                    RequestedSkillId,
                    Message,
                    PreferredDate,
                    PreferredTime,
                    DurationMinutes,
                    Status,
                    CreatedAt
                )
                OUTPUT INSERTED.SwapRequestId
                VALUES
                (
                    @SenderId,
                    @ReceiverId,
                    @OfferedSkillId,
                    @RequestedSkillId,
                    @Message,
                    @PreferredDate,
                    @PreferredTime,
                    @DurationMinutes,
                    'PENDING',
                    GETDATE()
                )";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@SenderId", request.SenderId);
            command.Parameters.AddWithValue("@ReceiverId", request.ReceiverId);
            command.Parameters.AddWithValue("@OfferedSkillId", request.OfferedSkillId);
            command.Parameters.AddWithValue("@RequestedSkillId", request.RequestedSkillId);
            command.Parameters.AddWithValue("@Message",
                (object?)request.Message ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredDate",
                (object?)request.PreferredDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@PreferredTime",
                (object?)request.PreferredTime ?? DBNull.Value);
            command.Parameters.AddWithValue("@DurationMinutes",
                (object?)request.DurationMinutes ?? DBNull.Value);

            await connection.OpenAsync();

            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<List<SwapRequest>> GetReceivedRequestsAsync(int receiverId)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
                SELECT
                    SwapRequestId,
                    SenderId,
                    ReceiverId,
                    OfferedSkillId,
                    RequestedSkillId,
                    Message,
                    PreferredDate,
                    PreferredTime,
                    DurationMinutes,
                    Status,
                    CreatedAt,
                    UpdatedAt
                FROM SwapRequests
                WHERE ReceiverId = @ReceiverId
                ORDER BY CreatedAt DESC";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ReceiverId", receiverId);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            var requests = new List<SwapRequest>();

            while (await reader.ReadAsync())
            {
                requests.Add(new SwapRequest
                {
                    SwapRequestId = reader.GetInt32(0),
                    SenderId = reader.GetInt32(1),
                    ReceiverId = reader.GetInt32(2),
                    OfferedSkillId = reader.GetInt32(3),
                    RequestedSkillId = reader.GetInt32(4),
                    Message = reader.IsDBNull(5) ? null : reader.GetString(5),
                    PreferredDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    PreferredTime = reader.IsDBNull(7) ? null : reader.GetTimeSpan(7),
                    DurationMinutes = reader.IsDBNull(8) ? null : reader.GetInt32(8),
                    Status = reader.GetString(9),
                    CreatedAt = reader.GetDateTime(10),
                    UpdatedAt = reader.IsDBNull(11) ? null : reader.GetDateTime(11)
                });
            }

            return requests;
        }

        public async Task<bool> UpdateStatusAsync(int swapRequestId, string status)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            string query = @"
                UPDATE SwapRequests
                SET Status = @Status,
                    UpdatedAt = GETDATE()
                WHERE SwapRequestId = @SwapRequestId";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@SwapRequestId", swapRequestId);
            command.Parameters.AddWithValue("@Status", status);

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}