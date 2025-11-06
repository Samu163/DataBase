WITH total_paying_country AS (
    SELECT 
        u.country,
        COUNT(DISTINCT s.player_id) AS paying_users,
        SUM(p.price) AS total_revenue
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY u.country
)
SELECT 
    country,
    ROUND(total_revenue / paying_users, 2) AS arppu
FROM total_paying_country
ORDER BY arppu DESC;