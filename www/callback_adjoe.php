<?php
include('config.inc.php');
include('functions.inc.php');

$tbl = "income_adjoe";
$s2s_token = "6rOaw3pl750QD9nZXKTq2vShj8df14xu";

$user_uuid = !isset($_GET['user_uuid'])? "" : rawurldecode($_GET["user_uuid"]);
$device_id = !isset($_GET['device_id'])? "-1" : rawurldecode($_GET["device_id"]);
$sid = !isset($_GET['sid'])? "-1" : rawurldecode($_GET["sid"]);
$coin_amount = !isset($_GET['coin_amount'])? 0 : rawurldecode($_GET["coin_amount"]);
$trans_uuid = !isset($_GET['trans_uuid'])? "" : rawurldecode($_GET["trans_uuid"]);
$sdk_app_id = !isset($_GET['sdk_app_id'])? "" : $_GET["sdk_app_id"];
$currency = !isset($_GET['currency'])? "" : rawurldecode($_GET["currency"]);
$event_type = !isset($_GET['event_type'])? "" : rawurldecode($_GET["event_type"]);

$data = $trans_uuid . $user_uuid . $currency . $coin_amount . $device_id . $sdk_app_id . $s2s_token;

$computed_sid = sha1($data);
$is_valid = $sid == $computed_sid;
$is_bonus_event = $event_type == "bonus" ? 1 : 0;

if (!$is_valid)
{
	echo "Not valid signature";
	//$user_id = GetUserId($conn, $device_id);
	//mysqli_query($conn, "UPDATE users SET access_restricted='3' WHERE user_id='$user_id' LIMIT 1");
	echo "User is blocked!";
}
else
{
	$user_id = 0;
	$referrer_lvl_1 = 0;
	$first_adjoe_reward_received = false;
	$is_organic = false;
	$access_restricted = false;
	$campaign = "";
	$country_code = "";
	$is_old_user = false;
	$gps_adid = "";
	$ip = "";
	$first_open_date = "";

	$sql = "SELECT user_id, first_adjoe_reward_received, is_organic, access_restricted, campaign, country, first_open_date, gps_adid, ip, referrer_user_id FROM users WHERE device_id='$user_uuid' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
			$referrer_lvl_1 = $r['referrer_user_id'];
			$first_adjoe_reward_received = $r['first_adjoe_reward_received'] == 1;
			$is_organic = $r['is_organic'] == 1;
			$access_restricted = $r['access_restricted'] == 1;
			$campaign = $r['campaign'];
			$country_code = $r['country'];
			$gps_adid = $r['gps_adid'];
			$ip = $r['ip'];
			$first_open_date = $r['first_open_date'];
		}

		mysqli_free_result($result);
	}

	if ($user_id == 0) {
		mysqli_close($conn);
		return;
	}

	if (!$access_restricted) {
		if (AdjoeCoinsPerTime($conn, $user_id, 1) > 75000) {
			mysqli_query($conn, "UPDATE users SET access_restricted='2' WHERE user_id='$user_id' LIMIT 1");
			BlockUserLeaderboard($conn, $user_id);
			echo "User is blocked!";
		} else if (CanShowAdjoe($is_organic, $country_code, $campaign, $first_open_date)) {
			$current_date = new DateTime();
			$current_date_str = $current_date->format('Y-m-d');

			if (mysqli_query($conn, "INSERT INTO $tbl (user_id, coin_amount, trans_uuid, device_id, timestamp, is_bonus_event) VALUES ('$user_id', '$coin_amount', '$trans_uuid', '$device_id', '$current_date_str', '$is_bonus_event')")) {
 				echo "Record added succesfully";

				if (!$first_adjoe_reward_received) {
					mysqli_query($conn, "UPDATE users SET first_adjoe_reward_received='1' WHERE user_id='$user_id' LIMIT 1");
				}

				if ($referrer_lvl_1 != 0) {
					$type = 1;
					$callback_id = mysqli_insert_id($conn);

					AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_1, $user_id, $coin_amount, 1);

					$referrer_lvl_2 = GetUserReferrer($conn, $referrer_lvl_1);
					if ($referrer_lvl_2 != 0) {
						AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_2, $user_id, $coin_amount, 2);
					}
				}

				if ($is_bonus_event) {
					SendSingularEvent('bonus', $gps_adid, $ip);
				}

				UpdateLeaderboard($conn, $user_id, $coin_amount, $country_code);

				if (CanShowMissions($first_open_date, $country_code, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
					CheckMissions($conn, $user_id, AdjoeForEarnButton($country_code, $first_open_date));
				}
			}
		} else {
			//mysqli_query($conn, "UPDATE users SET access_restricted='2' WHERE user_id='$user_id' LIMIT 1");
			//echo "User is blocked!";
		} 
	}
}

mysqli_close($conn);
?>