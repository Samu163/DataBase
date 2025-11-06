WITH paying_users AS (
    SELECT DISTINCT s.player_id
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
),
total_users AS (
    SELECT COUNT(DISTINCT player_id) AS total
    FROM Sesions
)
SELECT 
    (SELECT COUNT(*) FROM paying_users) AS paying_users,
    tu.total AS total_users,
    ROUND(((SELECT COUNT(*) FROM paying_users) / tu.total) * 100, 2) AS conversion_rate_percentage
FROM total_users tu;