<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$index = rawurldecode($_GET["index"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}

$result_value = 0;
$result_type = 0;

$sql_user = "SELECT gifts, gifts_group FROM users WHERE user_id='$user_id' LIMIT 1";

if ($result_user = mysqli_query($conn, $sql_user)) {
	$r_user = mysqli_fetch_array($result_user);
	$group = $r_user['gifts_group'];
	$states = array_map('intval', explode(';', $r_user['gifts']));

	$sql = "SELECT id, type, value FROM gifts WHERE group_id='$group' && id NOT IN ('$states[0]', '$states[1]', '$states[2]', '$states[3]', '$states[4]', '$states[5]', '$states[6]', '$states[7]', '$states[8]') ORDER BY RAND() LIMIT 1";
	$result = mysqli_query($conn, $sql);

	while ($r = mysqli_fetch_array($result)) {
		$gift_id = $r['id'];
		$states[$index] = $gift_id;
		$states_str = implode(";",$states);
		$result_value = $r['value'];
		$result_type = $r['type'];
		$current_date = date("Y-m-d H:i:s");

		mysqli_query($conn, "UPDATE users SET gifts='$states_str', last_gift_timestamp='$current_date', gift_paid='0'  WHERE user_id='$user_id' LIMIT 1");
	}

	mysqli_free_result($result);
	mysqli_free_result($result_user);
}

mysqli_close($conn);

echo $result_type.";".$result_value;
?>