<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$user_id = 0;

$sql_user = "SELECT user_id, country, first_open_date, campaign, ip FROM users WHERE device_id='$device_id' LIMIT 1";

if ($result_user = mysqli_query($conn, $sql_user)) {
	while ($r_user = mysqli_fetch_array($result_user)) {
		$user_id = $r_user['user_id'];
		$country = $r_user['country'];
		$registration_date = $r_user['first_open_date'];
		$campaign = $r_user['campaign'];
		$ip = $r_user['ip'];
	}
	mysqli_free_result($result_user);
}

if ($user_id == 0 || !CanShowMissions($registration_date, $country, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
	mysqli_close($conn);
	return;
}

$rows['Missions'] = GetMissionStates($conn, $user_id, AdjoeForEarnButton($country, $registration_date));

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>