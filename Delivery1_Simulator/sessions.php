<?php
header('Content-Type: application/json; charset=UTF-8');

// Logging temporal para debug
$logFile = __DIR__ . '/sessions_debug.log';
file_put_contents($logFile, date('Y-m-d H:i:s') . " - Request received\n", FILE_APPEND);

$servername = "citmalumnes.upc.es"; 
$username = "samuelm1"; 
$password = "SRhcWbnAPYdN"; 
$database = "samuelm1"; 

$conn = new mysqli($servername, $username, $password, $database); 

if ($conn->connect_error) { 
    http_response_code(500);
    echo json_encode([
        "success" => false,
        "error" => "Connection failed: " . $conn->connect_error
    ]);
    exit;
} 

$json = file_get_contents('php://input'); 
file_put_contents($logFile, "JSON received: " . $json . "\n", FILE_APPEND);

$data = json_decode($json, true); 

// Verificar si es una actualización de fin de sesión
if (isset($_GET['action']) && $_GET['action'] == 'end' && isset($_GET['session_id'])) {
    $session_id = (int)$_GET['session_id'];
    
    if (!$data || !isset($data['end'])) {
        http_response_code(400);
        echo json_encode([
            "success" => false,
            "error" => "Missing end time",
            "received" => $json
        ]);
        exit;
    }
    
    $end = $conn->real_escape_string($data['end']);
    
    $sql = "UPDATE Sesions SET end = '$end' WHERE session_id = $session_id";
    
    file_put_contents($logFile, "SQL: " . $sql . "\n", FILE_APPEND);
    
    if ($conn->query($sql) === TRUE) {
        echo json_encode([
            "success" => true,
            "message" => "Session ended successfully",
            "session_id" => $session_id
        ]);
    } else {
        http_response_code(500);
        echo json_encode([
            "success" => false,
            "error" => $conn->error,
            "sql" => $sql
        ]);
    }
    $conn->close();
    exit;
}

// Nueva sesión
if (!$data) { 
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Invalid JSON",
        "received" => $json
    ]);
    exit;
}

if (!isset($data['player_id']) || !isset($data['start'])) { 
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields (player_id, start)",
        "received_fields" => array_keys($data)
    ]);
    exit;
}

$player_id = (int)$data['player_id'];
$start = $conn->real_escape_string($data['start']);

$sql = "INSERT INTO Sesions (player_id, start) VALUES ($player_id, '$start')"; 

file_put_contents($logFile, "SQL: " . $sql . "\n", FILE_APPEND);

if ($conn->query($sql) === TRUE) {
    $session_id = $conn->insert_id;
    echo json_encode([
        "success" => true,
        "message" => "Session created successfully",
        "session_id" => $session_id,
        "data" => [
            "player_id" => $player_id,
            "session_id" => $session_id,
            "start" => $start,
            "end" => null
        ]
    ]);
    file_put_contents($logFile, "Success: session_id " . $session_id . "\n", FILE_APPEND);
} else { 
    http_response_code(500);
    echo json_encode([
        "success" => false,
        "error" => $conn->error,
        "sql" => $sql
    ]);
    file_put_contents($logFile, "Error: " . $conn->error . "\n", FILE_APPEND);
} 

$conn->close(); 
?>