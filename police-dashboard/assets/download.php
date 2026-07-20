<?php
/**
 * Download CSV Export
 * Exports processed data to CSV format
 * Format: Total | Completed (FIR, Rappat) | Filled | Pending
 */

session_start();

require_once __DIR__ . '/../config/config.php';
require_once __DIR__ . '/../includes/functions.php';

// Check if data exists
if (!isset($_SESSION['processedData'])) {
    die('No data available for export.');
}

$processedData = $_SESSION['processedData'];
$totals = calculateTotals($processedData);

// Create CSV file
header('Content-Type: text/csv; charset=utf-8');
header('Content-Disposition: attachment; filename="Police_Complaint_Summary_' . date('Y-m-d_H-i-s') . '.csv"');

$output = fopen('php://output', 'w');

// Add BOM for Excel UTF-8 compatibility
fputcsv($output, array('\xEF\xBB\xBF'), '');

// Write main headers (Category names)
$mainHeaders = ['Sr No', 'Office/PS Name'];
foreach (CRIME_CATEGORIES as $category) {
    $mainHeaders[] = $category;
    $mainHeaders[] = '';
    $mainHeaders[] = '';
    $mainHeaders[] = '';
    $mainHeaders[] = '';
}
fputcsv($output, $mainHeaders);

// Write second level headers
$secondHeaders = ['', ''];
foreach (CRIME_CATEGORIES as $category) {
    $secondHeaders[] = 'Total';
    $secondHeaders[] = 'Completed';
    $secondHeaders[] = '';
    $secondHeaders[] = 'Filled';
    $secondHeaders[] = 'Pending';
}
fputcsv($output, $secondHeaders);

// Write third level headers (FIR, Rappat under Completed)
$thirdHeaders = ['', ''];
foreach (CRIME_CATEGORIES as $category) {
    $thirdHeaders[] = '';
    $thirdHeaders[] = 'FIR';
    $thirdHeaders[] = 'Rappat';
    $thirdHeaders[] = '';
    $thirdHeaders[] = '';
}
fputcsv($output, $thirdHeaders);

// Write data rows
$srNo = 1;
foreach (POLICE_STATIONS as $psName) {
    $ps = $processedData[$psName];
    
    $row = [
        $srNo,
        $ps['Police Station']
    ];
    
    foreach (CRIME_CATEGORIES as $category) {
        $catData = $ps[$category];
        $row[] = $catData['Total'];
        $row[] = $catData['FIR'];
        $row[] = $catData['Rappat'];
        $row[] = $catData['Filled'];
        $row[] = $catData['Pending'];
    }
    
    fputcsv($output, $row);
    $srNo++;
}

// Write totals row
$totalRow = ['', 'Total'];
foreach (CRIME_CATEGORIES as $category) {
    $catTotals = $totals[$category];
    $totalRow[] = $catTotals['Total'];
    $totalRow[] = $catTotals['FIR'];
    $totalRow[] = $catTotals['Rappat'];
    $totalRow[] = $catTotals['Filled'];
    $totalRow[] = $catTotals['Pending'];
}
fputcsv($output, $totalRow);

fclose($output);
exit();