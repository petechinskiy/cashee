<?php
include('config.inc.php');
include('functions.inc.php');

$user_id = rawurldecode($_GET["device_id"]);
$phone = !isset($_GET['phone'])? "" : rawurldecode($_GET["phone"]);

$phone_verified = CheckPhoneDuplicate($conn, $user_id, $phone);

mysqli_close($conn);

echo $phone_verified;
?>