WITH daily_paying AS (
    SELECT 
        DATE(p.dayTime) AS day,
        COUNT(DISTINCT s.player_id) AS paying_users,
        SUM(p.price) AS total_revenue
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
    GROUP BY DATE(p.dayTime)
)
SELECT 
    day,
    total_revenue,
    paying_users,
    ROUND(total_revenue / paying_users, 2) AS arppu
FROM daily_paying
ORDER BY day;