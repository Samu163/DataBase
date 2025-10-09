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
$sql = "INSERT INTO UsersData (name, country, age, gender, DayTime) 
        VALUES ('John', 'Angola', 15, 1, '2025-03-01 13:44:00')";

if ($conn->query($sql) === TRUE) {
    echo "New record created successfully";
} else {
    echo "Error: " . $sql . "<br>" . $conn->error;
}

$sql2 = "INSERT INTO Sesions (DateTime, playerId) 
        VALUES ('2025-03-01 13:44:00', 1)";

if ($conn->query($sql2) === TRUE) {
    echo "New record created successfully";
} else {
    echo "Error: " . $sql2 . "<br>" . $conn->error;
}


$sql3 = "INSERT INTO Revenue (Item,DateTime,sesionId) 
        VALUES (1,'2025-03-01 13:44:00', 1)";

if ($conn->query($sql3) === TRUE) {
    echo "New record created successfully";
} else {
    echo "Error: " . $sql3 . "<br>" . $conn->error;
}

?>