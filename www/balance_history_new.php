<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$gift_index = !isset($_GET['gift_index'])? 0 : rawurldecode($_GET["gift_index"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}

$gift_id = 0;

$rows = [];
$days_amount = 7;
	
$sql_user = "SELECT gifts, gps_adid, first_open_date, campaign FROM users WHERE user_id='$user_id' LIMIT 1";

if ($res_user = mysqli_query($conn, $sql_user)) {
	while($r_user=mysqli_fetch_array($res_user)) {
		$gift_states = array_map('intval', explode(';', $r_user['gifts']));
		$gift_id = $gift_index >= 0 ? $gift_states[$gift_index] : 0;
		$gps_adid = $r_user['gps_adid'];
		$registration_date = $r_user['first_open_date'];
		$campaign = $r_user['campaign'];
	}

	mysqli_free_result($res_user);
}

$rows['CurrentBalance'] = GetBalanceWithGift($conn, $user_id, $gift_id, $registration_date, $campaign);
$rows['LastEarnedCoins'] = LastEarnCoins($conn, $user_id, $days_amount);
$rows['TodayEarnedCoins'] = LastEarnCoins($conn, $user_id, 1);
$rows['LastWeekEarnedCoins'] = LastEarnCoins($conn, $user_id, 7);
$rows['LastMonthEarnedCoins'] = LastEarnCoins($conn, $user_id, 30);
$rows['OfferwallEarnedCoins'] = OfferwallEarnedCoins($conn, $user_id);

$sql = "SELECT payout_method, wallet, timestamp, payout_usd, status, currency_symbol, direct_paypal
	FROM withdraws
	WHERE user_id='$user_id'
	ORDER BY timestamp DESC
	LIMIT 8";
	
if ($result = mysqli_query($conn, $sql)) {
	while($r = mysqli_fetch_array($result)) {
		$payout_type = 0;
		$payout_type_str = strtolower($r["payout_method"]);
		$payout_usd = round((float)$r["payout_usd"], 2, PHP_ROUND_HALF_UP);

		switch($payout_type_str) {
			case "amazon": $payout_type = 1; break;
			case "adidas": $payout_type = 2; break;
			case "airbnb": $payout_type = 3; break;
			case "apple": $payout_type = 4; break;
			case "burgerking": $payout_type = 5; break;
			case "dominos": $payout_type = 6; break;
			case "gap": $payout_type = 7; break;
			case "googleplay": $payout_type = 8; break;
			case "netflix": $payout_type = 9; break;
			case "nike": $payout_type = 10; break;
			case "spotify": $payout_type = 11; break;
			case "uber": $payout_type = 12; break;
			case "ubereats": $payout_type = 13; break;
		}

		if (version_compare(phpversion(), '7.1', '>=')) {
			ini_set( 'precision', 17 );
			ini_set( 'serialize_precision', -1 );
		}

		$jsonArrayObject = array('Date' => $r["timestamp"], 'PayoutUsd' => $payout_usd, 'PayoutType' => $payout_type, 'Wallet' => $r["wallet"], 'Status' => (int)$r["status"], 'CurrencySymbol' => $r["currency_symbol"], 'DirectPaypal' => $r["direct_paypal"]);
		$rows['PayoutHistoryData'][] = $jsonArrayObject;
	}
	
	mysqli_free_result($result);
}
	
echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>