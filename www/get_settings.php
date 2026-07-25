<?php
include('config.inc.php');
include('functions.inc.php');

$app_version = !isset($_GET['app_version'])? "" : rawurldecode($_GET["app_version"]);
$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$ip = GetClientIP();

$auto_payout = false;

if ($app_version != "" && $device_id != "") {
	$rows = [];

	$user_id = 0;
	$country_code = GetCountryCodeByIp($ip);

	$sql_users = "SELECT * FROM users WHERE device_id='$device_id' LIMIT 1";
	
	if ($res_users = mysqli_query($conn, $sql_users)) {
		while ($r_users=mysqli_fetch_array($res_users)) {
			$user_id = $r_users['user_id'];
			$access_restricted = AccessRestricted($conn, $user_id);
			$first_open_date = $r_users['first_open_date'];
			$click_earn_button = $r_users['click_earn_button'] == 1;
			$campaign = $r_users['campaign'];
			$is_organic = $r_users['is_organic'] == 1;

			$rows['Name'] = $r_users['first_name'];
			$rows['Surname'] = $r_users['last_name'];
			$rows['RegistrationDate'] = date('d F Y', strtotime($first_open_date));
			$rows['Email'] = $r_users['email'];
			$rows['PhoneNumber'] = $r_users['phone'];
			$rows['Status'] = GetUserStatus($conn, $user_id, $first_open_date, $campaign, $click_earn_button);
			$rows['IsOrganic'] = $is_organic;
			$rows['CheckUpdate'] = true;
			$rows['PublisherId'] = $r_users['publisher_id'];
			$rows['CampaignId'] = $campaign;
			$rows['NetworkName'] = GetNewtworkName($conn, $r_users['gps_adid']);
			$rows['AdjoeOpened'] = $click_earn_button;
			$rows['HasWelcomeBonus'] = !$is_organic && $r_users['welcome_bonus_received'] == 0 && HasWelcomeBonus($first_open_date);
			$rows['CanShowMissions'] = CanShowMissions($first_open_date, $country_code, $campaign, UserWasRegisteredIP($conn, $device_id, $ip));
			$rows['AdjoeForEarnButton'] = AdjoeForEarnButton($country_code, $first_open_date);
		}
		mysqli_free_result($res_users);
	}

	if ($user_id == 0) {
		return;
	} else {
		$date = $current_date = date("Y-m-d H:i:s");

		mysqli_query($conn, "UPDATE users SET timestamp='$date', country='$country_code', ip='$ip' WHERE user_id='$user_id' LIMIT 1");
	}

	$rows['AccessRestricted'] = $access_restricted || IsIPv6($ip);
	//$rows['PhoneNumber'] = GetPhoneNumber($conn, $user_id);
	$rows['CheckUpdate'] = true;
	$rows['CountryCode'] = $country_code;

	$sql = "SELECT * FROM settings";
	
	if ($result = mysqli_query($conn, $sql)) {
		$app_version_num_max = 0;
		$app_version_max = "";

		while($r = mysqli_fetch_array($result)) {
			if ($app_version == $r["app_version"]) {
				$rows['CheckUpdate'] = $r["check_update"] == 1;
				$auto_payout = $r["auto_payout"] == 1;
				$rows['AdjoeEnabled'] = $r['adjoe_enabled'] == 1;// && CanShowAdjoe($is_organic, $country_code, $age, $is_male, $campaign, $first_open_date);
				$rows['MyChipsEnabled'] = $r['mychips_enabled'] == 1;
				$rows['SpecialOfferCoins'] = $r['special_offer_coins'];
				$rows['MychipsPromo'] = $r['mychips_promo'] == 1;
				$rows['PrimeEnabled'] = $r['prime_enabled'] == 1;
			}

			$app_version_num = (int) str_replace('.', '', $r["app_version"]);

			if ($app_version_num > $app_version_num_max) {
				$app_version_num_max = $app_version_num;
				$app_version_max = $r["app_version"];
			}
		}
		
		$check_update = $rows['CheckUpdate'] && $app_version_max != $app_version;
		$rows['CheckUpdate'] = $check_update;

		if (!$check_update && !$access_restricted) {
			//$rows['PayoutNotify'] = NeedPayoutNotify($conn, $user_id);
		}

		mysqli_free_result($result);
	}
	
	echo json_encode($rows, JSON_PRETTY_PRINT);

	//include('clear_db.inc.php');

	if (CanShowMissions($first_open_date, $country_code, $campaign, UserWasRegisteredIP($conn, $device_id, $ip))) {
		CheckMissions($conn, $user_id, AdjoeForEarnButton($country_code, $first_open_date));
	}

	include('singular_report.inc.php');

	mysqli_close($conn);
}
?>