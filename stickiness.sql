WITH dau_daily AS (
    SELECT 
        DATE(start) AS day,
        COUNT(DISTINCT player_id) AS dau
    FROM Sesions
    GROUP BY DATE(start)
),
mau_daily AS (
    SELECT 
        d.day,
        COUNT(DISTINCT s.player_id) AS mau
    FROM (SELECT DISTINCT DATE(start) AS day FROM Sesions) d
    LEFT JOIN Sesions s 
        ON DATE(s.start) <= d.day 
        AND DATE(s.start) > DATE_SUB(d.day, INTERVAL 30 DAY)
    GROUP BY d.day
)
SELECT 
    dau.day,
    dau.dau,
    mau.mau,
    ROUND((dau.dau / mau.mau) * 100, 2) AS stickiness_percentage
FROM dau_daily dau
JOIN mau_daily mau ON dau.day = mau.day
ORDER BY dau.day;

