<?php
// ayet callback

include('config.inc.php');
include('functions.inc.php');

$tbl = "income_ayet";
$pubic_api_key = "18d958abb9766d3d3a4a3178f51d3155";

$transaction_id = !isset($_GET['transaction_id'])? "" : rawurldecode($_GET["transaction_id"]);
$payout_usd = !isset($_GET['payout_usd'])? "0" : rawurldecode($_GET["payout_usd"]);
$currency_amount = !isset($_GET['currency_amount'])? "0" : rawurldecode($_GET["currency_amount"]);
$external_identifier = !isset($_GET['external_identifier'])? "" : rawurldecode($_GET["external_identifier"]);
$is_chargeback = !isset($_GET['is_chargeback'])? "0" : rawurldecode($_GET["is_chargeback"]);

ksort($_REQUEST, SORT_STRING);
$sortedQueryString = http_build_query($_REQUEST, '', '&');
$securityHash = hash_hmac('sha256', $sortedQueryString, $pubic_api_key);
$is_valid = $_SERVER['HTTP_X_AYETSTUDIOS_SECURITY_HASH'] === $securityHash;

if (!$is_valid)
{
	echo "Not valid signature";
}
else 
{
	$user_id = 0;
	$referrer_lvl_1 = 0;
	$currency_amount = min(50000, $currency_amount);

	$sql = "SELECT user_id, referrer_user_id, ip, kochava_device_id, gps_adid FROM users WHERE device_id='$external_identifier' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
			$referrer_lvl_1 = $r['referrer_user_id'];
			$ip = $r['ip'];
			$kochava_device_id = $r['kochava_device_id'];
			$gps_adid = $r['gps_adid'];
		}

		mysqli_free_result($result);
	}

	if ($user_id == 0) {
		echo "User is not found";
	} else if (mysqli_query($conn, "INSERT INTO $tbl (`transaction_id`, `payout_usd`, `currency_amount`, `user_id`, `ip`, `is_chargeback`)
									VALUES ('$transaction_id', '$payout_usd', '$currency_amount', '$user_id', '$ip', '$is_chargeback')")) {
		echo "Record added succesfully";

		if ($referrer_lvl_1 != 0) {
			$type = 1;
			$callback_id = mysqli_insert_id($conn);

			AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_1, $user_id, $currency_amount, 1);

			$referrer_lvl_2 = GetUserReferrer($conn, $referrer_lvl_1);
			if ($referrer_lvl_2 != 0) {
				AddReferrerReward($conn, $callback_id, $type, $referrer_lvl_2, $user_id, $currency_amount, 2);
			}
		}

		/*
		$reward_usd = $coin_amount / 3000;
		$date = new DateTime();
		$date->setTimezone(new DateTimeZone('GMT'));
		$date_str_full = $date->format('Y-m-d H:i:s');
		$unixtime = strtotime($date_str_full);

		SendKochavaEvent("Purchase", $reward_usd, $gps_adid, $kochava_device_id, $ip, $unixtime, "1.0.0");
		*/
	}
}

mysqli_close($conn);
?>