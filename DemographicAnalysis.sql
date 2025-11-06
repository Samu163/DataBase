WITH revenue_by_country AS (
    SELECT 
        u.country,
        SUM(p.price) AS total_revenue,
        COUNT(DISTINCT s.player_id) AS total_users
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY u.country
)
SELECT 
    country,
    total_revenue,
    total_users,
    ROUND(total_revenue / total_users, 2) AS arpu_by_country
FROM revenue_by_country
ORDER BY arpu_by_country DESC;

WITH revenue_by_age AS (
    SELECT 
        CASE 
            WHEN u.age < 18 THEN 'Under 18'
            WHEN u.age BETWEEN 18 AND 24 THEN '18-24'
            WHEN u.age BETWEEN 25 AND 34 THEN '25-34'
            WHEN u.age BETWEEN 35 AND 44 THEN '35-44'
            ELSE '45+'
        END AS age_group,
        SUM(p.price) AS total_revenue,
        COUNT(DISTINCT s.player_id) AS total_users
    FROM Purchases p
    JOIN Sesions s ON p.session_id = s.session_id
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY age_group
)
SELECT 
    age_group,
    total_revenue,
    total_users,
    ROUND(total_revenue / total_users, 2) AS arpu_by_age
FROM revenue_by_age
ORDER BY age_group;

-- Retención D1 por género
WITH first_session AS (
    SELECT 
        s.player_id,
        DATE(MIN(s.start)) AS first_day,
        u.gender
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY s.player_id, u.gender
),
returned_d1 AS (
    SELECT DISTINCT
        fs.player_id,
        fs.first_day,
        fs.gender
    FROM first_session fs
    INNER JOIN Sesions s 
        ON fs.player_id = s.player_id 
        AND DATE(s.start) = DATE_ADD(fs.first_day, INTERVAL 1 DAY)
)
SELECT 
    CASE 
        WHEN fs.gender = 1 THEN 'Male'
        WHEN fs.gender = 0 THEN 'Female'
        ELSE 'Other'
    END AS gender,
    COUNT(DISTINCT fs.player_id) AS new_users,
    COUNT(DISTINCT rd.player_id) AS returned_d1,
    ROUND((COUNT(DISTINCT rd.player_id) / COUNT(DISTINCT fs.player_id)) * 100, 2) AS d1_retention_percentage
FROM first_session fs
LEFT JOIN returned_d1 rd ON fs.player_id = rd.player_id
GROUP BY fs.gender
ORDER BY d1_retention_percentage DESC;

-- Duración promedio de sesión por país
SELECT 
    u.country,
    COUNT(*) AS total_sessions,
    ROUND(AVG(TIMESTAMPDIFF(SECOND, s.start, s.end)) / 60, 2) AS avg_session_length_minutes
FROM Sesions s
JOIN UsersData u ON s.player_id = u.user_id
WHERE s.end IS NOT NULL
GROUP BY u.country
ORDER BY avg_session_length_minutes DESC;