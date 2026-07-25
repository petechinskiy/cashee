<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$gps_adid = !isset($_GET['gps_adid'])? "" : rawurldecode($_GET["gps_adid"]);

$status = 0;

if ($device_id != "") {
	$user_id = GetUserId($conn, $device_id);

	if ($user_id > 0) {
		$status = 1;

		if ($gps_adid != "") {
			mysqli_query($conn, "UPDATE users SET gps_adid='$gps_adid' WHERE user_id='$user_id' LIMIT 1");
		}
	}

	mysqli_close($conn);
}

echo $status;
?>