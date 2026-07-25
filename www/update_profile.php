<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$name = !isset($_GET['name'])? "" : rawurldecode($_GET["name"]);
$surname = !isset($_GET['surname'])? "" : rawurldecode($_GET["surname"]);
$email = !isset($_GET['email'])? "" : rawurldecode($_GET["email"]);
$phone = !isset($_GET['phone'])? 0 : rawurldecode($_GET["phone"]);

if ($device_id != "") {
	$name = ClearUserName($conn, $name);
	$phone = str_replace('+', '', $phone);
	$phone = str_replace(' ', '', $phone);
	$phone = (int)$phone;
	
	mysqli_query($conn, "UPDATE users SET first_name='$name', last_name='$surname', email='$email', phone='$phone' WHERE device_id='$device_id' LIMIT 1");
	mysqli_close($conn);
}
?>