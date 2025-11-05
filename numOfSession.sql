SELECT
    DATE(StartTime) AS session_date,
    COUNT(SessionID) AS number_of_sessions
FROM
    Sessions
GROUP BY
    session_date
