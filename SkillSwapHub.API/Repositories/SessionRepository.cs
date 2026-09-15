using Microsoft.Data.SqlClient;
using SkillSwapHub.API.Data;
using SkillSwapHub.API.Models;

namespace SkillSwapHub.API.Repositories
{
    public class SessionRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public SessionRepository(
            DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        // Schedule Session
        public async Task<int> ScheduleSessionAsync(
            Session session)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
                INSERT INTO Sessions
                (
                    SessionRequestId,
                    ScheduledDate,
                    StartTime,
                    DurationMinutes,
                    Status,
                    CreatedAt
                )
                OUTPUT INSERTED.SessionId
                VALUES
                (
                    @SessionRequestId,
                    @ScheduledDate,
                    @StartTime,
                    @DurationMinutes,
                    'SCHEDULED',
                    GETDATE()
                )";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@SessionRequestId",
                session.SessionRequestId);

            command.Parameters.AddWithValue(
                "@ScheduledDate",
                session.ScheduledDate);

            command.Parameters.AddWithValue(
                "@StartTime",
                session.StartTime);

            command.Parameters.AddWithValue(
                "@DurationMinutes",
                session.DurationMinutes);

            await connection.OpenAsync();

            return (int)await command.ExecuteScalarAsync();
        }


        // Get Upcoming Sessions
        public async Task<List<Session>> GetUpcomingSessionsAsync(
    int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
        SELECT
            s.SessionId,
            s.SessionRequestId,
            sr.RequesterUserId,
            sr.ReceiverUserId,
            s.ScheduledDate,
            s.StartTime,
            s.DurationMinutes,
            s.MeetingLink,
            s.TeacherJoinedAt,
            s.LearnerJoinedAt,

            CASE
                WHEN sr.RequesterUserId = @UserId
                    THEN s.LearnerJoinedAt
                WHEN sr.ReceiverUserId = @UserId
                    THEN s.TeacherJoinedAt
            END AS CurrentUserJoinedAt,

            CASE
                WHEN sr.RequesterUserId = @UserId
                    THEN s.LearnerCompleted
                WHEN sr.ReceiverUserId = @UserId
                    THEN s.TeacherCompleted
            END AS CurrentUserCompleted,

            s.TeacherCompleted,
            s.LearnerCompleted,
            s.Status,
            s.CreatedAt,
            s.UpdatedAt

        FROM Sessions s

        INNER JOIN SessionRequests sr
            ON s.SessionRequestId =
               sr.SessionRequestId

        WHERE
            (
                sr.RequesterUserId = @UserId
                OR sr.ReceiverUserId = @UserId
            )
            AND s.ScheduledDate >=
                CAST(GETDATE() AS DATE)

            AND s.Status IN
                ('SCHEDULED', 'IN_PROGRESS')

        ORDER BY
            s.ScheduledDate,
            s.StartTime";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@UserId",
                userId);

            await connection.OpenAsync();

            using var reader =
                await command.ExecuteReaderAsync();

            var sessions =
                new List<Session>();

            while (await reader.ReadAsync())
            {
                sessions.Add(new Session
                {
                    SessionId =
                        reader.GetInt32(0),

                    SessionRequestId =
                        reader.GetInt32(1),

                    RequesterUserId =
                        reader.GetInt32(2),

                    ReceiverUserId =
                        reader.GetInt32(3),

                    ScheduledDate =
                        reader.GetDateTime(4),

                    StartTime =
                        reader.GetTimeSpan(5),

                    DurationMinutes =
                        reader.GetInt32(6),

                    MeetingLink =
                        reader.IsDBNull(7)
                            ? null
                            : reader.GetString(7),

                    TeacherJoinedAt =
                        reader.IsDBNull(8)
                            ? null
                            : reader.GetDateTime(8),

                    LearnerJoinedAt =
                        reader.IsDBNull(9)
                            ? null
                            : reader.GetDateTime(9),

                    CurrentUserJoinedAt =
                        reader.IsDBNull(10)
                            ? null
                            : reader.GetDateTime(10),

                    CurrentUserCompleted =
                        reader.IsDBNull(11)
                            ? false
                            : reader.GetBoolean(11),

                    TeacherCompleted =
                        reader.GetBoolean(12),

                    LearnerCompleted =
                        reader.GetBoolean(13),

                    Status =
                        reader.GetString(14),

                    CreatedAt =
                        reader.GetDateTime(15),

                    UpdatedAt =
                        reader.IsDBNull(16)
                            ? null
                            : reader.GetDateTime(16)
                });
            }

            return sessions;
        }

        public async Task<List<Session>> GetCompletedSessionsAsync(int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
        SELECT
            s.SessionId,
            s.SessionRequestId,
            sr.RequesterUserId,
            sr.ReceiverUserId,
            s.ScheduledDate,
            s.StartTime,
            s.DurationMinutes,
            s.MeetingLink,
            s.TeacherJoinedAt,
            s.LearnerJoinedAt,

            CASE
                WHEN sr.RequesterUserId = @UserId
                    THEN s.LearnerCompleted
                WHEN sr.ReceiverUserId = @UserId
                    THEN s.TeacherCompleted
            END AS CurrentUserCompleted,

            s.TeacherCompleted,
            s.LearnerCompleted,
            s.Status,
            s.CreatedAt,
            s.UpdatedAt

        FROM Sessions s

        INNER JOIN SessionRequests sr
            ON s.SessionRequestId =
               sr.SessionRequestId

        WHERE
            (
                sr.RequesterUserId = @UserId
                OR sr.ReceiverUserId = @UserId
            )
            AND s.Status = 'COMPLETED'

        ORDER BY
            s.ScheduledDate DESC,
            s.StartTime DESC";

            using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@UserId",
                userId);

            await connection.OpenAsync();

            using var reader =
                await command.ExecuteReaderAsync();

            var sessions =
                new List<Session>();

            while (await reader.ReadAsync())
            {
                sessions.Add(new Session
                {
                    SessionId =
                        reader.GetInt32(0),

                    SessionRequestId =
                        reader.GetInt32(1),

                    RequesterUserId =
                        reader.GetInt32(2),

                    ReceiverUserId =
                        reader.GetInt32(3),

                    ScheduledDate =
                        reader.GetDateTime(4),

                    StartTime =
                        reader.GetTimeSpan(5),

                    DurationMinutes =
                        reader.GetInt32(6),

                    MeetingLink =
                        reader.IsDBNull(7)
                            ? null
                            : reader.GetString(7),

                    TeacherJoinedAt =
                        reader.IsDBNull(8)
                            ? null
                            : reader.GetDateTime(8),

                    LearnerJoinedAt =
                        reader.IsDBNull(9)
                            ? null
                            : reader.GetDateTime(9),

                    CurrentUserCompleted =
                        reader.IsDBNull(10)
                            ? false
                            : reader.GetBoolean(10),

                    TeacherCompleted =
                        reader.GetBoolean(11),

                    LearnerCompleted =
                        reader.GetBoolean(12),

                    Status =
                        reader.GetString(13),

                    CreatedAt =
                        reader.GetDateTime(14),

                    UpdatedAt =
                        reader.IsDBNull(15)
                            ? null
                            : reader.GetDateTime(15)
                });
            }

            return sessions;
        }


        // Join Session
        public async Task<bool> JoinSessionAsync(
            int sessionId,
            int userId)
        {
            using var connection =
                _dbConnectionFactory.CreateConnection();

            string query = @"
                UPDATE s
                SET
                    TeacherJoinedAt =
                        CASE
                            WHEN sr.ReceiverUserId = @UserId
                                 AND s.TeacherJoinedAt IS NULL
                            THEN GETDATE()
                            ELSE s.TeacherJoinedAt
                        END,

                    LearnerJoinedAt =
                        CASE
                            WHEN sr.RequesterUserId = @UserId
                                 AND s.LearnerJoinedAt IS NULL
                            THEN GETDATE()
                            ELSE s.LearnerJoinedAt
                        END,

                    Status =
                        CASE
                            WHEN
                                (
                                    sr.ReceiverUserId = @UserId
                                    AND s.LearnerJoinedAt IS NOT NULL
                                )
                                OR
                                (
                                    sr.RequesterUserId = @UserId
                                    AND s.TeacherJoinedAt IS NOT NULL
                                )
                            THEN 'IN_PROGRESS'

                            ELSE s.Status
                        END,

                    UpdatedAt = GETDATE()

                FROM Sessions s

                INNER JOIN SessionRequests sr
                    ON s.SessionRequestId =
                       sr.SessionRequestId

                WHERE
                    s.SessionId = @SessionId

                    AND
                    (
                        sr.RequesterUserId = @UserId
                        OR
                        sr.ReceiverUserId = @UserId
                    )

                    AND s.Status IN
                        ('SCHEDULED', 'IN_PROGRESS')";

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


        // Complete Session
        // Complete Session
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

        WHERE
            s.SessionId = @SessionId

            AND
            (
                sr.RequesterUserId = @UserId
                OR sr.ReceiverUserId = @UserId
            )

            AND
            (
                (sr.RequesterUserId = @UserId
                 AND s.LearnerJoinedAt IS NOT NULL)

                OR

                (sr.ReceiverUserId = @UserId
                 AND s.TeacherJoinedAt IS NOT NULL)
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