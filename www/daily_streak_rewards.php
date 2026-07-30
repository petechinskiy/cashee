<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$app_version = !isset($_GET['app_version'])? "" : rawurldecode($_GET["app_version"]);

$user_id = 0;

$sql_user = "SELECT user_id, first_open_date, country, campaign, ip, daily_streak FROM users WHERE device_id='$device_id' LIMIT 1";

if ($result_user = mysqli_query($conn, $sql_user)) {
	while ($r_user = mysqli_fetch_array($result_user)) {
		$user_id = $r_user['user_id'];
		$registration_date = $r_user['first_open_date'];
		$country = $r_user['country'];
		$campaign = $r_user['campaign'];
		$ip = $r_user['ip'];
		$states = array_map('intval', explode(';', $r_user['daily_streak']));
	}

	mysqli_free_result($result_user);
}

if ($user_id == 0 || !CanShowMissions($registration_date, $country, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
	mysqli_close($conn);
	return;
}

$sql = "SELECT daily_reward_coins FROM settings WHERE app_version='$app_version' LIMIT 1";
$result = mysqli_query($conn, $sql);

while ($r = mysqli_fetch_array($result)) {
	$daily_rewards = array_map('intval', explode(';', $r['daily_reward_coins']));
	$data = DailyStreakRewards($conn, $user_id, $states, $daily_rewards, true);

	echo $data['states'];
}
mysqli_free_result($result);

mysqli_close($conn);
?>