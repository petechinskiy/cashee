<?php
include('config.inc.php');
include('functions.inc.php');

$file = 'log.txt';
// Read the log file
$current = file_get_contents($file);
// Read the JSON file
$json = file_get_contents("php://input");
// Add to the log
$current .= $json . "\n";
// Write to the log file
file_put_contents($file, $current);

$result_array = json_decode($json, true);

$tbl = "income_samurai";
$app_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJidW5kbGVfaWQiOiJjb20ucGx1c2dhbWVzLnJld2FyZGVlIiwiaWQiOjEyNDQsInVzZXJfaWQiOjU0Nn0.4qc2yz9ziumph_h1yxf-RBGzXH_wIn1W7JSdGp4zWJ0";
$s2s_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzMnNfdXJsIjoiaHR0cHM6Ly9yZXdhcmRlZXRyYWNrLmNvbS9jYWxsYmFja19zYW11cmFpLnBocCJ9.JWZxehIR2qolFmSVAL0EzZPdLRguQQnAZb4akDJgeDM";

$trans_id = $result_array['trans_id'];
$device_id = $result_array['user_id'];
$coin_amount = $result_array['coin_amount'];
$coin_currency = $result_array['coin_currency'];
$sig = $result_array['sig'];
$type = $result_array['type'];

$data = $trans_id . $device_id . $coin_amount . $coin_currency . $app_token . $s2s_token;

$computed_sig = hash('sha256', $data);
$is_valid = $sig == $computed_sig;

$reward = 0;

if ($type != "install") {
	if (array_key_exists('rewards', $result_array)) {
		if (array_key_exists('points', $result_array['rewards'])) {
			$reward = $result_array['rewards']['points'];
		} else {
			$reward = array_sum(array_column($result_array['rewards'],'points'));
		}
	}
}

if (!$is_valid)
{
	echo "Not valid signature.";
}
else 
{
	$user_id = 0;
	$referrer_lvl_1 = 0;
	$gps_adid = "";
	$ip = "";

	$sql = "SELECT * FROM users WHERE device_id='$device_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
			$referrer_lvl_1 = $r['referrer_user_id'];
			$gps_adid = $r['gps_adid'];
			$ip = $r['ip'];
			$country_code = $r['country'];
		}

		mysqli_free_result($result);
	}

	if ($user_id == 0) {
		echo "User is not found";
	} else if (mysqli_query($conn, "INSERT INTO $tbl (trans_id, coin_amount, user_id)
									VALUES ('$trans_id', '$reward', '$user_id')")) {
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

		UpdateLeaderboard($conn, $user_id, $reward, $country_code);
	}
}

mysqli_close($conn);
?>