<?php
include('config.inc.php');
include('functions.inc.php');

$device_id = !isset($_GET['device_id'])? "" : rawurldecode($_GET["device_id"]);

$user_id = GetUserId($conn, $device_id);

if ($user_id == 0) {
	return;
}

$rows = [];

$sql_user = "SELECT gifts, gifts_group, last_gift_timestamp, gift_paid, first_open_date, campaign FROM users WHERE user_id='$user_id' LIMIT 1";

if ($result_user = mysqli_query($conn, $sql_user)) {
	$r_user = mysqli_fetch_array($result_user);
	$group = $r_user['gifts_group'];
	$gift_paid = $r_user['gift_paid'] == 1;
	$registration_date = $r_user['first_open_date'];
	$campaign = $r_user['campaign'];
	$states = array_map('intval', explode(';', $r_user['gifts']));

	$diff_hours = 24;
	$target_diff_time = $diff_hours * 60 * 60;
	$last_time = strtotime($r_user['last_gift_timestamp']);
	$current_time = time();
	$left_time = $last_time + $target_diff_time - $current_time;

	$rows['WasPaid'] = $gift_paid;
	$rows['BestGiftIndex'] = -1;
	$rows['BestGiftPercentIndex'] = -1;
	$rows['LeftSecondsToUpdate'] = $left_time;

	// если подарок уже был использован с выплатой и заданное время ожидания прошло, то обновляем подарки
	if ($gift_paid && $left_time < 20) {
		$group = 1;
		
		for($i = 0; $i < 9; $i++) {
			$states[$i] = 0;
		}

		$states_str = implode(";",$states);
		$rows['WasPaid'] = false;
		mysqli_query($conn, "UPDATE users SET gifts='$states_str', gift_paid='0', gifts_group='$group' WHERE user_id='$user_id' LIMIT 1");
	}

	$balance = GetBalance($conn, $user_id, $registration_date, $campaign);

	$sql = "SELECT id, type, value FROM gifts";
	$result = mysqli_query($conn, $sql);
	while($r[] = mysqli_fetch_array($result)){}

	
	$max_discount = 0;
	$max_percent_discount = 0;

	for ($i = 0; $i < 9; $i++) {
		$gift_id = $states[$i];
		$state = $gift_id;
		$value = 0;

		if ($gift_id != 0) {
			for ($j = 0; $j < count($r) - 1; $j++) {
				if ($r[$j]['id'] == abs($gift_id)) {
                    $state = $gift_id > 0 ? $r[$j]['type'] : -$r[$j]['type'];
					$value = $r[$j]['value'];

					if ($state == 1) {
						$new_discount = $value;

						if ($new_discount > $max_discount) {
							$max_discount = $new_discount;
							$rows['BestGiftIndex'] = $i;
						}
					} else if ($state == 2) {
						$new_discount = (int)($balance * ($value * 0.01));

						if ($new_discount > $max_percent_discount) {
							$max_percent_discount = $new_discount;
							$rows['BestGiftPercentIndex'] = $i;
						}
					}	
				}
			}	
		}

		$jsonArrayObject = array('State' => $state, 'Value' => $value);
		$rows['States'][] = $jsonArrayObject;
	}

	mysqli_free_result($result);
	mysqli_free_result($result_user);
}

echo json_encode($rows, JSON_PRETTY_PRINT);

mysqli_close($conn);
?>