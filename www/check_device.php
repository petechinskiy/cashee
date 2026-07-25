<?php
include('config.inc.php');
include('functions.inc.php');

$user_id = !isset($_GET['user_id'])? "" : rawurldecode($_GET["user_id"]);
$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$not_valid = $user_id == "" || $device_id == "" || DeviceIsDuplicate($conn, $user_id, $device_id);

mysqli_close($conn);

echo $not_valid;
?>