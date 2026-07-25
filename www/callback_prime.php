<?php
include('config.inc.php');
include('functions.inc.php');

$tbl = "income_prime";
$ip_whitelist = ['168.119.57.82','49.12.33.196','49.13.14.251'];
$ip = GetClientIP();

function isValidHash(): bool
{
    $secretSalt = '7PHkQJmKP4SdEF70GiefgjlnsWZD2SDI';

    $receivedHash = $_GET['hash'] ?? null;

    if ($receivedHash === null) {
        error_log('Hash is missing');
        return false;
    }

    // Safer if you are behind Cloudflare / proxy / nginx
    $scheme = $_SERVER['HTTP_X_FORWARDED_PROTO']
        ?? ((!empty($_SERVER['HTTPS']) && $_SERVER['HTTPS'] !== 'off') ? 'https' : 'http');

    if (str_contains($scheme, ',')) {
        $scheme = trim(explode(',', $scheme)[0]);
    }

    $host = $_SERVER['HTTP_HOST'];

    // Path without query string
    $path = parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH);

    // Preserve original encoding/order exactly
    $rawQuery = $_SERVER['QUERY_STRING'] ?? '';
    $parts = $rawQuery === '' ? [] : explode('&', $rawQuery);

    $filteredParts = [];

    foreach ($parts as $part) {
        if ($part === '') {
            continue;
        }

        $key = explode('=', $part, 2)[0];
        $decodedKey = urldecode($key);

        if ($decodedKey === 'hash' || $decodedKey === 'shash') {
            continue;
        }

        $filteredParts[] = $part;
    }

    $urlWithoutHashes = $scheme . '://' . $host . $path;

    if (!empty($filteredParts)) {
        $urlWithoutHashes .= '?' . implode('&', $filteredParts);
    }

    $computedHash = sha1($urlWithoutHashes . $secretSalt);

    return hash_equals(strtolower($computedHash), strtolower($receivedHash));
}

$is_valid = isValidHash();

$device_id = !isset($_GET['user_uuid'])? "" : rawurldecode($_GET["user_uuid"]);
$reward = !isset($_GET['reward'])? 0 : rawurldecode($_GET["reward"]);
$revenue = !isset($_GET['payout'])? 0.0 : rawurldecode($_GET["payout"]);
$tx_id = !isset($_GET['tx_id'])? 0.0 : rawurldecode($_GET["tx_id"]);
$postback_type = !isset($_GET['type'])? 0 : rawurldecode($_GET["type"]);
$offer_name = !isset($_GET['offer_name'])? "" : rawurldecode($_GET["offer_name"]);
$offer_name = mysqli_real_escape_string($conn, $offer_name);
$task_name = !isset($_GET['task_name'])? "" : rawurldecode($_GET["task_name"]);
$task_name = mysqli_real_escape_string($conn, $task_name);
$task_name = iconv_substr($task_name, 0, 64, 'UTF-8');

if (!$is_valid)
{
	echo "Not valid";
	//$file = 'mychips_log.txt';
	//$current = file_get_contents($file);
	//$content = "ip:$ip; postback_id:$postback_id; user_payout:$user_payout; device_id:$device_id; adunit_id:$adunit_id";
	//$current .= $content . "\n";
	//file_put_contents($file, $current);
}
else if ($postback_type == 7 || $postback_type == 9)
{
	$user_id = 0;
	$referrer_lvl_1 = 0;
	$current_date = new DateTime();
	$current_date_str = $current_date->format('Y-m-d');

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

	if ($user_id == 0 || PrimeTransactionIsExist($conn, $user_id, $tx_id)) {
		echo "User is not found";
	} else if (mysqli_query($conn, "INSERT INTO $tbl (`tx_id`, `reward`, `revenue`, `user_id`, `type`, `offer_name`, `task_name`, `timestamp`)
									VALUES ('$tx_id', '$reward', '$revenue', '$user_id', $postback_type, '$offer_name', '$task_name', '$current_date_str')")) {
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