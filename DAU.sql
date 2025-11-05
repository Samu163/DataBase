SELECT 
    DATE(StartTime) AS Dia,
    COUNT(DISTINCT UserID) AS DAU
FROM
    Sessions
GROUP BY
	Dia

