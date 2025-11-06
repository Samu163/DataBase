<?php
header('Content-Type: application/json; charset=UTF-8');

$servername = "citmalumnes.upc.es"; 
$username = "samuelm1"; 
$password = "SRhcWbnAPYdN"; 
$database = "samuelm1"; 

// Crear conexión 
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

// Leer el cuerpo de la solicitud (JSON) 
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

// Validar campos necesarios
if (!isset($data['session_id']) || !isset($data['item']) || 
    !isset($data['dayTime']) || !isset($data['price'])) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields (session_id, item, dayTime, price)"
    ]);
    exit;
}

// Extraer y escapar datos 
$session_id = (int)$data['session_id']; 
$item = (int)$data['item']; 
$dayTime = $conn->real_escape_string($data['dayTime']); 
$price = (float)$data['price']; 

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

// Insertar compra en la tabla Purchases (con P mayúscula)
$sql = "INSERT INTO Purchases (session_id, item, dayTime, price) 
        VALUES ($session_id, $item, '$dayTime', $price)"; 

if ($conn->query($sql) === TRUE) {
    echo json_encode([
        "success" => true,
        "message" => "Purchase recorded successfully",
        "purchase_id" => $conn->insert_id
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