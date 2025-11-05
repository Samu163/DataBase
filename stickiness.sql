WITH dau_daily AS (
    SELECT 
        DATE(start) AS day,
        COUNT(DISTINCT player_id) AS dau
    FROM Sesions
    GROUP BY DATE(start)
),

usuarios_por_dia AS (
    SELECT 
        DATE(start) AS activity_date,
        player_id
    FROM Sesions
)

SELECT 
    d.day,
    d.dau,
    COUNT(DISTINCT u.player_id) AS mau_30_days,
    ROUND(d.dau / COUNT(DISTINCT u.player_id) * 100, 2) AS stickiness_percent
FROM dau_daily d
LEFT JOIN usuarios_por_dia u 
    ON u.activity_date <= d.day 
    AND u.activity_date > DATE_SUB(d.day, INTERVAL 30 DAY)
GROUP BY d.day, d.dau
ORDER BY d.day;