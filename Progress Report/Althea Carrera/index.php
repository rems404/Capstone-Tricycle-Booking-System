<?php
	// NOTE: Add a function for creating an account for main admin because by default later there is no admin for the system yet. 

	error_reporting(E_ALL);
    ini_set('display_errors', 1);

	include '../connection.php';
	include '../functions.php';

	$adminSession = new session();

	session_start();

	// already logged in
	if (isset($_SESSION['adminId'])) {
		header("Location: dashboard.php");
		exit();
	}

	// handles login for admin
	if (isset($_POST['login'])) {
		$adminName = $_POST['name'];
		$password = $_POST['password'];

		$isAdmin = $conn->prepare("SELECT * FROM admin WHERE adminName=?");
		$isAdmin->bind_param("s", $adminName);
		$isAdmin->execute();
		$admin = $isAdmin->get_result();

		if ($admin->num_rows > 0) {
			$adminInfo = $admin->fetch_assoc();
			$hashedPassword = $adminInfo['password'];

            if ($password === $hashedPassword) {
                $_SESSION['adminId'] = $adminInfo['id'];
                $adminSession->adminLogin($adminName);
                header("Location: dashboard.php");
                exit();
            } else {
                $alert = "Incorrect password!";
            }
		} else {
			$alert = "Invalid login!";
		}
	}
?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Admin Login</title>
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
        if (isset($alert)) {
            echo "<div class='alert alert-danger text-center'>$alert</div>";
        }
?>
        <!-- Header -->
        <div class="header">
            <h1>PICK ME UP</h1>
        </div>
        <!-- Login Form -->
        <div class="text-center">
            <span class="login-icon"><i class="bi bi-person"></i></span>
            <p class="mb-4">LOGIN AS ADMIN</p>
            <form method="POST">
                <div class="form-floating">
                    <input style="margin-bottom: 1rem; " class="form-control" type="text" name="name" id="name" required>
                    <label for="studId">Name</label>
                </div>
                <div class="form-floating">
                    <input style="margin-bottom: 1rem; " class="form-control" type="password" name="password" id="password" required>
                    <label for="password">Password</label>
                </div>
                <button class="btn btn-primary" type="submit" name="login">LOGIN</button>
            </form>
        </div>
    </div>
</body>
</html>
