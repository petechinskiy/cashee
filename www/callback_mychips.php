<?php
include('config.inc.php');
include('functions.inc.php');

$tbl = "income_mychips";
$pubic_api_key = "18d958abb9766d3d3a4a3178f51d3155";
$ip_whitelist = ['168.63.37.145','20.54.96.37','13.70.194.104','34.146.139.91','34.54.234.115','34.54.248.253','34.64.93.62','34.47.93.43','34.84.180.208','48.209.163.104','4.207.193.125','48.209.162.122'];

$postback_id = !isset($_GET['postback_id'])? "" : rawurldecode($_GET["postback_id"]);
$user_payout = !isset($_GET['user_payout'])? 0.0 : rawurldecode($_GET["user_payout"]);
$device_id = !isset($_GET['user_id'])? "" : rawurldecode($_GET["user_id"]);
$adunit_id = !isset($_GET['adunit_id'])? "" : rawurldecode($_GET["adunit_id"]);
$revenue = !isset($_GET['payout'])? 0.0 : rawurldecode($_GET["payout"]);

$ip = GetClientIP();

$is_valid = in_array($ip, $ip_whitelist);

if (!$is_valid)
{
	$file = 'mychips_log.txt';
	$current = file_get_contents($file);
	$content = "ip:$ip; postback_id:$postback_id; user_payout:$user_payout; device_id:$device_id; adunit_id:$adunit_id";
	$current .= $content . "\n";
	file_put_contents($file, $current);

	echo "Not valid!";
}
else 
{
	$user_id = 0;
	$referrer_lvl_1 = 0;
	$reward = (int)($user_payout * 3000);

	$sql = "SELECT user_id, referrer_user_id, country, first_open_date, campaign FROM users WHERE device_id='$device_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
			$referrer_lvl_1 = $r['referrer_user_id'];
			$country = $r['country'];
			$registration_date = $r['first_open_date'];
			$campaign = $r['campaign'];
		}

		mysqli_free_result($result);
	}

	if ($user_id == 0) {
		echo "User is not found";
	} else if (mysqli_query($conn, "INSERT INTO $tbl (`postback_id`, `reward`, `user_id`, `adid`, `revenue`)
									VALUES ('$postback_id', '$reward', '$user_id', '$adunit_id', '$revenue')")) {
		echo "Record added succesfully";

		if ($referrer_lvl_1 != 0) {
			$type = 1;
			$callback_id = mysqli_insert_id($conn);

			AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_1, $user_id, $reward, 1);

			$referrer_lvl_2 = GetUserReferrer($conn, $referrer_lvl_1);
			if ($referrer_lvl_2 != 0) {
				AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_2, $user_id, $reward, 2);
			}
		}

		if (CanShowMissions($registration_date, $country, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
			CheckMissions($conn, $user_id, AdjoeForEarnButton($country, $registration_date));
		}

		UpdateLeaderboard($conn, $user_id, $reward, $country);
	}
}

mysqli_close($conn);
?>