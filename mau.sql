WITH dau_daily AS (
    SELECT 
        DATE(start) AS day,
        COUNT(DISTINCT player_id) AS dau,
        COUNT(*) AS total_sessions
    FROM Sesions
    GROUP BY DATE(start)
),

usuarios_por_dia AS (
    SELECT DISTINCT
        DATE(start) AS activity_date,
        player_id
    FROM Sesions
)

SELECT 
    d.day,
    d.dau AS usuarios_unicos_dia,
    d.total_sessions AS sesiones_totales_dia,
    COUNT(DISTINCT u.player_id) AS mau_30_days,
    COUNT(u.player_id) AS registros_en_ventana_30dias
FROM dau_daily d
LEFT JOIN usuarios_por_dia u 
    ON u.activity_date <= d.day 
    AND u.activity_date > DATE_SUB(d.day, INTERVAL 30 DAY)
GROUP BY d.day, d.dau, d.total_sessions
ORDER BY d.day;