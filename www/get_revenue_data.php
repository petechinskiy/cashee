<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}


$revenue = 0.0;
	
$sql = "SELECT
    (SELECT COALESCE(SUM(cpi), 0)
     FROM adjoe_installs
     WHERE user_id = '$user_id' && notified=0) +
    (SELECT COALESCE(SUM(revenue), 0)
     FROM income_mychips
     WHERE user_id = '$user_id' && notified=0) +
	(SELECT COALESCE(SUM(CASE WHEN type=9 THEN -revenue ELSE revenue END), 0)
     FROM income_prime
     WHERE user_id = '$user_id' && notified=0) AS total_sum";

	if ($res = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($res)) {
			$revenue += $r[0];
		}

		mysqli_free_result($res);

		if ($revenue > 0) {
			mysqli_query($conn, "UPDATE adjoe_installs SET notified='1' WHERE user_id='$user_id' && notified=0");
			mysqli_query($conn, "UPDATE income_mychips SET notified='1' WHERE user_id='$user_id' && notified=0");
			mysqli_query($conn, "UPDATE income_prime SET notified='1' WHERE user_id='$user_id' && notified=0");
		}
	}
	
echo $revenue;

mysqli_close($conn);
?>