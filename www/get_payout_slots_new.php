<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$gift_percent_index = !isset($_GET['gift_percent_index'])? 0 : rawurldecode($_GET["gift_percent_index"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}
	
$gift_percent_id = 0;
$rows = [];
$currency_multiplicator = 1.0;
$country_code = "";
$is_third_country = false;
$is_organic = true;

$sql_users = "SELECT country, gifts, is_organic, campaign, publisher_id, creative FROM users WHERE user_id='$user_id' LIMIT 1";

if ($res_users = mysqli_query($conn, $sql_users)) {
	while($r_users = mysqli_fetch_array($res_users)) {
		$gift_states = array_map('intval', explode(';', $r_users['gifts']));
		$gift_percent_id = $gift_percent_index >= 0 ? $gift_states[$gift_percent_index] : 0;
		$country_code = $r_users['country'];
		$is_third_country = IsThirdCountry($country_code);
		$is_organic = $r_users['is_organic'] == 1;
		$campaign = $r_users['campaign'];

		switch($country_code) {
			case "DE":
			case "FR":
			case "IE": //         
			case "IT":
			case "ES":
			case "SE": //       
			case "DK": //      
			case "CZ": //      
			case "BE": //        
			case "AT": //        
			case "NL": //
			case "BG": // Болгария
			case "AD": // Андорра
			case "PT": // Португалия
			case "PL": // Польша
			case "RO": // Румыния
			case "RS": // Сербия
			case "UA": // Украина
			case "EE": // Эстония
			case "NO": // Норвегия
			case "LV": // Латвия
			case "LT": // Литва
			case "GE": // Грузия
			case "MD": // Молдова
			case "HU": // Венгрия
			case "GR": // Греция
			case "SK": // Словакия
			case "HR": // Хорватия
				$currency_multiplicator = 0.9;
				break;
			case "GB": // великобритания
				$currency_multiplicator = 0.75;
				break;
			case "CA": // канада
				$currency_multiplicator = 1.428571428571429;
				break;
			case "AU": // австралия
				$currency_multiplicator = 1.666666666666667;
				break;
			case "CH": // швейцария
				$currency_multiplicator = 0.88;
				break;
			case "KR": // южная корея
				$currency_multiplicator = 1300;
				break;
			case "JP": // япония
				$currency_multiplicator = 150;
				break;
		}
	}
}

$sql = "SELECT * FROM payout_slots WHERE enabled='1'";

if ($result = mysqli_query($conn, $sql)) {
	$referrer_discount = AnyReferrers($conn, $user_id) && IsFirstPayout($conn, $user_id) ? 0.2 : 0.0;

	while($r = mysqli_fetch_array($result)) {
		$id = $r['id'];
		$country_filter = $r['country_filter'];
		$for_not_organic = $r['for_not_organic'] == 1;

		if (($country_filter == "all" || ($country_filter == "poor" && $is_third_country) || ($country_filter == "rich" && !$is_third_country) || str_contains($country_filter, $country_code)) && (!$is_organic || !$for_not_organic))
		{
			$usage_limited = $r['usage_limit'] <= PayoutsCount($conn, $user_id, (int)$r["id"]);
			$coins_orig = IsUnityCampaign($campaign) ? $r['coins_amount_unity'] : $r['coins_amount'];
			$currency_amount = (float)$r['currency_amount'] * $currency_multiplicator;
			$currency_amount =round($currency_amount, 2, PHP_ROUND_HALF_UP);
			$discount_coins = GetGiftDiscount($conn, $coins_orig, $gift_percent_id);
			$referrer_discount_coins = $coins_orig * $referrer_discount;
			$coins = $coins_orig - $discount_coins - $referrer_discount_coins;

			$jsonArrayObject = array('coins_amount' => (int)$coins, 'currency_amount' => (float)$currency_amount, 'payout_type' => (int)$r["payout_type"], 'is_active' => true, 'id' => (int)$id, 'direct_paypal' => (bool)$r['direct_paypal'], 'usage_limited' => $usage_limited);
			$rows['SlotsData'][] = $jsonArrayObject;
		}
	}

	mysqli_free_result($result);
}

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>