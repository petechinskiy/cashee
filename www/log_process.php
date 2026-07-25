<?php
$log_file = 'access.log';
$domain = 'https://rewardeetrack.com';

$handle = fopen($log_file, 'r');

if ($handle) {
    while (($line = fgets($handle)) !== false) {

        if (strpos($line, 'adjoe_install.php') === false) {
            continue;
        }

        $start_pos = strpos($line, '"GET ');
        if ($start_pos === false) {
            continue;
        }
        $start_pos += 5;

        $end_pos = strpos($line, ' HTTP/', $start_pos);
        if ($end_pos === false) {
            continue;
        }

        $url = substr($line, $start_pos, $end_pos - $start_pos);
        $full_url = $domain . $url;

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $full_url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
        curl_setopt($ch, CURLOPT_TIMEOUT, 15);
        curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 5);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 2);

        $response = curl_exec($ch);
        $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $curl_error = curl_error($ch);

        curl_close($ch);

        if ($response !== false && $http_code >= 200 && $http_code < 300) {
            echo htmlspecialchars($response) . "<hr>\n";
        } else {
            echo 'Ошибка запроса: ' . htmlspecialchars($full_url) .
                 ' | HTTP: ' . (int)$http_code .
                 ' | cURL: ' . htmlspecialchars($curl_error) . "<br>\n";
        }
    }

    fclose($handle);
} else {
    echo 'Не удалось открыть файл лога';
}
?>