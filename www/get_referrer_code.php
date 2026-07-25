<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$code = "";

if ($device_id != "") {
	$user_id = GetUserId($conn, $device_id);

	if ($user_id > 0) {
		$code = GetReferrerCode($conn, $user_id);
	}

	mysqli_close($conn);
}

echo $code;
?>