<?php
/**
 * Configuration File
 * Centralized configuration for the Police Dashboard
 */

// Police Stations List
define('POLICE_STATIONS', [
    'City Lodhran',
    'Sadar Lodhran',
    'Qureshi Wala',
    'Galaywal',
    'City Kahror Pacca',
    'Saddar Kahror Pacca',
    'Dhanot',
    'City Dunya Pur',
    'Sadar Dunya Pur',
    'Jalla Arain'
]);

// Crime Categories
define('CRIME_CATEGORIES', [
    'Crime Against Property',
    'Crime Against Person',
    'Local and Special Law',
    'Miscellaneous'
]);

// Complaint Status Types
define('STATUS_TYPES', [
    'Completed',
    'Filled',
    'Pending',
    'FIR',
    'Rappat'
]);

// Upload Configuration
define('MAX_FILE_SIZE', 5 * 1024 * 1024); // 5MB
define('UPLOAD_DIR', __DIR__ . '/../uploads/');
define('TEMP_DIR', __DIR__ . '/../temp/');

// Ensure directories exist
if (!is_dir(UPLOAD_DIR)) {
    mkdir(UPLOAD_DIR, 0755, true);
}
if (!is_dir(TEMP_DIR)) {
    mkdir(TEMP_DIR, 0755, true);
}

// Error handling
ini_set('display_errors', 0);
error_reporting(E_ALL);
ini_set('log_errors', 1);
ini_set('error_log', __DIR__ . '/../logs/error.log');