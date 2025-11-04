<?php
header('Content-Type: application/json; charset=UTF-8');

$servername = "citmalumnes.upc.es"; 
$username = "samuelm1"; 
$password = "SRhcWbnAPYdN"; 
$database = "samuelm1"; 

$conn = new mysqli($servername, $username, $password, $database); 

// Comprobar conexión 
if ($conn->connect_error) { 
    http_response_code(500);
    echo json_encode([
        "success" => false,
        "error" => "Connection failed: " . $conn->connect_error
    ]);
    exit;
} 

$json = file_get_contents('php://input'); 
$data = json_decode($json, true); 

if (!$data || empty($json)) { 
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Invalid JSON received",
        "received" => $json
    ]);
    exit;
} 

if (!isset($data['player_id']) || !isset($data['start'])) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields (player_id, start)"
    ]);
    exit;
}

$player_id = (int)$data['player_id']; 
$start = $conn->real_escape_string($data['start']); 

$check_sql = "SELECT user_id FROM UsersData WHERE user_id = $player_id";
$result = $conn->query($check_sql);

if ($result->num_rows == 0) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Player ID $player_id does not exist"
    ]);
    $conn->close();
    exit;
}

$sql = "INSERT INTO Sesions (player_id, start, end) 
        VALUES ($player_id, '$start', NULL)"; 

if ($conn->query($sql) === TRUE) {
    echo json_encode([
        "success" => true,
        "message" => "Session started successfully",
        "session_id" => $conn->insert_id
    ]);
} else { 
    http_response_code(500);
    echo json_encode([
        "success" => false,
        "error" => $conn->error
    ]);
} 

$conn->close(); 
?>