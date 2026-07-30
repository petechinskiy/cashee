<?php
include('config.inc.php');
include('functions.inc.php');

$tbl = "withdraws";

$payout_method = !isset($_GET['payout_method']) ? "PayPal" : rawurldecode($_GET["payout_method"]);
$currency_amount = !isset($_GET['currency_amount']) ? 0 : rawurldecode($_GET["currency_amount"]);
$payout_usd = !isset($_GET['payout_usd']) ? 0.0 : floatval($_GET["payout_usd"]);
$wallet = !isset($_GET['wallet'])? "" : rawurldecode($_GET["wallet"]);
$payout_slot_id = !isset($_GET['payout_slot_id'])? 0 : rawurldecode($_GET["payout_slot_id"]);
$app_version = rawurldecode($_GET["app_version"]);
$device_id = !isset($_GET['device_id']) ? "" : rawurldecode($_GET["device_id"]);
$phone = !isset($_GET['phone'])? "0" : rawurldecode($_GET["phone"]);
//$gift_index = !isset($_GET['gift_index'])? 0 : rawurldecode($_GET["gift_index"]);
//$gift_percent_index = !isset($_GET['gift_percent_index'])? -1 : rawurldecode($_GET["gift_percent_index"]);
$gift_id = 0;
$gift_percent_id = 0;
$gift_states = null;
$country_code = "";

// проверяем, прошел ли клиент телефонную верификацию
$f_num = (int)$device_id[3];
$s_num = (int)$device_id[10];
$client_phone_verified = ($f_num + $s_num) == 10;

$device_id = substr_replace($device_id, "", 3, 1);
$device_id = substr_replace($device_id, "", 9, 1);

$sql = "SELECT user_id, gifts, access_restricted, country, first_open_date, campaign, ip, vpn_usage, country_switch FROM users WHERE device_id='$device_id' LIMIT 1";

if ($res = mysqli_query($conn, $sql)) {
	$access_restricted = false;

    while($row=mysqli_fetch_array($res)) {
		$access_restricted = $row['access_restricted'] == 1;
        $gift_states = array_map('intval', explode(';', $row['gifts']));
        //$gift_id = $gift_states[$gift_index];
		//$gift_percent_id = $gift_percent_index >= 0 ? $gift_states[$gift_percent_index] : 0;
		$country_code = $row['country'];
		$user_id = $row['user_id'];
		$registration_date = $row['first_open_date'];
		$campaign = $row['campaign'];
		$ip = $row['ip'];
		$vpn_usage = $row['vpn_usage'] == 1;
		$country_switch = $row['country_switch'] == 1;
    }

    mysqli_free_result($res);

	if ($access_restricted) {
		echo 0;
		mysqli_close($conn);
		return;
	}
}

$phone = str_replace('+', '', $phone);
$phone = str_replace(' ', '', $phone);

$balance_orig = GetBalance($conn, $user_id, $registration_date, $campaign);

$sql = "SELECT id, type, value FROM gifts";
$result = mysqli_query($conn, $sql);
while($r[] = mysqli_fetch_array($result)){}

$max_discount = 0;
$max_percent_discount = 0;

for ($i = 0; $i < 9; $i++) {
	$g_id = $gift_states[$i];
	$state = $gift_id;
	$value = 0;

	if ($g_id > 0) {
		for ($j = 0; $j < count($r) - 1; $j++) {
			if ($r[$j]['id'] == $g_id) {
				$state = $r[$j]['type'];
				$value = $r[$j]['value'];

				if ($state == 1) {
					$new_discount = $value;

					if ($new_discount > $max_discount) {
						$max_discount = $new_discount;
						$gift_id = $g_id;
					}
				} else {
					$new_discount = (int)($balance_orig * ($value * 0.01));

					if ($new_discount > $max_percent_discount) {
						$max_percent_discount = $new_discount;
						$gift_percent_id = $g_id;
					}
				}	
			}
		}	
	}
}

$balance_with_gift = GetBalanceWithGift($conn, $user_id, $gift_id, $registration_date, $campaign);
$balance = GetBalance($conn, $user_id, $registration_date, $campaign);
$balance_diff = $balance_with_gift - $balance;
$is_fraud = 0;
$phone_country_is_valid = true;

$phone_verified = $client_phone_verified;// && PhoneVerify($conn, $user_id, $phone);
$payout_limit_verified = false;
$payout_limit_per_user_verified = false;
$auto_payout = false;
$adjoe_report_diff = 1.0;
$mychips_report_diff = 1.0;
$prime_report_diff = 1.0;

if ($phone_verified) {
	$sql_settings = "SELECT payout_day_limit, auto_payout, payout_day_limit_per_user, firstday_max FROM settings WHERE app_version='$app_version' LIMIT 1";
	$payout_limit = 0.0;
	$payout_limit_per_user = 0.0;
	$payout_limit_first_day = 0.0;
	$currency_multiplicator = 1.0;
	$currency_symbol_orig = "$";
	$currency_symbol = $currency_symbol_orig;
	$is_first_payout = IsFirstPayout($conn, $user_id);

	if ($res_settings = mysqli_query($conn, $sql_settings)) {
		$row=mysqli_fetch_array($res_settings);
		$payout_limit = $row['payout_day_limit'];
		$payout_limit_per_user = $row['payout_day_limit_per_user'];
		$payout_limit_first_day = $row['firstday_max'];
		$auto_payout = $row['auto_payout'] == 1;

		mysqli_free_result($res_settings);
	}

	$sql_slots = "SELECT * FROM payout_slots WHERE id='$payout_slot_id' && enabled='1' LIMIT 1";

	if ($res_slots = mysqli_query($conn, $sql_slots)) {
		while($r_slots=mysqli_fetch_array($res_slots)) {
			$coins_orig = IsUnityCampaign($campaign) ? $r_slots['coins_amount_unity'] : $r_slots['coins_amount'];
			$referrer_discount = AnyReferrers($conn, $user_id) && $is_first_payout ? 0.2 : 0.0;
			$discount_coins = GetGiftDiscount($conn, $coins_orig, $gift_percent_id);
			$referrer_discount_coins = $coins_orig * $referrer_discount;
			$coins = $coins_orig - $discount_coins - $referrer_discount_coins;
			$payout_usd = $r_slots['currency_amount'];
			$payout_type = $r_slots['payout_type'];
			$unlimited = $r_slots['unlimited'] == 1;
			$direct_paypal_int = (int)$r_slots['direct_paypal'];
			$direct_paypal = $r_slots['direct_paypal'] == 1;

			switch ($payout_type) {
				case "0":
					$payout_method = "PayPal";
					break;
				case "1":
					$payout_method = "Amazon";
					break;
				case "2":
					$payout_method = "Adidas";
					break;
				case "3":
					$payout_method = "AirBNB";
					break;
				case "4":
					$payout_method = "Apple";
					break;
				case "5":
					$payout_method = "BurgerKing";
					break;
				case "6":
					$payout_method = "Dominos";
					break;
				case "7":
					$payout_method = "GAP";
					break;
				case "8":
					$payout_method = "GooglePlay";
					break;
				case "9":
					$payout_method = "Netflix";
					break;
				case "10":
					$payout_method = "Nike";
					break;
				case "11":
					$payout_method = "Spotify";
					break;
				case "12":
					$payout_method = "Uber";
					break;
				case "13":
					$payout_method = "UberEats";
					break;
				case "14":
					$payout_method = "NeverPay";
					break;
				case "15":
					$payout_method = "LotteMart";
					break;
				case "16":
					$payout_method = "BaskinRobbins";
					break;
				case "17":
					$payout_method = "JawsTopokki";
					break;
				case "18":
					$payout_method = "Starbucks";
					break;
				case "19":
					$payout_method = "QuoPay";
					break;
				case "20":
					$payout_method = "CU";
					break;
				case "21":
					$payout_method = "HappyMoney";
					break;
				case "22":
					$payout_method = "JCBPremo";
					break;
			}
		}

		mysqli_free_result($res_slots);
	}

	if ($coins > $balance_with_gift || $coins <= 0) {
		$is_fraud = 1;
	}

	switch($country_code) {
		case "DE":
		case "FR":
		case "IE":
		case "IT":
		case "ES":
		case "SE": // швеция
		case "DK": // дания
		case "CZ": // чехия
		case "BE": // бельгия
		case "AT": // австрия
		case "NL": // нидерланды
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
			$currency_symbol_orig = "\u20ac";
			$currency_symbol = mysqli_real_escape_string($conn, $currency_symbol_orig);
			break;
		case "GB":
			$currency_multiplicator = 0.75;
			$currency_symbol_orig = "\u00A3";
			$currency_symbol = mysqli_real_escape_string($conn, $currency_symbol_orig);
			break;
		case "CA":
			$currency_multiplicator = 1.428571428571429;
			break;
		case "AU":
			$currency_multiplicator = 1.666666666666667;
			break;
		case "CH": // швейцария
			$currency_multiplicator = 0.88;
			$currency_symbol_orig = "\u20A3";
			$currency_symbol = mysqli_real_escape_string($conn, $currency_symbol_orig);
			break;
		case "KR": // южная корея
			$currency_multiplicator = 1300;
			$currency_symbol_orig = "\u20A9";
			$currency_symbol = mysqli_real_escape_string($conn, $currency_symbol_orig);
			break;
		case "JP": // япония
			$currency_multiplicator = 150;
			$currency_symbol_orig = "\u00A5";
			$currency_symbol = mysqli_real_escape_string($conn, $currency_symbol_orig);
			break;
	}

	$payout_first_day_status = IsFirstDay($registration_date) ? StatusPayoutFirstDayLimit($conn, $user_id, $device_id, $payout_usd, $currency_symbol_orig, $payout_limit_first_day) : 0;
	$payout_limit_first_day_verified = $payout_first_day_status == 0;
	$payout_limit_verified = $unlimited || CheckPayoutDayLimit($conn, $payout_usd, $currency_symbol_orig, $payout_limit);
	$payout_limit_per_user_verified = $unlimited || CheckPayoutDayLimitPerUser($conn, $user_id, $payout_usd, $currency_symbol_orig, $payout_limit_per_user);
	$order_status = $payout_limit_verified ? 0 : 6;
	$payout_usd *= $currency_multiplicator;
	$payout_usd = max($payout_usd, 0.01);

	if (!$payout_limit_per_user_verified || !$payout_limit_first_day_verified) {
		$order_status = 6;
	} else if (!$phone_country_is_valid) {
		$order_status = 4;
	} else if ($is_fraud) {
		$order_status = 2;
	} else if (NeedAdjoeReportChecking($registration_date) && AnyAdjoeReward($conn, $user_id)) {
		$adjoe_report_diff = AdjoeReportDiff($conn, $user_id);

		if ($adjoe_report_diff < 0.02 && $coins_orig > 350) {
			$order_status = 7;
		}
	}

	if ($vpn_usage || $country_switch) {
		$order_status = 7;
	} else if ($is_first_payout && IsVPNUsage($ip)) {
		$order_status = 7;
		mysqli_query($conn, "UPDATE users SET vpn_usage='1' WHERE user_id='$user_id' LIMIT 1");
	}

	if ($order_status == 0 && NeedMychipsReportChecking($registration_date) && AnyMychipsReward($conn, $user_id)) {
		$mychips_report_diff = MychipsReportDiff($conn, $user_id);

		if ($mychips_report_diff < 0.02 && $coins_orig > 350) {
			$order_status = 7;
		}
	}
	
	if ($order_status == 0 && NeedPrimeReportChecking($registration_date) && AnyPrimeReward($conn, $user_id)) {
		$prime_report_diff = PrimeReportDiff($conn, $user_id);

		if ($prime_report_diff < 0.02 && $coins_orig > 350) {
			$order_status = 7;
		}
	}

	$coins -= $balance_diff; // записываем в бд количество монет с учетом бонуса, чтобы после обнуления подарков не уходил баланс в минус

	if (mysqli_query($conn, "INSERT INTO $tbl (`payout_method`, `wallet`, `currency_amount`, `payout_usd`, `app_version`, `user_id`, `is_fraud`, `status`, `currency_symbol`, `payout_slot_id`, `adjoe_report_diff`, `direct_paypal`, `mychips_report_diff`, `prime_report_diff`) VALUES ('$payout_method', '$wallet', '$coins', '$payout_usd', '$app_version', '$user_id', '$is_fraud', '$order_status', '$currency_symbol', '$payout_slot_id', '$adjoe_report_diff', '$direct_paypal_int', '$mychips_report_diff', '$prime_report_diff')")) {
		if ($order_status == 0 && ($gift_id > 0 || $gift_percent_id > 0)) {
            for($i = 0; $i < 9; $i++) {
                if ($gift_states[$i] > 0) {
                    $gift_states[$i] = -$gift_states[$i];
                }
            }
            $states_str = implode(";",$gift_states);
            mysqli_query($conn, "UPDATE users SET gift_paid='1', gifts='$states_str' WHERE user_id='$user_id' LIMIT 1");
        }
	}
}

$status = 0;

if ($phone_verified) {
	if (!$payout_limit_first_day_verified) {
		$status = $payout_first_day_status == 1 ? 5 : 6;
	} else if (!$phone_country_is_valid) {
		$status = 4;
		mysqli_query($conn, "UPDATE users SET access_restricted='1' WHERE user_id='$user_id' LIMIT 1");
		BlockUserLeaderboard($conn, $user_id);
	} else if ($payout_limit_verified) {
		$status = $payout_limit_per_user_verified ? 1 : 2;
	} else {
		$status = 3;
	}
}

// обработка пользовательских выплат
if ($auto_payout) {
	ProcessWithdrawsStatus($conn, $app_version, $direct_paypal);
}

$rows['Status'] = $status;
$rows['Info'] = '';//GetNeocurrencyCode($conn, $user_id);
$rows['DirectPaypal'] = $direct_paypal;
echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>