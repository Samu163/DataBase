WITH age_ranges AS (
    SELECT 
        s.player_id,
        DATE(s.start) AS day,
        CASE 
            WHEN u.age <= 18 THEN '0-18'
            WHEN u.age BETWEEN 19 AND 40 THEN '19-40'
            WHEN u.age BETWEEN 41 AND 60 THEN '41-60'
            ELSE '61+'
        END AS age_range
    FROM Sesions s
    JOIN UsersData u ON s.player_id = u.user_id
),
dau_daily_range AS (
    SELECT 
        day,
        age_range,
        COUNT(DISTINCT player_id) AS dau
    FROM age_ranges
    GROUP BY day, age_range
),
mau_daily_range AS (
    SELECT 
        d.day,
        d.age_range,
        COUNT(DISTINCT ar.player_id) AS mau
    FROM (
        SELECT DISTINCT day, age_range
        FROM age_ranges
    ) d
    LEFT JOIN age_ranges ar 
        ON ar.age_range = d.age_range
        AND ar.day <= d.day 
        AND ar.day > DATE_SUB(d.day, INTERVAL 30 DAY)
    GROUP BY d.day, d.age_range
),
stickiness_daily AS (
    SELECT 
        dau.age_range,
        dau.day,
        dau.dau,
        mau.mau,
        (dau.dau / mau.mau) * 100 AS stickiness_percentage
    FROM dau_daily_range dau
    JOIN mau_daily_range mau ON dau.day = mau.day AND dau.age_range = mau.age_range
)
SELECT 
    age_range,
    ROUND(AVG(stickiness_percentage), 2) AS avg_stickiness
FROM stickiness_daily
GROUP BY age_range
ORDER BY age_range;