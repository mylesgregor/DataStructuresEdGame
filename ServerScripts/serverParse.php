<?php
$servername = 'localhost';
$username  = 'ace';
$password = '';
$dbname = 'edData';

ini_set('memory_limit', '1024M'); // or you could use 1G
// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// only need playerID, levelFile, worldState

$sqlQuery = "SELECT playerID, levelFile, worldState FROM actiontable";

$result = $conn->query($sqlQuery);



while($row = $result->fetch_assoc())
{


$tblname = $row["playerID"]."_".$row["levelFile"];

$playerID = $row["playerID"];
$levelFile = $row["levelFile"];
$worldState = $row["worldState"];

if($conn->query("DESCRIBE $tblname")) {
  //  echo "it works!";

    $sql = "INSERT INTO $tblname VALUES ('$playerID', '$levelFile', '$worldState')";
    $conn->query($sql);

  


}
else{
  //echo "broke";


  $create = "CREATE TABLE $tblname(playerID int(11) NOT NULL,
   levelFile varchar(100) NOT NULL,
 worldState text NOT NULL)";
 $conn->query($create);


 $sql = "INSERT INTO $tblname VALUES ('$playerID', '$levelFile', '$worldState')";
  $conn->query($sql);


}


}


$conn->close();

echo "SUCCESS";

 ?>
