<?php
include('config.inc.php');
include('functions.inc.php');

$days = !isset($_GET['days'])? 1 : rawurldecode($_GET["days"]);
$pub_limit = !isset($_GET['pub_limit'])? 1000 : rawurldecode($_GET["pub_limit"]);
$adjoe_rewards = [];
$button_clicks = [];
$all_clicks = [];
$all_installs = [];
$adjoe_coins = [];
$current_date = date("Y-m-d H:i:s");

$sql_test = "SELECT publisher_id, gps_adid FROM ironsource_installs WHERE timestamp > DATE_SUB('$current_date', INTERVAL '$days' DAY) && gps_adid != '' && gps_adid != '{android_id}'";
	
if ($result = mysqli_query($conn, $sql_test)) {
	$gpsAdids = array();
	while($r_test = mysqli_fetch_array($result)) {
		$gps_adid = $r_test['gps_adid'];
		$publisher_id = $r_test['publisher_id'];

		if (!array_key_exists($publisher_id, $all_clicks)) {
			$adjoe_rewards[$publisher_id] = 0;
			$button_clicks[$publisher_id] = 0;
			$all_installs[$publisher_id] = 0;
			$all_clicks[$publisher_id] = 0;
			$adjoe_coins[$publisher_id] = 0;

			/*
			if (count($all_clicks) >= $pub_limit) {
				$gpsAdids[] = $gps_adid;
				$all_clicks[$publisher_id]++;
				break;
			}
			*/
		}

		$gpsAdids[] = $gps_adid;
		$all_clicks[$publisher_id]++;
	}

	//$gpsAdids = array_column($r_test, 'gps_adid');
	$gpsAdidsString = "'" . implode("','", $gpsAdids) . "'";
	
	$sql = "SELECT u.user_id, u.first_adjoe_reward_received, u.click_earn_button, t.publisher_id FROM users u LEFT JOIN ironsource_installs t ON u.gps_adid=t.gps_adid WHERE u.gps_adid IN ($gpsAdidsString) && t.timestamp > DATE_SUB('$current_date', INTERVAL '$days' DAY)";
	if ($res = mysqli_query($conn, $sql)) {
		while($r = mysqli_fetch_array($res)) {
			$publisher_id = $r['publisher_id'];
			$user_id = $r['user_id'];
			$coins = 0;

			if ($r['first_adjoe_reward_received'] == 1) {
				$adjoe_rewards[$publisher_id]++;
				$button_clicks[$publisher_id]++;
				$coins = LastAdjoeCoins($conn, $user_id, $days);
			} else if ($r['click_earn_button'] == 1) {
				$button_clicks[$publisher_id]++;
			}

			$all_installs[$publisher_id]++;
			$adjoe_coins[$publisher_id] += $coins; 
		}

		mysqli_free_result($res);
	}

	mysqli_free_result($result);
}
	
arsort($all_clicks);

echo "Statistic for the last $days days:<br>";

foreach($all_clicks as $key => $val) {
	$avg_coins =  array_key_exists($key, $all_installs) && $adjoe_coins[$key] > 0 && $all_installs[$key] > 0 ? $adjoe_coins[$key] / $all_installs[$key] : 0;
	$avg_coins = (int)$avg_coins;

	echo $key.":".$val.":".$all_installs[$key].":".$button_clicks[$key].":".$adjoe_rewards[$key].":".$avg_coins."<br>";
}
?>