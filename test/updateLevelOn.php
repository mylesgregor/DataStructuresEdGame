<?php
# return either success or fail based on if they succeeded in logging in or not.
###

$host = 'localhost';
$user = 'root';
$pass = '';
$db = 'EdGameDB';
$con = mysqli_connect($host, $user, $pass, $db);
if(mysqli_connect_errno()){
	echo 'Failed to connect to MySQL: ' .mysqli_connect_error();
	exit(1);
}

// from: https://docs.unity3d.com/Manual/webgl-networking.html
header("Access-Control-Allow-Credentials: true");
header("Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS");
header("Access-Control-Allow-Origin: *");


$playerId = $_POST['playerID'];
$levelOn = $_POST['levelOn'];
$qry = "UPDATE users SET levelOn=" . htmlspecialchars($levelOn) . " WHERE playerId = " . htmlspecialchars($playerId);

$res = mysqli_query($con, $qry);
if(!$res){
	echo 'database error';
	mysqli_close($con);
	exit(1);
}

echo 'success';

mysqli_close($con);

?>