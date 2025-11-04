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

if (!isset($data['session_id']) || !isset($data['end'])) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields (session_id, end)"
    ]);
    exit;
}
 
$session_id = (int)$data['session_id']; 
$end = $conn->real_escape_string($data['end']); 

// Verificar que la sesión existe en Sesions (con S mayúscula)
$check_sql = "SELECT session_id FROM Sesions WHERE session_id = $session_id";
$result = $conn->query($check_sql);

if ($result->num_rows == 0) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Session ID $session_id does not exist"
    ]);
    $conn->close();
    exit;
}

$sql = "UPDATE Sesions 
        SET end = '$end' 
        WHERE session_id = $session_id"; 

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
        "error" => $conn->error
    ]);
} 

$conn->close(); 
?>