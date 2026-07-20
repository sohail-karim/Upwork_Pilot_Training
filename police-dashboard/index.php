<?php
/**
 * Police Complaint Summary Dashboard
 * Main Entry Point
 */

session_start();

// Include configuration and helper functions
require_once 'config/config.php';
require_once 'includes/functions.php';

// Handle file upload
$uploadStatus = null;
$uploadMessage = null;

if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_FILES['excelFile'])) {
    $result = handleFileUpload($_FILES['excelFile']);
    $uploadStatus = $result['status'];
    $uploadMessage = $result['message'];
    
    if ($result['status'] === 'success') {
        $_SESSION['processedData'] = $result['data'];
        $_SESSION['fileName'] = $result['fileName'];
    }
}

// Handle clear request
if (isset($_GET['clear'])) {
    unset($_SESSION['processedData']);
    unset($_SESSION['fileName']);
    header('Location: index.php');
    exit();
}

$processedData = $_SESSION['processedData'] ?? null;
$fileName = $_SESSION['fileName'] ?? null;
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Police Complaint Summary Report</title>
    <link rel="stylesheet" href="assets/css/style.css">
</head>
<body>
    <div class="container">
        <h1>🚔 Police Complaint Summary Report</h1>

        <?php if ($uploadStatus === 'error'): ?>
            <div class="error-message">
                <?php echo htmlspecialchars($uploadMessage); ?>
                <button class="close-btn" onclick="this.parentElement.style.display='none';">&times;</button>
            </div>
        <?php elseif ($uploadStatus === 'success'): ?>
            <div class="success-message">
                <?php echo htmlspecialchars($uploadMessage); ?>
                <button class="close-btn" onclick="this.parentElement.style.display='none';">&times;</button>
            </div>
        <?php endif; ?>

        <form method="POST" enctype="multipart/form-data" class="upload-form">
            <div class="upload-section">
                <div class="file-input-wrapper">
                    <input type="file" id="excelFile" name="excelFile" accept=".xlsx,.xls,.csv" required />
                    <label for="excelFile" class="file-input-label">📁 Choose Excel File</label>
                </div>
                <span class="file-name" id="fileName">No file selected</span>
                <button type="submit" class="btn btn-primary">⚙️ Process File</button>
                
                <?php if ($processedData): ?>
                    <a href="assets/download.php" class="btn btn-export">💾 Export to Excel</a>
                    <a href="?clear=true" class="btn btn-danger" onclick="return confirm('Clear all data?');">🗑️ Clear</a>
                <?php endif; ?>
            </div>
        </form>

        <?php if ($processedData): ?>
            <div class="table-wrapper">
                <?php include 'includes/table-display.php'; ?>
            </div>
        <?php else: ?>
            <div class="info-box">
                <p>Upload an Excel file to generate the complaint summary report.</p>
                <p class="info-details">Supported formats: .xlsx, .xls, .csv</p>
            </div>
        <?php endif; ?>
    </div>

    <script src="assets/js/script.js"></script>
</body>
</html>