SELECT 
    DATE(start) AS day,
    COUNT(DISTINCT player_id) AS dau
FROM Sesions
GROUP BY DATE(start)
ORDER BY day;