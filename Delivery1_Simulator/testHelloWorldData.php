<?php


$servername = "citmalumnes.upc.es";
$username = "samuelm1";
$password = "SRhcWbnAPYdN";
$dataBase = "samuelm1";

// Create connection
$conn = new mysqli($servername, $username, $password, $dataBase);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
echo "Connected successfully";


// Data base connection done before
$playerDataJSON = $_POST['playerData'];
$playerData = json_decode($playerDataJSON, true);

// Validar que se recibieron los datos correctamente
if (!$playerData) {
    die("Error decoding JSON data.");
}

// Asignar los valores del JSON a variables PHP
$name = $conn->real_escape_string($playerData['name']);
$country = $conn->real_escape_string($playerData['country']);
$age = (int)$playerData['age'];
$gender = (float)$playerData['gender'];
$dayTime = $conn->real_escape_string($playerData['dayTime']);

?>