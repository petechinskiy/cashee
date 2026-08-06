<?php
function GetUserId($conn, $device_id) {
	$id = 0;
	$sql = "SELECT user_id FROM users WHERE device_id='$device_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$id = $r['user_id'];
		}

		mysqli_free_result($result);
	}

	return $id;
}

function GetUserIdByAdid($conn, $gps_adid) {
	$id = 0;
	$sql = "SELECT user_id FROM users WHERE gps_adid='$gps_adid' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$id = $r['user_id'];
		}

		mysqli_free_result($result);
	}

	return $id;
}

function GetDeviceId($conn, $user_id) {
	$id = 0;
	$sql = "SELECT device_id FROM users WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$id = $r['device_id'];
		}

		mysqli_free_result($result);
	}

	return $id;
}

function GetNeocurrencyCode($conn, $user_id) {
	$code = '';
	$sql = "SELECT neocurrency_code FROM withdraws WHERE user_id='$user_id' ORDER BY id DESC LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$code = $r['neocurrency_code'];
		}

		mysqli_free_result($result);
	}

	return $code;
}

function GetUserStatus($conn, $user_id, $registration_date_str, $campaign, $click_earn_button) {
	$status = 0;
	$pluscoins = GetBalance($conn, $user_id, $registration_date_str, $campaign, false);

	$now = time();
	$registration_date = strtotime($registration_date_str);
	$datediff = $now - $registration_date;
    $days_left = round($datediff / (60 * 60 * 24));

	if ($pluscoins > 500000 && $days_left > 60) {
		$status = 13;
	} else if ($pluscoins > 250000 && $days_left > 60) {
		$status = 12;
	} else if ($pluscoins > 250000 && $days_left > 30) {
		$status = 11;
	} else if ($pluscoins > 250000 && $days_left > 7) {
		$status = 10;
	} else if ($pluscoins > 150000 && $days_left > 7) {
		$status = 9;
	} else if ($pluscoins > 50000 && $days_left > 30) {
		$status = 8;
	} else if ($pluscoins > 120000 && $days_left > 30) {
		$status = 7;
	} else if ($pluscoins > 100000 && $days_left > 7) {
		$status = 6;
	} else if ($pluscoins > 25000 && $days_left > 7) {
		$status = 5;
	} else if ($pluscoins > 5000) {
		$status = 4;
	} else if ($pluscoins > 2000) {
		$status = 3;
	} else if ($pluscoins > 1000) {
		$status = 2;
	} else if ($click_earn_button) {
		$status = 1;
	}

	return $status;
}

function GetBalance($conn, $user_id, $timestamp, $campaign, $include_payouts = true) {
	$start_bonus = 0; //CheckWelcomeBonus($timestamp, $campaign) ? 300 : 0;
	$total = 0;

	$sql = "
	        SELECT
	            (
	                SELECT SUM(currency_amount) 
	                FROM income_ayet 
	                WHERE user_id='$user_id'
	            ) AS ayet_total,
		    (
	                SELECT SUM(currency_amount) 
	                FROM income_unityads 
	                WHERE user_id='$user_id'
	            ) AS unityads_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_adjoe 
	                WHERE user_id='$user_id'
	            ) AS adjoe_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_samurai 
	                WHERE user_id='$user_id'
	            ) AS samurai_total,
		    (
	                SELECT SUM(reward) 
	                FROM income_mychips 
	                WHERE user_id='$user_id'
	            ) AS mychips_total,
			(
	                SELECT SUM(reward) 
	                FROM income_prime 
	                WHERE user_id='$user_id' && type=7
	            ) AS prime_total,
			(
	                SELECT SUM(reward) 
	                FROM missions_completed 
	                WHERE user_id='$user_id'
	            ) AS missions_total,
			(
	                SELECT SUM(reward) 
	                FROM daily_streak_rewards 
	                WHERE user_id='$user_id'
	            ) AS daily_streak_total,
		    (
	                SELECT SUM(currency_amount) 
	                FROM withdraws 
	                WHERE user_id='$user_id' && is_fraud=0 && (status <= 1 || status = 9)
	            ) AS withdraw_total,
			(
	                SELECT SUM(coins) 
	                FROM income_referrer
	                WHERE user_id='$user_id'
	            ) AS referrer_total";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$payouts = $include_payouts ? $row['withdraw_total'] : 0;
		    $total = $start_bonus + $row['adjoe_total'] + $row['ayet_total'] + $row['unityads_total'] + $row['samurai_total'] + $row['mychips_total'] + $row['prime_total'] + $row['missions_total'] + $row['daily_streak_total'] + $row['referrer_total'] - $payouts;
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function GetBalanceWithGift($conn, $user_id, $gift_id, $timestamp, $campaign) {
	$balance = GetBalance($conn, $user_id, $timestamp, $campaign);

	if ($gift_id <= 0) {
        return $balance;
	}

	$sql = "SELECT type, value FROM gifts WHERE id='$gift_id' LIMIT 1";
	$result = mysqli_query($conn, $sql);
	$r = mysqli_fetch_array($result);

	$bonus = 0;
	$type = $r['type'];
	$value = $r['value'];

	if ($type == 1) {
		$bonus = $value;
	}

	return $balance + $bonus;
}

function LastEarnCoins($conn, $user_id, $days_amount) {
	$total = 0;

	$sql = "
	        SELECT
	            (
	                SELECT SUM(currency_amount) 
	                FROM income_ayet 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS ayet_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_adjoe 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS adjoe_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_samurai 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS samurai_total,
		    (
	                SELECT SUM(reward) 
	                FROM income_mychips 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS mychips_total,
			(
	                SELECT SUM(reward)  
	                FROM income_prime 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY)) && type=7
	            ) AS prime_total,
			(
	                SELECT SUM(reward) 
	                FROM missions_completed 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS missions_total,
			(
	                SELECT SUM(reward) 
	                FROM daily_streak_rewards 
	                WHERE user_id='$user_id'
	            ) AS daily_streak_total,
		    (
	                SELECT SUM(currency_amount) 
	                FROM income_unityads 
	                WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))
	            ) AS unityads_total,
			(
	                SELECT SUM(coins) 
	                FROM income_referrer
	                WHERE user_id='$user_id'
	            ) AS referrer_total";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row['ayet_total'] + $row['adjoe_total'] + $row['unityads_total'] + $row['samurai_total'] + $row['mychips_total'] + $row['prime_total'] + $row['missions_total'] + $row['daily_streak_total'] + $row['referrer_total'];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function OfferwallEarnedCoins($conn, $user_id) {
	$total = 0;

	$sql = "
	        SELECT
	            (
	                SELECT SUM(currency_amount) 
	                FROM income_ayet 
	                WHERE user_id='$user_id'
	            ) AS ayet_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_adjoe 
	                WHERE user_id='$user_id'
	            ) AS adjoe_total,
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_samurai 
	                WHERE user_id='$user_id'
	            ) AS samurai_total,
			(
	                SELECT SUM(reward) 
	                FROM income_mychips 
	                WHERE user_id='$user_id'
	            ) AS mychips_total, 
			(
	                SELECT SUM(reward) 
	                FROM income_prime 
	                WHERE user_id='$user_id' && type=7
	            ) AS prime_total";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row['ayet_total'] + $row['adjoe_total'] + $row['samurai_total'] + $row['mychips_total'] + $row['prime_total'];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function HasOfferwallRewardToday($conn, $user_id) {
	$total = 0;
	$date = new DateTime();
	$date_str = $date->format('Y-m-d');

	$sql = "
	        SELECT
		    (
	                SELECT SUM(coin_amount) 
	                FROM income_adjoe 
	                WHERE user_id='$user_id' && timestamp='$date_str'
	            ) AS adjoe_total,
			(
	                SELECT SUM(reward) 
	                FROM income_mychips 
	                WHERE user_id='$user_id' && timestamp='$date_str'
	            ) AS mychips_total, 
			(
	                SELECT SUM(reward) 
	                FROM income_prime 
	                WHERE user_id='$user_id' && type=7 && timestamp='$date_str'
	            ) AS prime_total";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row['adjoe_total'] + $row['mychips_total'] + $row['prime_total'];
		}
		mysqli_free_result($result);
	}
	
	return $total > 0;
}

function SamuraiCoins($conn, $user_id) {
	$total = 0;

	$sql = "SELECT SUM(coin_amount) FROM income_samurai WHERE user_id='$user_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function AdjoeCoins($conn, $user_id) {
	$total = 0;

	$sql = "SELECT SUM(coin_amount) FROM income_adjoe WHERE user_id='$user_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function AdjoeCoinsToDate($conn, $user_id, $date) {
	$total = 0;

	$sql = "SELECT SUM(coin_amount) FROM income_adjoe WHERE user_id='$user_id' && is_bonus_purchase=0 && timestamp <= '$date'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function AdjoeReportCoins($conn, $user_id) {
	$total = 0;

	$sql = "SELECT SUM(reward) FROM income_adjoe_report WHERE user_id='$user_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function AdjoeReportDiff($conn, $user_id) {
	$total = 1.0;

	$sql = "SELECT
  (
    SELECT SUM(cpi)
    FROM adjoe_installs
    WHERE user_id = '$user_id'
  )
  /
  NULLIF(
    (
      SELECT SUM(coin_amount) / 3000
      FROM income_adjoe
      WHERE user_id = '$user_id' && is_bonus_purchase=0
    ),
    0
  ) AS adjoe_report_diff";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    if ($row[0] > 0) {
				$total = (float)$row[0];
			}
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function LastAdjoeCoins($conn, $user_id, $days_amount) {
	$total = 0;

	$sql = "SELECT SUM(coin_amount) FROM income_adjoe WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$days_amount' DAY))";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function AdjoeCoinsPerTime($conn, $user_id, $hours) {
	$total = 0;

	$sql = "SELECT SUM(coin_amount) FROM income_adjoe WHERE user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$hours' HOUR))";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function LastAdjoeReportDate($conn) {
	$date_str = '2020-01-01';

	$sql = "SELECT date FROM adjoe_reports ORDER BY id DESC LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$date_str = $r['date'];
		}

		mysqli_free_result($result);
	}

	return $date_str;
}

function IsIPv6($ip) {
	return false;//filter_var($ip, FILTER_VALIDATE_IP, FILTER_FLAG_IPV6);
}

// проверка подключения c одного IP не более 5 устройств
function CheckIPNoMoreDevice($conn, $user_id, $ip) {
	$count = 0;

	$sql = "SELECT COUNT(*) AS total_records
FROM (
    SELECT ip
    FROM users
    WHERE timestamp > DATE_SUB(NOW(), INTERVAL 1 MONTH)
      AND ip = '$ip'

    UNION ALL

    SELECT ip
    FROM deleted_users
    WHERE timestamp > DATE_SUB(NOW(), INTERVAL 1 MONTH)
      AND ip = '$ip'
) t";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$count = (int)$row[0];
		}
		mysqli_free_result($result);
	}
	
	return $count <= 5;
}

function CheckClient() {
	return str_contains($_SERVER['HTTP_USER_AGENT'],'UnityPlayer');
}

function GetClientIP() {
    if (!empty($_SERVER['HTTP_X_FORWARDED_FOR'])) {
        // Split the X-Forwarded-For header into an array
        $forwarded_ips = explode(',', $_SERVER['HTTP_X_FORWARDED_FOR']);
        
        // The first IP in the list is the real client IP (AWS specific)
        $client_ip = trim($forwarded_ips[0]);
    } else {
        // Fallback to REMOTE_ADDR if X-Forwarded-For is not present
        $client_ip = $_SERVER['REMOTE_ADDR'];
    }

    return $client_ip;
}

// верификация по номеру телефона
function PhoneVerify($conn, $user_id, $phone) {
	$success = false;
	$tbl = "users";

	$sql = "SELECT phone, is_organic FROM $tbl WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		if ($count > 0) {
			while($row=mysqli_fetch_array($result)) {
				$is_organic = $row['is_organic'] == 1;

				if (!$is_organic) {
					return true;
				}

				if ($row['phone'] == 0) {
					if (CheckPhoneDuplicate($conn, $user_id, $phone)) {
						$success = true;
						mysqli_query($conn, "UPDATE $tbl SET phone='$phone' WHERE user_id='$user_id'");
					}
				} else {
		    			$success = $row['phone'] == $phone;
				}
			}
		}

		mysqli_free_result($result);
	}
	
	return $success;
}

// если адрес вывода средств совпадает с другим пользователем
function IsDuplicateWallet($conn, $user_id, $wallet) {
	$is_duplicate = false;
	$tbl = "withdraws";

	$sql = "SELECT user_id FROM $tbl WHERE wallet='$wallet' && (status<=1 || status=9) && user_id != '$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		if ($count > 0) {
			$is_duplicate = true;
		}

		mysqli_free_result($result);
	}
	
	return $is_duplicate;
}

function AccessRestricted($conn, $user_id) {
	$access_restricted = false;

	$sql = "SELECT access_restricted FROM users WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$access_restricted = $row['access_restricted'] > 0 || WithdrawIsBlocked($conn, $user_id);
		}

		mysqli_free_result($result);
	}
	

	return $access_restricted;
}

function WithdrawIsBlocked($conn, $user_id) {
	$is_blocked = false;
	$tbl = "withdraws";

	$sql = "SELECT id FROM $tbl WHERE user_id='$user_id' && (status=3 || status=5 || status=7) LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);
		$is_blocked = $count > 0;

		mysqli_free_result($result);
	}
	
	return $is_blocked;
}

function GetPhoneNumber($conn, $user_id) {
	$phone_number = "";
	$tbl = "users";

	$sql = "SELECT phone FROM $tbl WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		if ($count > 0) {
			while($row=mysqli_fetch_array($result)) {
		    		$phone_number = $row['phone'];
			}
		}

		mysqli_free_result($result);
	}
	
	return $phone_number;
}

function CheckPhoneDuplicate($conn, $user_id, $phone) {
	$is_duplicate = true;
	$tbl = "users";

	$sql = "SELECT phone FROM $tbl WHERE phone='$phone' && user_id != '$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		$is_duplicate = $count > 0;

		mysqli_free_result($result);
	}
	
	return !$is_duplicate;
}

function DeviceIsDuplicate($conn, $user_id, $device_id) {
	$is_duplicate = false;
	$tbl = "users";

	$sql = "SELECT device_id FROM $tbl WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		if ($count > 0) {
			while($row=mysqli_fetch_array($result)) {
				$is_duplicate = $row['device_id'] != "" && $row['device_id'] != $device_id;
			}
		}

		mysqli_free_result($result);
	}
	
	return $is_duplicate;
}

function GetNewtworkName($conn, $gps_adid) {
	$name = "";

	$sql = "SELECT network_name FROM ironsource_installs WHERE gps_adid='$gps_adid' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$name = $row['network_name'];
		}

		mysqli_free_result($result);
	}
	
	return $name;
}

function SendRevenueEvent($adid, $revenue, $unixtime, $event_token) {
    $s2_token = "23fcf342902e5aac1f1e43604b0d5817";
    $app_token = "x0fbgnfsighs";

    $adid_query = str_contains($adid, '-') ? '&gps_adid='.$adid : '&adid='.$adid;
    $url = 'https://s2s.adjust.com/event?s2s=1&event_token='.$event_token.'&app_token='.$app_token.'&currency=USD&created_at_unix='.$unixtime.'&environment=production&revenue='.$revenue.$adid_query;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        
    $result = curl_exec($ch);

    curl_close($ch);

    return $result;
}

function SendEvent($adid, $unixtime, $event_token) {
    $s2_token = "23fcf342902e5aac1f1e43604b0d5817";
    $app_token = "x0fbgnfsighs";

    $adid_query = str_contains($adid, '-') ? '&gps_adid='.$adid : '&adid='.$adid;
    $url = 'https://s2s.adjust.com/event?s2s=1&event_token='.$event_token.'&app_token='.$app_token.'&created_at_unix='.$unixtime.'&environment=production'.$adid_query;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        
    $result = curl_exec($ch);

    curl_close($ch);

    return $result;
}

function IsFirstRewardAdjoe($conn, $user_id) {
	$is_first_reward = false;
	$tbl = "income_adjoe";

	$sql = "SELECT id FROM $tbl WHERE user_id='$user_id' && coin_amount>0 LIMIT 2";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);
		$is_first_reward = $count == 1;
		
		if ($count > 0) {
            		mysqli_query($conn, "UPDATE users SET first_adjoe_reward_received='1' WHERE user_id='$user_id'");
        	}

		mysqli_free_result($result);
	}
	
	return $is_first_reward;
}

function IsFirstPayout($conn, $user_id) {
	$success = false;

	$sql = "SELECT id FROM withdraws WHERE user_id='$user_id' && (status=0 || status=1 || status=9) LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);
		$success = $count == 0;

		mysqli_free_result($result);
	}
	
	return $success;
}

// первая награда была получена (исключая награду за рекламу)
function AnyRewardReceivedWithoutAds($conn, $user_id) {
    $count = 0;

    $sql = "
            SELECT
                (
                    SELECT COUNT(*) 
                    FROM income_ayet 
                    WHERE external_identifier='$user_id'
                    LIMIT 2
                ) AS ayet_count,
                (
                    SELECT COUNT(*) 
                    FROM income_adjoe 
                    WHERE user_uuid='$user_id' && is_bonus_purchase=0
                    LIMIT 2
                ) AS adjoe_count,
				(
                    SELECT COUNT(*) 
                    FROM income_samurai 
                    WHERE user_id='$user_id'
                    LIMIT 2
                ) AS samurai_count,
				(
                    SELECT COUNT(*) 
                    FROM income_mychips 
                    WHERE user_id='$user_id'
                    LIMIT 2
                ) AS mychips_count,
				(
                    SELECT COUNT(*) 
                    FROM income_prime 
                    WHERE user_id='$user_id' && type != 9
                    LIMIT 2
                ) AS prime_count";
    
    if ($result = mysqli_query($conn, $sql)) {
        $row=mysqli_fetch_array($result);
        $count = $row['ayet_count'] + $row['adjoe_count'] + $row['samurai_count'] + $row['mychips_count'] + $row['prime_count'];
        
        mysqli_free_result($result);
    }
    
    return $count >= 1;
}

// первая награда за рекламу была получена
function AnyAdsRewardReceived($conn, $user_id) {
	$count = 0;

	$sql = "
	        SELECT
	            (
	                SELECT COUNT(*) 
	                FROM income_unityads 
	                WHERE device_uuid='$user_id'
					LIMIT 2
	            ) AS ads_count";
	
	if ($result = mysqli_query($conn, $sql)) {
		$row=mysqli_fetch_array($result);
		$count = $row['ads_count'];
		
		mysqli_free_result($result);
	}
	
	return $count >= 1;
}

// был получен хотя бы один доход
function AnyRewardReceived($conn, $user_id) {
	$first_reward_received = AnyAdsRewardReceived($conn, $user_id) || AnyRewardReceivedWithoutAds($conn, $user_id);
	$tbl = "users";

	$sql = "SELECT inst_bonus_received FROM $tbl WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$row = mysqli_fetch_array($result);
		if ($row['inst_bonus_received'] == 1) {
			$first_reward_received = true;
		}
		
		mysqli_free_result($result);
	}
	
	return $first_reward_received;
}

function SuccessPayoutCount($conn, $user_id) {
	$count = 0;
	$tbl = "withdraws";

	$sql = "SELECT id FROM $tbl WHERE user_id='$user_id' && status='1'";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count;
}

function CanShowAdjoe($is_organic, $country_code, $campaign, $registration_date) {
	return true; //!$is_organic || IsOldUser($registration_date);
}

function CampaignIsExists($conn, $campaign) {
	//return str_contains($campaign, '8652175') || str_contains($campaign, '8652815') || str_contains($campaign, '8654543');
	$count = 0;
	
	$sql = "SELECT id FROM campaign_whitelist WHERE campaign_id='$campaign' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count > 0;
}

function InstallIsExists($conn, $gps_adid) {
	$count = 0;
	
	$sql = "SELECT id FROM ironsource_installs WHERE gps_adid='$gps_adid' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count > 0;
}

function IsOldUser($timestamp) {
	$start_date = new DateTime('2025-01-27 18:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');
	return strtotime($timestamp) < strtotime($start_date_str);
}

function IsFirstDay($registration_date) {
	$current_timestamp = time();
	$diff = $current_timestamp - strtotime($registration_date);

	return $diff < 24 * 60 * 60;
}

function NeedAdjoeReportChecking($timestamp) {
	return true;

	//$start_date = new DateTime('2025-12-21');
	//$start_date_str = $start_date->format('Y-m-d');
	//return strtotime($timestamp) > strtotime($start_date_str);
}

function IsUnityCampaign($campaign) {
	$campaign = strtolower($campaign);
	return str_contains($campaign, "unity") || str_contains($campaign, "iron");
}

function HasWelcomeBonus($registration_date) {
	$start_date = new DateTime('2026-04-03 00:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');

	return strtotime($registration_date) > strtotime($start_date_str);
}

function GetGiftDiscount($conn, $balance, $gift_id) {
	if ($gift_id <= 0) {
		return 0;
	}

	$sql = "SELECT type, value FROM gifts WHERE id='$gift_id' LIMIT 1";
	$result = mysqli_query($conn, $sql);
	$r = mysqli_fetch_array($result);

	$discount = 0;
	$type = $r['type'];
	$value = $r['value'];

	if ($type == 1) {
		//$discount = $value;
	} else {
		$discount = (int)($balance * ($value * 0.01));
	}

	return $discount;
}

function IsThirdCountry($country_code) {
	switch($country_code) {
		case "US":
		case "DE": //         
		case "FR": //        
		//case "IE": //         
		case "ES": //        
		case "IT": //       
		case "SE": //       
		//case "DK": //      
		//case "CZ": //      
		case "BE": //        
		case "AT": //        
		case "NL": //           
		case "GB": //               
		case "CA": //       
		case "AU": //          
		case "CH": //          
		case "KR": //            
		case "JP": //       
		case "NO": //         
		case "SG": //         
		case "FI": //          
			return false;
	}

	return true;
}



function IsThirdCountrySql($conn, $user_id) {
	$country_code = "";
	$sql_users = "SELECT country FROM users WHERE user_id='$user_id' LIMIT 1";
	
	if ($res_users = mysqli_query($conn, $sql_users)) {
		$r_users=mysqli_fetch_array($res_users);
		$country_code = $r_users['country'];

		mysqli_free_result($res_users);
	}

	return IsThirdCountry($country_code);
}

function ClearUserName($conn, $name) {
	return preg_replace('/[^A-Za-z0-9 _-]/', '', $name);
}

function GetUserIdByReferrerCode($conn, $code) {
	$user_id = 0;

	if ($code == "") {
		return $user_id;
	}

	$sql = "SELECT user_id FROM referrer_codes WHERE code='$code' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];
		}

		mysqli_free_result($result);
	}

	return $user_id;
}

function GetUserReferrer($conn, $user_id) {
	$referrer_id = 0;

	if ($user_id == 0) {
		return $referrer_id;
	}

	$sql = "SELECT referrer_user_id FROM users WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$referrer_id = $r['referrer_user_id'];
		}

		mysqli_free_result($result);
	}

	return $referrer_id;
}

function GetReferrers($conn, $user_id) {
	$ids = [];

	if ($user_id == 0) {
		return $count;
	}

	$sql = "SELECT user_id FROM users WHERE referrer_user_id='$user_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while ($r=mysqli_fetch_array($result)) {
			$user_id = $r['user_id'];

			$ids[] = $user_id;
		}

		mysqli_free_result($result);
	}

	return $ids;
}

function AnyReferrers($conn, $user_id) {
	$count = 0;

	if ($user_id == 0) {
		return false;
	}

	$sql = "SELECT user_id FROM users WHERE referrer_user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);
		mysqli_free_result($result);
	}

	return $count > 0;
}

function GetReferrerCode($conn, $user_id) {
	$code = "";
	$sql = "SELECT code FROM referrer_codes WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$code = $r['code'];
		}

		mysqli_free_result($result);
	}

	if ($code == "") {
		$code = GenerateReferrerCode($conn);

		mysqli_query($conn, "INSERT INTO referrer_codes (user_id, code) VALUES ('$user_id', '$code')");
	}

	return $code;
}

function AddReferrerReward($conn, $callback_id, $offerwall_type, $user_id, $from_user_id, $coins, $lvl) {
	$reward = $lvl == 1 ? $coins * 0.15 : $coins * 0.05;
	$reward = (int)$reward;

	mysqli_query($conn, "INSERT INTO referrer_callbacks (referrer_level, callback_id, offerwall_type, user_id, from_user_id, coins) VALUES ('$lvl', '$callback_id', '$offerwall_type', '$user_id', '$from_user_id', '$reward')");
}

function GetReferrerBalance($conn, $user_id) {
	$total = 0;

	$sql = "
	        SELECT
			(
	                SELECT SUM(coins) 
	                FROM referrer_callbacks 
	                WHERE user_id='$user_id'
	            ) AS callbacks_total,
			(
	                SELECT SUM(coins) 
	                FROM income_referrer
	                WHERE user_id='$user_id'
	            ) AS referrer_total";
	
	if ($result = mysqli_query($conn, $sql)) {
		$row=mysqli_fetch_array($result);
		$total += $row['callbacks_total'] + $row['referrer_total'];
		
		mysqli_free_result($result);
	}
	
	return $total;
}

function  GenerateReferrerCode($conn) {
	$last_code = "A00A00";

	$sql = "SELECT code FROM referrer_codes ORDER BY id DESC LIMIT 1";

	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$last_code = $r['code'];
		}

		mysqli_free_result($result);
	}

	$num = (int)($last_code[1].$last_code[2].$last_code[4].$last_code[5]);
	$first_letter_old = $first_letter_new = $last_code[0];
	$second_letter_old = $second_letter_new = $last_code[3];

	if ($num >= 9999) {
		$num = 0;
		$second_letter_old = $last_code[3];
		$second_letter_new = GetNextLetter($second_letter_old);

		if ($second_letter_new == 'A') {
			$first_letter_new = GetNextLetter($first_letter_old);
		}
	} else {
		$num++;
	}

	$num_str = (string)$num;

	while (strlen($num_str) < 4) {
		$num_str = '0'.$num_str;
	}

	return $first_letter_new.$num_str[0].$num_str[1].$second_letter_new.$num_str[2].$num_str[3];
}

function GetNextLetter($letter) {
	$index = 0;
	$letters = array('A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z');

	if ($letter != 'Z') {
		$index = array_search($letter, $letters); 
		$index++;
	}

	return $letters[$index];
}

function ProcessWithdrawsStatus($conn, $build_version, $direct_paypal) {
	$tbl = "withdraws";

	$sql = "SELECT id, user_id, status, wallet, is_fraud, payout_usd, payout_method FROM $tbl WHERE status='0' || status='9' && (last_send_time < DATE_SUB(now(), INTERVAL '10' MINUTE)) LIMIT 15";
	
	if ($result = mysqli_query($conn, $sql)) {
		$current_date = date("Y-m-d H:i:s", strtotime('+1 hours')); // доюавляем 1ч для синхронизации с временем в БД

		while($row=mysqli_fetch_array($result)) {
			$status_code = 9;
			$user_id = $row['user_id'];
			$trans_id = $row['id'];
			$wallet = $row['wallet'];
			$country_code = "US";
			$full_name = "A A";

			if ($row['is_fraud'] == 1) {
				$status_code = 2;
			} else {
				$sql_users = "SELECT ip, phone, is_organic, country, first_name, last_name FROM users WHERE user_id='$user_id' LIMIT 10";

				if ($result_users = mysqli_query($conn, $sql_users)) {
					while($row_users=mysqli_fetch_array($result_users)) {
						$ip = $row_users['ip'];
						$phone = $row_users['phone'];
						$is_organic = $row_users['is_organic'] == 1;
						$country_code = $row_users['country'];
						
						if ($row_users['first_name'] != "" || $row_users['last_name'] != "") {
                            $full_name = $row_users['first_name'].' '.$row_users['last_name'];
                        }

						if (!CheckIPNoMoreDevice($conn, $user_id, $ip)) {
							$status_code = 3;
						} else if (IsDuplicateWallet($conn, $user_id, $wallet)) {
							$status_code = 5;
						} //else if ($is_organic && GetCountryCodeByIp($ip) == "US" && !str_starts_with($phone, '1')) {
							//$status_code = 4;
						//}
					}

					mysqli_free_result($result_users);
				}
			}

			if ($status_code == 9) {
				$payout_value = $row['payout_usd'];
				$payout_method = strtolower($row['payout_method']);
				$brand_id = 873; // по умолчанию paypal
				$currency_code = "USD";
				$campaign_id = $payout_method == "paypal" ? "X4XJRPY3KJ3R" : "DJ48X9H73QYZ";

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
						$currency_code = "EUR";
						
						if (!$direct_paypal) {
							$payout_value = round($payout_value * 1.111111111111111, 2, PHP_ROUND_HALF_UP); 
						}
						break;
					case "GB":
						$payout_value = round($payout_value * 1.333333333333333, 2, PHP_ROUND_HALF_UP);
						break;
					case "CA":
						$payout_value = round($payout_value * 0.7, 2, PHP_ROUND_HALF_UP); 
						break;
					case "AU":
						$payout_value = round($payout_value * 0.6, 2, PHP_ROUND_HALF_UP);
						break;
					case "CH": //  
						$currency_code = "EUR";
						$payout_value = round($payout_value * 1.136363636363636, 2, PHP_ROUND_HALF_UP);

						if ($direct_paypal) {
							$payout_value = round($payout_value * 0.9, 2, PHP_ROUND_HALF_UP);
						}
						break;
					case "KR":
						$payout_value = round($payout_value * (1.0 / 1300), 2, PHP_ROUND_HALF_UP);
						break;
					case "JP":
						$payout_value = round($payout_value * 0.0066666666666667, 2, PHP_ROUND_HALF_UP);
						break;
				}
				
				if ($direct_paypal) {
					$token = GetPaypalToken();
						
					if (SendEmailPaypal($wallet, $payout_value, $token, $currency_code)) {
						$status_code = 1;
					} else {
						$status_code = 2;
					}
				} else if (SendTremendous($wallet, $payout_value, $full_name, $campaign_id)) {
					$status_code = 1;
				} else {
					$status_code = 2;
				}
			}

			mysqli_query($conn, "UPDATE $tbl SET status='$status_code', last_send_time='$current_date' WHERE id='$trans_id' LIMIT 1");
		}

		mysqli_free_result($result);
	}
}

function GetCountryCodeByIp($ip)
{
	$country_code = "";
    $query = @unserialize(file_get_contents('http://ip-api.com/php/'.$ip));

	if ($query && $query['status'] == 'success') {
		$country_code = $query['countryCode']; 
	}

	return $country_code;
}

function CheckPayoutDayLimit($conn, $current_payout, $currency_symbol, $payout_limit) {
	$total = ConvertToUsd($current_payout, $currency_symbol);

	$sql = "SELECT * FROM (SELECT payout_usd, currency_symbol FROM withdraws WHERE (status<=1 || status=9) && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY)) UNION
						SELECT payout_usd, currency_symbol FROM deleted_withdraws WHERE (status<=1 || status=9) && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY))) b";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$total += ConvertToUsd($row['payout_usd'], $row['currency_symbol']);
		}
		mysqli_free_result($result);
	}
	
	return $payout_limit == 0 || $total < $payout_limit;
}

function CheckPayoutDayLimitPerUser($conn, $user_id, $current_payout, $currency_symbol, $payout_limit) {
	$total = ConvertToUsd($current_payout, $currency_symbol);
	$device_id = GetDeviceId($conn, $user_id);

	$sql = "SELECT * FROM (SELECT payout_usd, currency_symbol FROM withdraws WHERE (status<=1 || status=9) && user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY)) UNION
						SELECT payout_usd, currency_symbol FROM deleted_withdraws WHERE (status<=1 || status=9) && device_id='$device_id' && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY))) b";
	
	if ($result = mysqli_query($conn, $sql)) {
		while ($row=mysqli_fetch_array($result)) {
			$total += ConvertToUsd($row['payout_usd'], $row['currency_symbol']);
		}
		mysqli_free_result($result);
	}
	
	return $payout_limit == 0 || $total < $payout_limit;
}

function StatusPayoutFirstDayLimit($conn, $user_id, $device_id, $current_payout, $currency_symbol, $payout_limit) {
	$status = 0;
	$total = ConvertToUsd($current_payout, $currency_symbol);
	$is_first_payout = true;

	$sql = "SELECT * FROM (SELECT payout_usd, currency_symbol FROM withdraws WHERE (status<=1 || status=9) && user_id='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY)) UNION
						SELECT payout_usd, currency_symbol FROM deleted_withdraws WHERE (status<=1 || status=9) && device_id='$device_id' && (timestamp > DATE_SUB(now(), INTERVAL '1' DAY))) b";
	
	if ($result = mysqli_query($conn, $sql)) {
		while ($row=mysqli_fetch_array($result)) {
			$is_first_payout = false;
			$total += ConvertToUsd($row['payout_usd'], $row['currency_symbol']);
		}
		mysqli_free_result($result);
	}
	
	$limited = $payout_limit > 0 && $total >= $payout_limit;
	
	if ($limited) {
		$status = $is_first_payout ? 1 : 2;
	}

	return $status;
}

function ConvertToUsd($amount, $currency_symbol) {
	$new_amount = $amount;

	switch($currency_symbol) {
		case "\u20ac": // евро
			$new_amount = round($amount * 1.111111111111111, 2, PHP_ROUND_HALF_UP); 
			break;
		case "\u00A3": // фунт
			$new_amount = round($amount * 1.333333333333333, 2, PHP_ROUND_HALF_UP);
			break;
		case "\u20BD": // руб
			$new_amount = round($amount * 0.0105263157894737, 2, PHP_ROUND_HALF_UP);
			break;
		case "\u20A9": // воны
			$new_amount = round($amount * (1.0 / 1300), 2, PHP_ROUND_HALF_UP);
			break;
		case "\u00A5": // йены
			$new_amount = round($amount * 0.0066666666666667, 2, PHP_ROUND_HALF_UP);
	}
	
	return $new_amount;
}


function PayoutsCount($conn, $user_id, $payout_slot_id) {
	$tbl = "withdraws";
	$count = 0;

	$sql = "SELECT id FROM $tbl WHERE user_id='$user_id' && (status<2 || status>5) && payout_slot_id='$payout_slot_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count;
}

function NeedPayoutNotify($conn, $user_id) {
	$count = 0;
	$tbl = "withdraws";

	$sql = "SELECT id FROM $tbl WHERE user_id='$user_id' && status='1' && notified='0'";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		if ($count > 0) {
			while($row=mysqli_fetch_array($result)) {
				$trans_id = $row['id'];
				mysqli_query($conn, "UPDATE $tbl SET notified='1' WHERE id='$trans_id'");
			}
		}

		mysqli_free_result($result);
	}

	return $count > 0;
}

function InstagramBonusReceived($conn, $device_id) {
    $received = false;
    $tbl = "users";

    $sql = "SELECT inst_bonus_received FROM $tbl WHERE device_id='$device_id' LIMIT 1";
    
    if ($result = mysqli_query($conn, $sql)) {
        while($row=mysqli_fetch_array($result)) {
            $received = $row['inst_bonus_received'] == 1;
        }

        mysqli_free_result($result);
    }
    
    return $received;
}

function NeedInstagramBonusNotify($conn, $user_id) {
    $notify = false;
    $tbl = "users";

    $sql = "SELECT inst_bonus_notified FROM $tbl WHERE user_id='$user_id' LIMIT 1";
    
    if ($result = mysqli_query($conn, $sql)) {
        while($row=mysqli_fetch_array($result)) {
            $notify = $row['inst_bonus_notified'] == 0;

            if ($notify) {
                mysqli_query($conn, "UPDATE $tbl SET inst_bonus_notified='1' WHERE user_id='$user_id'");
            }
        }

        mysqli_free_result($result);
    }
    
    return $notify;
}

function SendFCM($device_tokens, $title, $desc) {
  $url = 'https://fcm.googleapis.com/fcm/send';

  // server key
  $apiKey = "AAAAaCXxkVE:APA91bEDwt6e2DF8HntN6mi_6rJjYMhwf3D1GfJYfr6JpgeLDi_2bU7S9IgxDcQX9-5rvNXwGEWA7Njm1YvQk_2kAV5umt_3Bo1dNjQ0iY-xWd1dESf_-sF6SI5WdbAEJZAB09hCYfGI";

  $headers = array (
    'Authorization:key=' . $apiKey,
    'Content-Type:application/json'
  );

  $notifData = [
    'title' => $title,
    'body' => $desc,
    //  "image": "url-to-image",//Optional
    //'click_action' => "activities.NotifHandlerActivity" //Action/Activity - Optional
  ];

  /*
  $dataPayload = ['to'=> 'My Name', 
  'points'=>80, 
  'other_data' => 'This is extra payload'
  ];
  */

  $apiBody = [
    'notification' => $notifData,
    //'data' => $dataPayload, //Optional
    //'time_to_live' => 600, // optional - In Seconds
    //'to' => '/topics/all'
    'registration_ids' => $device_tokens
  ];

  $ch = curl_init();
  curl_setopt ($ch, CURLOPT_URL, $url);
  curl_setopt ($ch, CURLOPT_POST, true);
  curl_setopt ($ch, CURLOPT_HTTPHEADER, $headers);
  curl_setopt ($ch, CURLOPT_RETURNTRANSFER, true);
  curl_setopt ($ch, CURLOPT_POSTFIELDS, json_encode($apiBody));

  $result = curl_exec($ch);

  curl_close($ch);
}

function ProcessNotificationOneDay($conn) {
	$device_tokens = array();
	$tbl = "users";

	$sql = "SELECT user_id, fcm_token FROM $tbl WHERE fcm_token != '' && 1day_notified = '0' && (timestamp < DATE_SUB(now(), INTERVAL 1 DAY)) LIMIT 10";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$user_id = $row['user_id'];
			$fcm_token = $row['fcm_token'];
			$device_tokens[] = $fcm_token;

			mysqli_query($conn, "UPDATE $tbl SET 1day_notified='1' WHERE user_id='$user_id'");
		}

		mysqli_free_result($result);
	}

	if (count($device_tokens) > 0) {
		SendFCM($device_tokens, "Reward Plus", "You only have a few coins left to earn before the next payout.");
	}
}

function ProcessNotificationOneWeek($conn) {
	$device_tokens = array();
	$tbl = "users";

	$sql = "SELECT user_id, fcm_token FROM $tbl WHERE fcm_token != '' && 7day_notified = '0' && (timestamp < DATE_SUB(now(), INTERVAL 7 DAY)) LIMIT 10";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$user_id = $row['user_id'];
			$fcm_token = $row['fcm_token'];

			if(AnyRewardReceived($conn, $user_id)) {
				$device_tokens[] = $fcm_token;

				mysqli_query($conn, "UPDATE $tbl SET 7day_notified='1' WHERE user_id='$user_id'");
			}
		}

		mysqli_free_result($result);
	}

	if (count($device_tokens) > 0) {
		SendFCM($device_tokens, "Reward Plus", "We have a lot of new games. Just check it out.");
	}
}

function GetNeocurrencyToken() {
	$url = 'https://redeem.yourdigitalreward.com/api/get-token';
	$client_id = "W022SJYV609ZTWOFRB0RHVNA";
	$client_secret = "93FJ3MJHK2A0XAFX2AZO8VALSXXKPMN4D8SWVE3Z96RY7WKV";
	$email = "cash@plus-games.com";
	$password = "UkEt%#YLhvq3";
	$body = array(
        "client_id" => $client_id,
        "client_secret" => $client_secret,
		"email" => $email,
		"password" => $password,
      );

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type' => 'application/json'
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);
    curl_close($ch);

	$decoded_text = html_entity_decode($result);
	$result_array = json_decode($decoded_text, true);
	$token = $result_array['success'][0]['access_token'];

	return $token;
}

function CreateNeocurrencyOrder($user_id, $payout_value, $brand_id, $token) {
	$url = 'https://redeem.yourdigitalreward.com/api/createordernow';
	$custom1 = "user_id:".$user_id;
	$brands = array(
        "id" => $brand_id,
        "denomination" => $payout_value,
		"quantity" => 1
      );
	$body = array(
        "custom1" => $custom1,
		"brands" => array($brands)
      );

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type' => 'application/json',
		'Authorization: Bearer '.$token
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);
	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

	$order_id = 0;
	$campaign_brand_id = 0;
	$code = "0";

	if ($httpcode == "200" && str_contains($result,'success')) {
		$decoded_text = html_entity_decode($result);
		$result_array = json_decode($decoded_text, true);
		$order_id = $result_array['success']['order_id'];
		$campaign_brand_id = $result_array['success']['data'][0]['campaign_brand_id'];
		$code = $result_array['success']['data'][0]['codes'][0];
	}

	$res_array = array("order_id" => $order_id, "campaign_brand_id" => $campaign_brand_id, "code" => $code);

	return $res_array;
}

function AddEmailNeocurrency($order_id, $campaign_brand_id, $email, $token) {
	$success = false;
	$url = 'https://redeem.yourdigitalreward.com/api/addemails';

	$emails = array(
        "email" => $email,
        "first_name" => "",
		"second_name" => ""
      );

	$body = array(
        "order_id" => $order_id,
		"campaign_brand_id" => $campaign_brand_id,
		"emails" => array($emails)
      );

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type' => 'application/json',
		'Authorization: Bearer '.$token
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);
	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

	$success = $httpcode == "200" && str_contains($result,'success');

	if (!$success) {
		$file = 'neocurrency_log.txt';
		$current = file_get_contents($file);
		$current .= $result ."\n";
		file_put_contents($file, $current);
	}

	return $success;
}

function SendEmailNeocurrency($order_id, $campaign_brand_id, $email, $token) {
	$url = 'https://redeem.yourdigitalreward.com/api/sendemails';
	$email_template_id = 971;
	
	$body = array(
        "order_id" => $order_id,
		"campaign_brand_id" => $campaign_brand_id,
		"email_template_id" => $email_template_id
      );

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type' => 'application/json',
		'Authorization: Bearer '.$token
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);
	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

	return $httpcode == "200";
}

function ClearSpecialChars($string) {
	return preg_replace('/[^ a-z.\d]/ui', '', $string);
}

function CheckAdTiming($conn, $user_id) {
	$cooldown_seconds = 30;
	$count = 0;

	$sql = "SELECT id FROM income_unityads WHERE device_uuid='$user_id' && (timestamp > DATE_SUB(now(), INTERVAL '$cooldown_seconds' SECOND))";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count == 0;
}

function GetPaypalToken() {
	$url = 'https://api-m.paypal.com/v1/oauth2/token';
	$client_id = "AQ-rLyXVEfUbnZzSJh9AjUi3V4-9uAJFI2LipcI2zc0qTkL2pU4Hpw9aSkRMg9yif1uJ2E-JWa3ObOzR";
	$client_secret = "EFBPH_FgRtQbA8EwY5L7KVFnJWtD6asAYu7R46-vT3wORCStzqsxKieI8-4G8KpgZ9O7BWRhtPvvUGYu";
	$pass = base64_encode($client_id.":".$client_secret);

	$body = array(
		"grant_type" => "client_credentials"
      );

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type' => 'application/x-www-form-urlencoded',
		'Authorization: Basic '.$pass
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_HTTPAUTH, CURLAUTH_ANY);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);
    curl_close($ch);

	$decoded_text = html_entity_decode($result);
	$result_array = json_decode($decoded_text, true);
	$token = $result_array['access_token'];

	return $token;
}

function SendEmailPaypal($email, $amount, $token, $currency_code) {
	$url = 'https://api-m.paypal.com/v1/payments/payouts';
	$amount_str = number_format($amount, 2, '.', '');

	$sender_batch_header = array(
		"recipient_type" => "EMAIL",
		"email_subject" => "Here is your Cashee Payout!",
		"email_message" => "Thanks for using Cashee. Please Rate us 5 Stars!",
      );

	$item = array(
        "amount" => array("value" => $amount_str, "currency" => $currency_code),
		"recipient_type" => "EMAIL",
		"receiver" => $email
      );

	$body = array(
        "sender_batch_header" => $sender_batch_header,
		"items" => array($item)
	);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type: application/json',
		'Authorization: Bearer '.$token,
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt ($ch, CURLOPT_POST, true);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

	/*
	if ($httpcode !== 201) {
		echo $result.'<br><br>';
	}
	*/

    curl_close($ch);

	return $httpcode === 201;
}

function SendKochavaEvent($event_name, $value, $gps_adid, $kochava_device_id, $ip, $unixtime, $app_version) {
    $app_guid = "kobucks-plus-an9iagr";
	$url = "http://control.kochava.com/track/json";

	$device_ids = array(
        "idfa" => "",
		"idfv" => "",
		"adid" => "",
		"android_id" => $gps_adid
      );

	$event_data = array(
        "id" => "123",
		"name" => "test",
		"sum" => $value
      );

	$gdpr = array(
        "gdpr_applies" => 1,
		"tc_string" => "some example string",
		"ad_user_data" => 1,
		"ad_personalization" => 1
      );

	$data = array(
		"usertime" => $unixtime,
        "app_version" => $app_version,
        "device_ver" => "test",
		"device_ids" => $device_ids,
		"device_ua" => "test",
		"event_name" => $event_name,
		"origination_ip" => $ip, 
        "currency" => "USD",
		"gdpr_privacy_consent" => $gdpr,
		"event_data" => $event_data
      );

	$body = array(
        "data" => $data,
		"action" => "event",
		"kochava_app_id" => $app_guid,
		"kochava_device_id" => $kochava_device_id
	);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type: application/json',
		//'Authorization: Bearer '.$token,
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_HTTPAUTH, CURLAUTH_ANY);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

    curl_close($ch);

	return $result;
}

function SendKochavaEventTest() {
    $app_guid = "komoney-win-ra2quvxty";
	$url = "http://control.kochava.com/track/json";

	$device_ids = array(
        "idfa" => "",
		"idfv" => "",
		"adid" => "123456789abcdefg0",
		"android_id" => "abcdefg123450"
      );

	$event_data = array(
        "id" => "123",
		"name" => "Skis",
		"sum" => 150
      );

	$gdpr = array(
        "gdpr_applies" => 1,
		"tc_string" => "some example string",
		"ad_user_data" => 1,
		"ad_personalization" => 1
      );

	$data = array(
		"usertime" => "1757409472",
        "app_version" => "33",
        "device_ver" => "nikolay_test",
		"device_ids" => $device_ids,
		"device_ua" => "nikolayandroiddevice",
		"event_name" => "Purchase_Test",
		"origination_ip" => "104.219.46.66", 
        "currency" => "USD",
		"gdpr_privacy_consent" => $gdpr,
		"event_data" => $event_data
      );

	$body = array(
        "data" => $data,
		"action" => "event",
		"kochava_app_id" => $app_guid,
		"kochava_device_id" => "KAnikolaytest123456"
	);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type: application/json',
		//'Authorization: Bearer '.$token,
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
	curl_setopt($ch, CURLOPT_HTTPAUTH, CURLAUTH_ANY);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
	curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_POSTFIELDS, http_build_query($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

    curl_close($ch);

	return $result;
}

function SendTremendous($email, $amount, $full_name, $campaign_id) {
    $api_key = "PROD_TD84nUuoe--mG8rFqRYtoEbcVsyNtzYLumkxpTbX6D6";
	$url = "https://api.tremendous.com/api/v2/orders";

	$body = array(
        "payment" => array("funding_source_id" => "BALANCE"),
		"reward" => array("value" => array("denomination" => $amount, "currency_code" => "USD"), "delivery" => array("method" => "EMAIL"), "recipient" => array("name" => $full_name, "email" => $email), "campaign_id" => $campaign_id)
	);

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'Content-Type: application/json',
		'Authorization: Bearer '.$api_key,
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt ($ch, CURLOPT_POST, true);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($body));
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

    curl_close($ch);

	return $httpcode == 200 && str_contains($result,'order_id');
}

function SendSingularEvent($event_name, $gps_adid, $ip) {
	$api_key = 'plus_games_91a55198';
	$bundle_id = 'com.plusgames.coinup';

	$params = [
		'a' => $api_key,
		'evt_ts' => time(),
		'n' => $event_name,
		'aifa' => $gps_adid,
		'ip' => $ip,
		'ua' => $_SERVER['HTTP_USER_AGENT'] ?? '',
		'p' => 'Android',
		'i' => $bundle_id
	];

	$url = 'https://s2s.singular.net/api/v1/evt?' . http_build_query($params);

	$ch = curl_init($url);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    $result = curl_exec($ch);
    curl_close($ch);

	return $result;
}

function GetUsernameAndCountry($conn, $user_id) {
	$data = array('first_name' => 'A', 'last_name' => 'A', 'country' => 'US');

	$sql = "SELECT country, first_name, last_name FROM users WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$data['first_name'] = $r['first_name'];
			$data['last_name'] = $r['last_name'];
			$data['country'] = $r['country'];
		}

		mysqli_free_result($result);
	}

	return $data;
}

function LeaderboardResetDate($conn, $app_version) {
	$date = '2026-03-23';

	$sql = "SELECT leaderboard_reset_date FROM settings WHERE app_version='$app_version' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$date = $r['leaderboard_reset_date'];
		}

		mysqli_free_result($result);
	}

	return $date;
}

function GetLeaderboardData($conn, $user_id, $tier, $min_balance) {
	$data = [];
	
	$leaderboard_balance = GetLeaderboardBalance($conn, $user_id);

	if ($leaderboard_balance < $min_balance) {
		return $data;
	}

	$sql = "WITH ranked AS (
	  SELECT
		user_id,
		coins,
		tier,
		ROW_NUMBER() OVER (ORDER BY coins DESC, user_id ASC) AS `rank`
	  FROM leaderboard
	  WHERE tier = '$tier' && blocked = 0
	),
	user_rank AS (
	  SELECT `rank` AS pos
	  FROM ranked 
	  WHERE user_id = '$user_id'
	),
	positions AS (
	  SELECT 
		pos,
		CASE WHEN pos <= 3 THEN pos ELSE pos - 2 END AS low_pos,
		CASE WHEN pos <= 3 THEN 8 ELSE pos + 2 END AS high_pos
	  FROM user_rank
	)
	SELECT 
	  r.user_id,
	  r.coins, 
	  r.tier,
	  r.`rank`
	FROM ranked r
	CROSS JOIN positions p
	WHERE r.`rank` <= 3 OR r.`rank` BETWEEN p.low_pos AND p.high_pos
	ORDER BY r.`rank`
	LIMIT 8;
	";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$is_owner = $r['user_id'] == $user_id;
			$user_data = GetUsernameAndCountry($conn, $r['user_id']);
			$revenue = (int)$r['coins'] / 10000;

			$arr = array('FirstName' => $user_data['first_name'], 'LastName' => $user_data['last_name'], 'CountryCode' => $user_data['country'], 'Rank' => $r['rank'], 'Coins' => $r['coins'], 'Revenue' => $revenue, 'IsOwner' => $is_owner);
			$data[] = $arr;
		}

		mysqli_free_result($result);
	}

	return $data;
}

function GetLeaderboardBalance($conn, $user_id) {
	$balance = 0;

	$sql = "SELECT coins FROM leaderboard WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$balance = $r[0];
		}

		mysqli_free_result($result);
	}

	return $balance;
}

function UpdateLeaderboard($conn, $user_id, $reward, $country_code) {
	$leaderboard_balance = GetLeaderboardBalance($conn, $user_id);

	if ($leaderboard_balance > 0) {
		$leaderboard_balance += $reward;
		mysqli_query($conn, "UPDATE leaderboard SET coins='$leaderboard_balance' WHERE user_id='$user_id' LIMIT 1");
	} else {
		$tier = GetTier($country_code);

		mysqli_query($conn, "INSERT INTO leaderboard (user_id, coins, tier) VALUES ('$user_id', '$reward', '$tier')");
	}
}

function BlockUserLeaderboard($conn, $user_id) {
	mysqli_query($conn, "UPDATE leaderboard SET blocked='1' WHERE user_id='$user_id' LIMIT 1");
}

function GetTier($country_code) {
	$tier = 3;

	$tier1 = array("US", "DE", "FR", "GB", "CA", "UA", "JP", "KR", "IT");
	$tier2 = array("SP", "SG", "NZ", "NL", "BE", "AT", "PL", "HU", "CZ");

	if (in_array($country_code, $tier1)) {
		$tier = 1;
	} else if (in_array($country_code, $tier2)) {
		$tier = 2;
	}

	return $tier;
}

function GetMissions($conn) {
	$data = [];

	$sql = "SELECT mission_id, currency, coins FROM missions";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$arr = array('mission_id' => $r['mission_id'], 'currency' => $r['currency'], 'coins' => $r['coins']);
			$data[] = $arr;
		}

		mysqli_free_result($result);
	}

	return $data;
}

function GetCompletedMissions($conn, $user_id, $limit) {
	$data = [];

	$sql = "SELECT mission_id, reward, notified FROM missions_completed WHERE user_id='$user_id' LIMIT $limit";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$arr = array('mission_id' => $r['mission_id'], 'reward' => $r['reward'], 'notified' => $r['notified']);
			$data[] = $arr;
		}

		mysqli_free_result($result);
	}

	return $data;
}

function AdjoeAppInstalls($conn, $user_id) {
	$count = 0;

	$sql = "SELECT COUNT(id) FROM adjoe_installs WHERE user_id='$user_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$count = (int)$row[0];
		}

		mysqli_free_result($result);
	}
	
	return $count;
}

function AnyAdjoeReward($conn, $user_id) {
	$success = false;

	$sql = "SELECT id FROM income_adjoe WHERE user_id='$user_id' && is_bonus_purchase=0 LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$success = true;
		}
		
		mysqli_free_result($result);
	}
	
	return $success;
}

function GetMissionStates($conn, $user_id, $is_adjoe_offerwall) {
	$data = [];
	$all = GetMissions($conn);
	$count_all = count($all);
	$completed = GetCompletedMissions($conn, $user_id, $count_all);
	$count_completed = count($completed);
	$need_update = false;

	foreach ($all as $a) {
		$mission_id = $a['mission_id'];
		$is_completed = false;
		$notified = false;
		$reward = $a['currency'];
		$coins = $a['coins'];
		$progress = [];

		foreach ($completed as $c) {
			if ($c['mission_id'] == $mission_id) {
				$is_completed = true;
				$notified = $c['notified'] == 1;
				$coins = $a['coins'];

				if (!$notified) {
					$need_update = true;
				}
				break;
			}
		}

		if ($mission_id == 4) {
			$total_apps = 3;
			$total_coins = 5000;
			$installs =  $is_adjoe_offerwall ? AdjoeAppInstalls($conn, $user_id) : PrimeAppInstalls($conn, $user_id);
			$earn_coins = $is_adjoe_offerwall ? AdjoeCoins($conn, $user_id) : PrimeCoins($conn, $user_id);

			$user_installs = $is_completed ? $total_apps : $installs;
			$user_coins = $is_completed ? $total_coins : $earn_coins;

			$progress[] = array("Type" => 0, "Current" => min($user_installs, $total_apps), "Total" => $total_apps);
			$progress[] = array("Type" => 1, "Current" => min($user_coins, $total_coins), "Total" => $total_coins);
		}
		
		$arr = array('Id' => $mission_id, 'Reward' => $reward, 'Coins' => $coins, 'Completed' => $is_completed, 'Progresses' => $progress, 'Notified' => $notified);
		$data[] = $arr;
	}

	if ($need_update) {
		mysqli_query($conn, "UPDATE missions_completed SET notified='1' WHERE user_id='$user_id' && notified=0 LIMIT $count_completed");
	}

	return $data;
}

function CheckMissions($conn, $user_id, $is_adjoe_offerwall) {
	$all = GetMissions($conn);
	$count = count($all);
	$completed = GetCompletedMissions($conn, $user_id, $count);

	foreach ($all as $a) {
		$mission_id = $a['mission_id'];
		$is_completed = false;
		$coins = $a['coins'];

		foreach ($completed as $c) {
			if ($c['mission_id'] == $mission_id) {
				$is_completed = true;
				break;
			}
		}

		if ($is_completed) {
			continue;
		}

		switch ($mission_id) {
			case 1:
				$total_apps = 1;
				$adjoe_installs = $is_adjoe_offerwall ? AdjoeAppInstalls($conn, $user_id) : PrimeAppInstalls($conn, $user_id);
				$is_completed = $adjoe_installs >= $total_apps;
				break;
			case 2:
				$is_completed = $is_adjoe_offerwall ? AnyAdjoeReward($conn, $user_id) : AnyPrimeReward($conn, $user_id);
				break;
			case 3:
				$is_completed = AnyMychipsReward($conn, $user_id);
				break;
			case 4:
				$total_apps = 3;
				$total_coins = 5000;
				$adjoe_installs = $is_adjoe_offerwall ? AdjoeAppInstalls($conn, $user_id) : PrimeAppInstalls($conn, $user_id);
				$adjoe_coins = $is_adjoe_offerwall ? AdjoeCoins($conn, $user_id) : PrimeCoins($conn, $user_id);
				$is_completed = $adjoe_installs >= $total_apps && $adjoe_coins >= $total_coins;
				break;
		}

		if ($is_completed) {
			mysqli_query($conn, "INSERT INTO missions_completed (user_id, mission_id, reward) VALUES ('$user_id', '$mission_id', '$coins')");
		}
	}
}

function CanShowMissions($registration_date, $country, $campaign, $was_registered) {
	$start_date = new DateTime('2026-04-27 06:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');
	$tier = GetTier($country);

	return strtotime($registration_date) > strtotime($start_date_str) && $tier == 1 && IsUnityCampaign($campaign) && !$was_registered;
}

function NeedMychipsReportChecking($timestamp) {
	$start_date = new DateTime('2026-05-22 00:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');

	return strtotime($timestamp) > strtotime($start_date_str);
}

function AnyMychipsReward($conn, $user_id) {
	$success = false;

	$sql = "SELECT id FROM income_mychips WHERE user_id='$user_id' && revenue>0 LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$success = true;
		}
		
		mysqli_free_result($result);
	}
	
	return $success;
}

function MychipsReportDiff($conn, $user_id) {
	$total = 1.0;

	$sql = "SELECT
  (
    SELECT SUM(revenue)
    FROM income_mychips
    WHERE user_id = '$user_id'
  )
  /
  NULLIF(
    (
      SELECT SUM(reward) / 3000
      FROM income_mychips
      WHERE user_id = '$user_id'
    ),
    0
  ) AS mychips_report_diff";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = (float)$row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function NeedPrimeReportChecking($timestamp) {
	return true;

	$start_date = new DateTime('2026-06-01 00:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');

	return strtotime($timestamp) > strtotime($start_date_str);
}

function AnyPrimeReward($conn, $user_id) {
	$success = false;

	$sql = "SELECT id FROM income_prime WHERE user_id='$user_id' && reward>0 LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($r=mysqli_fetch_array($result)) {
			$success = true;
		}
		
		mysqli_free_result($result);
	}
	
	return $success;
}

function PrimeReportDiff($conn, $user_id) {
	$total = 1.0;

	$sql = "SELECT
  (
    SELECT SUM(revenue)
    FROM income_prime
    WHERE user_id = '$user_id'
  )
  /
  NULLIF(
    (
      SELECT SUM(reward) / 3000
      FROM income_prime
      WHERE user_id = '$user_id'
    ),
    0
  ) AS prime_report_diff";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = (float)$row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function UserWasRegistered($conn, $device_id) {
	$count = 0;
	
	$sql = "SELECT id FROM deleted_withdraws WHERE device_id='$device_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count > 0;
}

function UserWasRegisteredIP($conn, $device_id, $ip) {
	$exist = false;
	
	$sql = "SELECT
    (
        EXISTS (
            SELECT 1
            FROM deleted_users
            WHERE ip='$ip'
        )
        AND EXISTS (
            SELECT 1
            FROM deleted_withdraws
            WHERE device_id='$device_id'
        )
    ) AS both_exists";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $exist = $row[0] == 1;
		}

		mysqli_free_result($result);
	}
	
	return $exist;
}

function AdjoeForEarnButton($country_code, $timestamp) {
	return true;

	$countries = array("US", "FR", "DE", "CA", "GB");
	$start_date = new DateTime('2026-05-21 00:00:00');
	$start_date_str = $start_date->format('Y-m-d H:i:s');

	return !in_array($country_code, $countries) || strtotime($timestamp) < strtotime($start_date_str);
}

function PrimeCoins($conn, $user_id) {
	$total = 0;

	$sql = "SELECT SUM(reward) FROM income_prime WHERE user_id='$user_id'";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
		    $total = $row[0];
		}
		mysqli_free_result($result);
	}
	
	return $total;
}

function PrimeAppInstalls($conn, $user_id) {
	$count = 0;

	$sql = "SELECT COUNT(id) FROM income_prime WHERE user_id='$user_id' && revenue>0 LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		while($row=mysqli_fetch_array($result)) {
			$count = (int)$row[0];
		}

		mysqli_free_result($result);
	}
	
	return $count;
}

function IsVPNUsage($ip)
{
	$value = 0.0;
	$url = "https://pluv78nykixtkoj.getipintel.net/check.php?ip=".$ip."&contact=pyotr@plus-games.com";

	$response = @file_get_contents($url);

	if ($response !== false) {
		$response = trim($response);
		if (is_numeric($response)) {
			$value = (float)$response;
		}
	}

	return $value > 0.93;
}

function IsCountrySwitch($country_code) {
	$countries = array("DZ", "EG", "BD", "VT");

	return in_array($country_code, $countries);
}

function PrimeTransactionIsExist($conn, $user_id, $transaction_id) {
	$count = 0;
	
	$sql = "SELECT id FROM income_prime WHERE user_id='$user_id' && tx_id='$transaction_id' LIMIT 1";
	
	if ($result = mysqli_query($conn, $sql)) {
		$count = mysqli_num_rows($result);

		mysqli_free_result($result);
	}
	
	return $count > 0;
}

function GetAdjoeReport($date) {
	$token = '5b6a6984c5b117a433a2c8f9c6ba8570';
    $sdk_hash = "8b50f8826e52379f6d9819db7d7b2498";
	$url = 'https://prod.adjoe.zone/v3/ssp-api/user-ad-data-report/sdk/'.$sdk_hash.'?date='.$date;

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
		'X-API-KEY: '.$token,
    ));
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

    curl_close($ch);

	return $result;;
}

function GetSingularReport() {
	$url = 'https://docs.google.com/spreadsheets/d/1hRYyMYWdrySWynCpDbMBZ-BqlyEEK_FrW2QJq50ThOI/export?format=csv&gid=1642518850';

    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, 1);
    $result = curl_exec($ch);

	$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

    curl_close($ch);

	return $result;;
}

function DailyStreakStates($conn, $user_id, $start_time, $states) {
	$current_timestamp = time();
	$time_diff = $current_timestamp - $start_time;
	$days_passed = (int)($time_diff / (24 * 60 * 60));
	$days_passed = min(6, $days_passed);
	$need_update = false;
	$has_reward_today = HasOfferwallRewardToday($conn, $user_id);
	$i = 0;
	$reset_timer = false;

	do {
		$state = $states[$i];
		$get_reward = $i == $days_passed && $has_reward_today;

		if ($state == 0) {
			$need_update = true;

			if ($get_reward) {
				$states[$i] = 1;
			} else {
				$states = [0, 0, 0, 0, 0, 0, 0];
				$reset_timer = true;
				break;
			}
		} else if ($state < 0) {
			$need_update = $reset_timer = true;
			$states = $get_reward ? [1, 0, 0, 0, 0, 0, 0] : [0, 0, 0, 0, 0, 0, 0];
			break;
		}

		$i++;
	} while ($i <= $days_passed);

	if ($need_update) {
		$states_str = implode(";",$states);

		if ($reset_timer) {
			$reset_time = new DateTime();
			$reset_time_str = $reset_time->format('Y-m-d H:i:s');

			mysqli_query($conn, "UPDATE users SET daily_streak='$states_str', daily_streak_reset_time='$reset_time_str' WHERE user_id='$user_id' LIMIT 1");
		} else {
			mysqli_query($conn, "UPDATE users SET daily_streak='$states_str' WHERE user_id='$user_id' LIMIT 1");
		}
	}

	return array('states' => $states, 'reset_timer' => $reset_timer);
}

function DailyStreakRewards($conn, $user_id, $states, $rewards, $get_reward) {
	$i = 0;
	$c = 0;
	$reward = 0;
	$reward_index = 0;
	$reset_timer = false;
	$need_update = false;

	for ($i = 0; $i < 7; $i++) {
		$state = $states[$i];

		if ($state == 1) {
			$reward = $rewards[$c];
			$reward_index = $c;

			if ($get_reward) {
				$states[$i] = 2;
				$current_date = new DateTime();
				$current_date_str = $current_date->format('Y-m-d');

				mysqli_query($conn, "INSERT INTO daily_streak_rewards (user_id, reward, timestamp) VALUES ('$user_id', $reward, '$current_date_str')");

				//if ($c == 7) {
				//	mysqli_query($conn, "INSERT INTO daily_streak_rewards (user_id, reward, timestamp) VALUES ('$user_id', $streak_reward, '$current_date_str')");
				//}

				$need_update = true;
			}
		}

		$c = $state > 0 ? $c + 1 : 0;
	}

	if ($need_update) {
		$states_str = implode(";",$states);
		mysqli_query($conn, "UPDATE users SET daily_streak='$states_str' WHERE user_id='$user_id' LIMIT 1");
	}

	return array('states' => $states, 'daily_reward_index' => $reward_index);
}
?>