WITH first_session AS (
    SELECT 
        s.player_id,
        u.country,
        DATE(MIN(s.start)) AS first_day
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
    GROUP BY s.player_id, u.country
),
returned_d1 AS (
    SELECT DISTINCT
        fs.player_id,
        fs.country
    FROM first_session fs
    INNER JOIN Sesions s 
        ON fs.player_id = s.player_id 
        AND DATE(s.start) = DATE_ADD(fs.first_day, INTERVAL 1 DAY)
),
returned_d3 AS (
    SELECT DISTINCT
        fs.player_id,
        fs.country
    FROM first_session fs
    INNER JOIN Sesions s 
        ON fs.player_id = s.player_id 
        AND DATE(s.start) = DATE_ADD(fs.first_day, INTERVAL 3 DAY)
),
returned_d7 AS (
    SELECT DISTINCT
        fs.player_id,
        fs.country
    FROM first_session fs
    INNER JOIN Sesions s 
        ON fs.player_id = s.player_id 
        AND DATE(s.start) = DATE_ADD(fs.first_day, INTERVAL 7 DAY)
)
SELECT 
    fs.country,
    ROUND((COUNT(DISTINCT rd1.player_id) / COUNT(DISTINCT fs.player_id)) * 100, 2) AS d1_retention,
    ROUND((COUNT(DISTINCT rd3.player_id) / COUNT(DISTINCT fs.player_id)) * 100, 2) AS d3_retention,
    ROUND((COUNT(DISTINCT rd7.player_id) / COUNT(DISTINCT fs.player_id)) * 100, 2) AS d7_retention
FROM first_session fs
LEFT JOIN returned_d1 rd1 ON fs.player_id = rd1.player_id AND fs.country = rd1.country
LEFT JOIN returned_d3 rd3 ON fs.player_id = rd3.player_id AND fs.country = rd3.country
LEFT JOIN returned_d7 rd7 ON fs.player_id = rd7.player_id AND fs.country = rd7.country
GROUP BY fs.country
ORDER BY d7_retention DESC;