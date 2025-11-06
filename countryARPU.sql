WITH total_revenue_country AS (
    SELECT 
        u.country,
        SUM(p.price) AS total_revenue
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY u.country
),
total_users_country AS (
    SELECT 
        u.country,
        COUNT(DISTINCT s.player_id) AS total_users
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY u.country
)
SELECT 
    tu.country,
    ROUND(COALESCE(tr.total_revenue, 0) / tu.total_users, 2) AS arpu
FROM total_users_country tu
LEFT JOIN total_revenue_country tr ON tu.country = tr.country
ORDER BY arpu DESC;