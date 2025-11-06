SELECT 
    DATE(start) AS day,
    COUNT(*) AS total_sessions,
    COUNT(DISTINCT player_id) AS unique_players,
    ROUND(COUNT(*) / COUNT(DISTINCT player_id), 2) AS avg_sessions_per_user
FROM Sesions
GROUP BY DATE(start)
ORDER BY day;

SELECT 
    DATE(start) AS day,
    COUNT(*) AS total_sessions,
    ROUND(AVG(TIMESTAMPDIFF(SECOND, start, end)) / 60, 2) AS avg_session_length_minutes,
    ROUND(MIN(TIMESTAMPDIFF(SECOND, start, end)) / 60, 2) AS min_session_length_minutes,
    ROUND(MAX(TIMESTAMPDIFF(SECOND, start, end)) / 60, 2) AS max_session_length_minutes
FROM Sesions
WHERE end IS NOT NULL
GROUP BY DATE(start)
ORDER BY day;
