<?php
$servername = 'localhost';
$username  = 'ace';
$password = '';
$dbname = 'edData';

// Create connection
$conn = mysqli_connect($servername, $username, $password, $dbname);
// Check connection
if(mysqli_connect_errno()){
	echo 'Failed to connect to MySQL: ' .mysqli_connect_error();
	exit(1);
}


// from: https://docs.unity3d.com/Manual/webgl-networking.html
header("Access-Control-Allow-Credentials: true");
header("Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Origin: *");

// only need playerID, levelFile, worldState

//$currentTable = mysqli_real_escape_string($_POST['currentTable']);
//$currentLine = mysqli_real_escape_string($_POST['currentLine']);
$currentTable = filter_input(INPUT_POST, 'currentTable');
$currentLine = filter_input(INPUT_POST, 'currentLine');

//$currentTable = "13_tutorial2_lvl1";
//$currentLine = 1;



$sqlQuery = "SELECT worldState FROM $currentTable LIMIT $currentLine,1";

$result = $conn->query($sqlQuery);


while($row = $result->fetch_assoc()){
      echo $row['worldState'];
}



$conn->close();


 ?>
