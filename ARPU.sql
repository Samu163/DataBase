WITH daily_revenue AS (
    SELECT 
        DATE(p.dayTime) AS day,
        SUM(p.price) AS total_revenue
    FROM Purchases p
    GROUP BY DATE(p.dayTime)
),
daily_users AS (
    SELECT 
        DATE(s.start) AS day,
        COUNT(DISTINCT s.player_id) AS total_users
    FROM Sesions s
    GROUP BY DATE(s.start)
)
SELECT 
    du.day,
    COALESCE(dr.total_revenue, 0) AS total_revenue,
    du.total_users,
    ROUND(COALESCE(dr.total_revenue, 0) / du.total_users, 2) AS arpu
FROM daily_users du
LEFT JOIN daily_revenue dr ON du.day = dr.day
ORDER BY du.day;
