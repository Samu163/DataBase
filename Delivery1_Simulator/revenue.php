<?php
header('Content-Type: application/json; charset=UTF-8');

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
$data = json_decode($json, true); 

if (!$data || !isset($data['Item']) || !isset($data['dayTime']) || 
    !isset($data['session_id']) || !isset($data['price'])) { 
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields",
        "received" => $data
    ]);
    exit;
}

$item = (int)$data['Item'];
$dayTime = $conn->real_escape_string($data['dayTime']);
$session_id = (int)$data['session_id'];
$price = (float)$data['price'];

$sql = "INSERT INTO Revenue (Item, dayTime, session_id, price) 
        VALUES ($item, '$dayTime', $session_id, $price)"; 

if ($conn->query($sql) === TRUE) {
    echo json_encode([
        "success" => true,
        "message" => "Purchase recorded successfully",
        "purchase_id" => $conn->insert_id,
        "data" => [
            "Item" => $item,
            "dayTime" => $dayTime,
            "session_id" => $session_id,
            "price" => $price,
            "purchase_id" => $conn->insert_id
        ]
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