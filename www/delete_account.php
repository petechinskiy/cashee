<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);
$ip = GetClientIP();

if ($device_id != "") {
	$user_id = GetUserId($conn, $device_id);

	if ($user_id != 0) {
		$sql_w = "SELECT * FROM withdraws WHERE user_id='$user_id'";

		if ($res_w = mysqli_query($conn, $sql_w)) {
			while ($r_w = mysqli_fetch_array($res_w)) {
				$payout_method = $r_w['payout_method'];
				$wallet = $r_w['wallet'];
				$coins = $r_w['currency_amount'];
				$payout_usd = $r_w['payout_usd'];
				$order_status = $r_w['status'];
				$currency_symbol = $r_w['currency_symbol'];
				$timestamp = $r_w['timestamp'];
				
				mysqli_query($conn, "INSERT INTO deleted_withdraws (payout_method, wallet, currency_amount, payout_usd, status, currency_symbol, device_id, timestamp) VALUES ('$payout_method', '$wallet', '$coins', '$payout_usd', '$order_status', '$currency_symbol', '$device_id', '$timestamp')");
			}

			mysqli_free_result($res_w);
		}

		mysqli_query($conn, "INSERT INTO deleted_users (ip) VALUES ('$ip')");

		mysqli_query($conn, "DELETE FROM users WHERE user_id='$user_id' LIMIT 1");
		mysqli_query($conn, "DELETE FROM income_unityads WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM income_ayet WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM income_adjoe WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM income_prime WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM income_samurai WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM income_mychips WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM withdraws WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM referrer_codes WHERE user_id='$user_id' LIMIT 1");
		mysqli_query($conn, "DELETE FROM income_referrer WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM referrer_callbacks WHERE user_id='$user_id'");
		mysqli_query($conn, "UPDATE users SET referrer_user_id='0' WHERE referrer_user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM missions_completed WHERE user_id='$user_id'");
		mysqli_query($conn, "DELETE FROM daily_streak_rewards WHERE user_id='$user_id'");

		mysqli_close($conn);
	}
}
?>