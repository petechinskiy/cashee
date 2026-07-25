<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$app_version = !isset($_GET['app_version'])? "" : rawurldecode($_GET["app_version"]);

$sql = "SELECT user_id, country FROM users WHERE device_id='$device_id' LIMIT 1";
	
if ($result = mysqli_query($conn, $sql)) {
	while($r=mysqli_fetch_array($result)) {
		$user_id = $r['user_id'];
		$country_code = $r['country'];
	}

	mysqli_free_result($result);
}

if ($user_id == 0) {
	mysqli_close($conn);
	return;
}

// считаем время до следующего сброса таблицы лидербордов (каждый понедельник)
$last_reset_date = LeaderboardResetDate($conn, $app_version);
$last_time       = strtotime($last_reset_date);
$current_time    = time();

// сколько прошло дней с последнего сброса
$left_time = $current_time - $last_time;
$days_passed = $left_time / 86400;
$today_is_monday = (date('N') == 1); // 1 = понедельник
$next_reset = strtotime('+7 days', $last_time);
$time_until_reset = $next_reset - time();

if ($today_is_monday && $days_passed > 6) {
    $current_date = date("Y-m-d");

    mysqli_query($conn, "UPDATE settings SET leaderboard_reset_date='$current_date'");
    mysqli_query($conn, "TRUNCATE TABLE leaderboard");
}

$min_balance = 1000;
$tier = GetTier($country_code);
	
if ($tier == 2) {
	$min_balance = 750;
} else if ($tier == 3) {
	$min_balance = 500;
}

$rows['LeftSecondsToUpdate'] = $time_until_reset;
$rows['MinBalance'] = $min_balance;
$rows['Ranks'] = GetLeaderboardData($conn, $user_id, $tier, $min_balance);

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>