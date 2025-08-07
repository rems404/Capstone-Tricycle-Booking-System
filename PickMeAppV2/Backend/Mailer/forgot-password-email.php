<?php
    require 'src/PHPMailer.php';
    require 'src/SMTP.php';
    require 'src/Exception.php';

    use PHPMailer\PHPMailer\PHPMailer;
    use PHPMailer\PHPMailer\Exception;

    function sendEmail(String $userid, String $email, String $name, $conn) {
        $upperName = strtoupper($name);
        $mail = new PHPMailer(true);

        try {
            // server settings
            $mail->isSMTP();
            $mail->Host = 'smtp.gmail.com';
            $mail->SMTPAuth = true;
            $mail->Username   = 'remydventuraaa@gmail.com';
            $mail->Password   = 'bfeeuiizrytfzezv';
            $mail->SMTPSecure = 'tls';
            $mail->Port       = 587; //465

            //recipients
            $mail->setFrom('remydventuraaa@gmail.com', 'Pick-Me App Team');
            $mail->addAddress($email);

            // reset token
            $resetToken = random_int(100000, 999999);
            $tokenSaved = saveToken($resetToken, $userid, $conn);

            if ($tokenSaved) {
                // email content
                $mail->isHTML(true);
                $mail->Subject = "Password Reset Request";
                $mail->Body = <<<EOT
                                <div style="width: 100%; margin: auto; padding: 20px; font-family: Arial, sans-serif; background-color: #ffffff; border: 1px solid #ccc;">
                                    <h4 style="font-size: 1.8rem; color: forestgreen; margin: 0;">Pick-Me App</h4>

                                    <hr style="border: none; border-top: 1px solid #ddd; margin: 20px 0;" />

                                    <div style="font-size: 16px; color: #333;">
                                        <h2 style="font-size: 1.5rem;">Hello, {$upperName},</h2>
                                        <p>We received a request to reset the password for your <strong>Pick-Me App</strong> account. Use the verification code below to reset your password in the app:</p>
                                        <p style="font-size: 1rem"><strong>{$resetToken}</strong></p>
                                        <p><i>Keep this code private. Never share it with anyone, including Pick-Me App support staff. It will expire in 15 minutes.</i></p>
                                        <p>If you didn’t request a password reset, you can safely ignore this message.</p>

                                        <p>Thank you,<br><strong>Pick-Me App Team</strong></p>
                                    </div>

                                    <hr style="border: none; border-top: 1px solid #ddd; margin: 30px 0;" />

                                    <div style="text-align: center; font-size: 14px; color: #777;">
                                        <p style="margin: 0;"><strong>Pick-Me App</strong></p>
                                        <p style="margin: 0;">CSU-G's Tricycle Booking Platform</p>
                                    </div>
                                </div>
EOT;
                $mail->send();
                echo "email sent";
            } else {
                echo "failed";
            }

        } catch (Exception $e) {
            echo "Failed: " . $e->getMessage();
        }
    }

    function saveToken(int $token, String $userid, $conn) {
        date_default_timezone_set('Asia/Manila');
        $expiry = date("Y-m-d H:i:s", strtotime("+1 hour"));

        $saveQuery = $conn->prepare("INSERT INTO resetlinks (userid, token, expiry) VALUES (?, ?, ?)");
        $saveQuery->bind_param("sis", $userid, $token, $expiry);
        $result = $saveQuery->execute();

        if (!$result) {
            echo "Execute failed: " . $saveQuery->error;
        }

        return $result;
    }

    function getEmailById(String $userid, $conn) {
        $info = array();

        $getEmail = $conn->prepare("SELECT email, firstname FROM users WHERE userid = ?");
        $getEmail->bind_param("s", $userid);
        $getEmail->execute();
        $emailResult = $getEmail->get_result();

        if ($emailResult->num_rows > 0) {
            $row = $emailResult->fetch_assoc();
            $info[] = $row["email"];
            $info[] = $row["firstname"];
        } 

        return $info;
    }

    $conn = new mysqli("localhost", "root", "", "pickmeapp");
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    if (isset($_POST['userid'])) {
        $userid = $_POST['userid'];

        $info = getEmailById($userid, $conn);

        if ($info != null) {
            $email = $info[0];
            $name = $info[1];

            sendEmail($userid, $email, $name, $conn);
        } else {
            echo "failed";
        }
    }
?>