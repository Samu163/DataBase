SELECT
    DATE(start) AS session_date,
    COUNT(session_id) AS number_of_sessions
FROM
    Sesions
 GROUP BY DATE(start)
