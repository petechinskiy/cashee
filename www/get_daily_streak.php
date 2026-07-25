<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$app_version = !isset($_GET['app_version'])? "" : rawurldecode($_GET["app_version"]);

$user_id = 0;
$rows = [];

$sql_user = "SELECT user_id, first_open_date, timestamp, country, campaign, ip, daily_streak FROM users WHERE device_id='$device_id' LIMIT 1";

if ($result_user = mysqli_query($conn, $sql_user)) {
	while ($r_user = mysqli_fetch_array($result_user)) {
		$user_id = $r_user['user_id'];
		$registration_date = $r_user['first_open_date'];
		$last_opened_date = $r_user['timestamp'];
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

$sql = "SELECT daily_reward_coins, daily_reward_revenue, daily_streak_revenue FROM settings WHERE app_version='$app_version' LIMIT 1";
$result = mysqli_query($conn, $sql);

while ($r = mysqli_fetch_array($result)) {
	$daily_coins = array_map('intval', explode(';', $r['daily_reward_coins']));
	$daily_revenue = array_map('floatval', explode(';', $r['daily_reward_revenue']));

	$reward_data = DailyStreakRewards($conn, $user_id, $states, $daily_coins, false);

	$current_time = time();
	$target_diff_time = 7 * 24 * 60 * 60;
	$start_time = $reward_data['reset_timer'] ? $current_time : strtotime($last_opened_date);
	$target_time = $start_time + $target_diff_time;
	$left_time = $target_time - $current_time;

	$states_data = DailyStreakStates($conn, $user_id, $start_time, $states);

	$daily_reward_index = $reward_data['daily_reward_index'];

	$rows['DailyRevenue'] = $daily_revenue[$daily_reward_index];
	$rows['StreakRevenue'] = (float)$r['daily_streak_revenue'];
	$rows['States'] = $states_data;
	$rows['LeftSecondsToEnd'] = max(0, $left_time);
}

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>