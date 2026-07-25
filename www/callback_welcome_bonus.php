<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$user_id = 0;
$welcome_bonus_received = true;
$bonus = 250;

$sql_users = "SELECT user_id, welcome_bonus_received FROM users WHERE device_id='$device_id' LIMIT 1";
	
if ($res_users = mysqli_query($conn, $sql_users)) {
	while ($r_users=mysqli_fetch_array($res_users)) {
		$user_id = $r_users['user_id'];
		$welcome_bonus_received = $r_users['welcome_bonus_received'] == 1;
	}
	mysqli_free_result($res_users);
}

if ($user_id == 0 || $welcome_bonus_received) {
	mysqli_close($conn);
	return;
}

$current_date = new DateTime();
$current_date_str = $current_date->format('Y-m-d');

mysqli_query($conn, "UPDATE users SET welcome_bonus_received='1' WHERE user_id='$user_id' LIMIT 1");
mysqli_query($conn, "INSERT INTO income_adjoe (user_id, timestamp, coin_amount, device_id, trans_uuid, is_bonus_purchase) VALUES ('$user_id', '$current_date_str', '$bonus', '$device_id', 'welcome_bonus', '1')");

mysqli_close($conn);
?>