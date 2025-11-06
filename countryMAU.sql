WITH dau_daily_country AS (
    SELECT 
        DATE(s.start) AS day,
        u.country,
        COUNT(DISTINCT s.player_id) AS dau
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY DATE(s.start), u.country
),
usuarios_por_dia_country AS (
    SELECT 
        DATE(s.start) AS activity_date,
        u.country,
        s.player_id
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
),
mau_calculation AS (
    SELECT 
        d.day,
        d.country,
        d.dau,
        COUNT(DISTINCT u.player_id) AS mau_30_days
    FROM dau_daily_country d
    LEFT JOIN usuarios_por_dia_country u 
        ON u.country = d.country
        AND u.activity_date <= d.day 
        AND u.activity_date > DATE_SUB(d.day, INTERVAL 30 DAY)
    GROUP BY d.day, d.country, d.dau
)
SELECT 
    country,
    ROUND(AVG(mau_30_days), 0) AS avg_mau
FROM mau_calculation
GROUP BY country
ORDER BY avg_mau DESC;