
WITH daily_active AS (
    SELECT 
        u.country,
        DATE(s.start) AS date,
        COUNT(DISTINCT s.player_id) AS dau
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY u.country, DATE(s.start)
)
SELECT 
    country,
    ROUND(AVG(dau), 2) AS dau
FROM daily_active
GROUP BY country
ORDER BY avg_dau DESC