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

// Validar que existan los campos necesarios
if (!isset($data['name']) || !isset($data['country']) || !isset($data['age']) || 
    !isset($data['gender']) || !isset($data['dayTime'])) {
    http_response_code(400);
    echo json_encode([
        "success" => false,
        "error" => "Missing required fields"
    ]);
    exit;
}

// Extraer y escapar datos 
$name = $conn->real_escape_string($data['name']); 
$country = $conn->real_escape_string($data['country']); 
$age = (int)$data['age']; 
$gender = (float)$data['gender']; 
$dayTime = $conn->real_escape_string($data['dayTime']); 

// Insertar datos en la tabla 
$sql = "INSERT INTO UsersData (name, country, age, gender, dayTime) 
        VALUES ('$name', '$country', $age, $gender, '$dayTime')"; 

if ($conn->query($sql) === TRUE) {
    // IMPORTANTE: Aquí está el objeto 'data' que falta
    echo json_encode([
        "success" => true,
        "message" => "Record created successfully",
        "id" => $conn->insert_id,
        "data" => [
            "name" => $name,
            "country" => $country,
            "age" => $age,
            "gender" => $gender,
            "dayTime" => $dayTime
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