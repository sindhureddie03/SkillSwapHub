using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class SessionRequestRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SessionRequestRepository(
            DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task AddSessionRequestAsync(
            SessionRequest request)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
                INSERT INTO SessionRequests
                (
                    RequesterUserId,
                    ReceiverUserId,
                    SkillId,
                    Status
                )
                VALUES
                (
                    @RequesterUserId,
                    @ReceiverUserId,
                    @SkillId,
                    'Pending'
                );";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@RequesterUserId",
                request.RequesterUserId
            );

            command.Parameters.AddWithValue(
                "@ReceiverUserId",
                request.ReceiverUserId
            );

            command.Parameters.AddWithValue(
                "@SkillId",
                request.SkillId
            );

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<object>> GetReceivedRequestsAsync(int userId)
        {
            var requests = new List<object>();

            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        SELECT
            SR.SessionRequestId,
            SR.RequesterUserId,
            U.Name AS RequesterName,
            U.Email AS RequesterEmail,
            SR.SkillId,
            S.SkillName,
            SR.Status,
            SR.RequestedAt
        FROM SessionRequests SR

        INNER JOIN Users U
            ON SR.RequesterUserId = U.UserId

        INNER JOIN Skills S
            ON SR.SkillId = S.SkillId

        WHERE SR.ReceiverUserId = @UserId

        ORDER BY SR.RequestedAt DESC;";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@UserId",
                userId);

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                requests.Add(new
                {
                    SessionRequestId =
                        reader.GetInt32(
                            reader.GetOrdinal("SessionRequestId")),

                    RequesterUserId =
                        reader.GetInt32(
                            reader.GetOrdinal("RequesterUserId")),

                    RequesterName =
                        reader.GetString(
                            reader.GetOrdinal("RequesterName")),

                    RequesterEmail =
                        reader.GetString(
                            reader.GetOrdinal("RequesterEmail")),

                    SkillId =
                        reader.GetInt32(
                            reader.GetOrdinal("SkillId")),

                    SkillName =
                        reader.GetString(
                            reader.GetOrdinal("SkillName")),

                    Status =
                        reader.GetString(
                            reader.GetOrdinal("Status")),

                    RequestedAt =
                        reader.GetDateTime(
                            reader.GetOrdinal("RequestedAt"))
                });
            }

            return requests;
        }

        public async Task<bool> UpdateRequestStatusAsync(
    int sessionRequestId,
    int receiverUserId,
    string status)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            await connection.OpenAsync();

            string query = @"
        UPDATE SessionRequests
        SET Status = @Status
        WHERE
            SessionRequestId = @SessionRequestId
            AND ReceiverUserId = @ReceiverUserId
            AND Status = 'Pending';";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SessionRequestId",
                sessionRequestId);

            command.Parameters.AddWithValue(
                "@ReceiverUserId",
                receiverUserId);

            command.Parameters.AddWithValue(
                "@Status",
                status);

            int rowsAffected =
                await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}