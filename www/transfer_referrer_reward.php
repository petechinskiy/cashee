<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$success = 0;
$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return $success;
}

$sql = "SELECT SUM(coins) FROM referrer_callbacks WHERE user_id='$user_id'";

if ($result = mysqli_query($conn, $sql)) {
	$r = mysqli_fetch_array($result);
	$coins = $r[0];
	mysqli_free_result($result);

	if ($coins > 0) {
		$success = 1;

		mysqli_query($conn, "INSERT INTO income_referrer (user_id, coins) VALUES ('$user_id', '$coins')");
		mysqli_query($conn, "DELETE FROM referrer_callbacks WHERE user_id='$user_id'");
	}
}

mysqli_close($conn);

echo $success;
?>