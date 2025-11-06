WITH first_session AS (
    SELECT 
        player_id,
        DATE(MIN(start)) AS first_day
    FROM Sesions
    GROUP BY player_id
),
returned_d1 AS (
    SELECT DISTINCT
        fs.player_id,
        fs.first_day
    FROM first_session fs
    INNER JOIN Sesions s 
        ON fs.player_id = s.player_id 
        AND DATE(s.start) = DATE_ADD(fs.first_day, INTERVAL 1 DAY)
)
SELECT 
    fs.first_day AS cohort_date,
    COUNT(DISTINCT fs.player_id) AS new_users,
    COUNT(DISTINCT rd.player_id) AS returned_d1,
    ROUND((COUNT(DISTINCT rd.player_id) / COUNT(DISTINCT fs.player_id)) * 100, 2) AS d1_retention_percentage
FROM first_session fs
LEFT JOIN returned_d1 rd ON fs.player_id = rd.player_id
GROUP BY fs.first_day
ORDER BY fs.first_day;