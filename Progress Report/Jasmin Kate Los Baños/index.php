<?php
	error_reporting(E_ALL);
    ini_set('display_errors', 1);

	include '../connection.php';
	include '../functions.php';

    $userSession = new session();

	session_start();

    // already logged in
    if (isset($_SESSION['userId'])) {
        header("Location: dashboard.php");
        exit();
    }

    // handles user login
    if (isset($_POST['login'])) {
        $username = $_POST['username'];
        $password = $_POST['password'];

        $isUser = $conn->prepare("SELECT * FROM users WHERE username=?");
        $isUser->bind_param("s", $username);
        $isUser->execute();
        $user = $isUser->get_result();

        if ($user->num_rows > 0) {
            $userInfo = $user->fetch_assoc();
            $userName = $userInfo['lastName'] . ", " . $userInfo['firstName']; 
            $userHashedPass = $userInfo['password'];

            if (password_verify($password, $userHashedPass)) {
                $_SESSION['userId'] = $userInfo['id'];
                $userSession->userLogin($userName);
                header("Location: dashboard.php");
                exit();
            } else {
                setAlert('alert-danger', "Wrong password!");
            }
        } else {
            setAlert('alert-danger', "User not found!");
        }
    }

    function setAlert($alertClass, $message) {
        $_SESSION['alertClass'] = $alertClass;
        $_SESSION['alert'] = $message;
        header("Location: " . $_SERVER['PHP_SELF']);
        exit(); // Make sure to exit after the redirect
    }
?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>User Login</title>
    <link rel="stylesheet" type="text/css" href="../bootstrap/css/bootstrap.css">
    <script type="text/javascript" src="../bootstrap/js/bootstrap.bundle.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <style>
        body {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .header {
            text-align: center;
            margin-bottom: 1.5rem;
        }
        .header h1 {
            font-size: 2.5rem;
            color: #2c3e50;
        }
        .header p {
            color: #6c757d;
        }
        .login-icon {
            font-size: 4rem;
            color: #007bff;
            text-align: center;
        }
        .btn {
            width: 100%;
            padding: 0.5rem;
        }
    </style>
</head>
<body>
    <div class="col-7 col-md-3">
        <!-- invalid login alert -->
<?php
        if (isset($_SESSION['alert'])) {
            $alert = $_SESSION['alert'];
            $alertClass = $_SESSION['alertClass'];
            echo "<div class='alert $alertClass alert-dismissible fade show' role='alert'>
                    $alert
                    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
                  </div>";
            // Clear the session variables after displaying the alert
            unset($_SESSION['alert']);
            unset($_SESSION['alertClass']);
        }
?>
        <!-- Header -->
        <div class="header">
            <h1>PICK ME UP</h1>
        </div>
        <!-- Login Form -->
        <div class="text-center">
            <span class="login-icon"><i class="bi bi-person"></i></span>
            <p class="mb-4">LOGIN AS USER</p>
            <form method="POST">
                <div class="form-floating">
                    <input style="margin-bottom: 1rem; " class="form-control" type="text" name="username" id="username" required>
                    <label for="username">Username</label>
                </div>
                <div class="form-floating">
                    <input style="margin-bottom: 1rem; " class="form-control" type="password" name="password" id="password" required>
                    <label for="password">Password</label>
                </div>
                <div>
                    <button class="btn btn-primary" type="submit" name="login">LOGIN</button>
                    <a href="register.php" class="btn btn-link text-center">Create an account.</a>
                </div>
            </form>
        </div>
    </div>
</body>
</html>
