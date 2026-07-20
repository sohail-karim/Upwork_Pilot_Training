<?php
/**
 * Helper Functions
 * Core functions for file processing and data manipulation
 */

// Simple CSV reader (no external dependencies required)

/**
 * Handle file upload and processing
 * @param array $file - $_FILES array
 * @return array - Status and processed data
 */
function handleFileUpload($file) {
    // Validate file
    if (!$file || $file['error'] !== UPLOAD_ERR_OK) {
        return [
            'status' => 'error',
            'message' => 'File upload failed. Please try again.'
        ];
    }

    // Check file size
    if ($file['size'] > MAX_FILE_SIZE) {
        return [
            'status' => 'error',
            'message' => 'File size exceeds maximum limit of 5MB.'
        ];
    }

    // Check file extension
    $allowedExt = ['xlsx', 'xls', 'csv'];
    $fileExt = strtolower(pathinfo($file['name'], PATHINFO_EXTENSION));
    
    if (!in_array($fileExt, $allowedExt)) {
        return [
            'status' => 'error',
            'message' => 'Invalid file format. Only .xlsx, .xls, and .csv are allowed.'
        ];
    }

    try {
        // Read the file based on type
        if ($fileExt === 'csv') {
            $data = readCSVFile($file['tmp_name']);
        } else {
            // For Excel files, use simple CSV conversion or read as CSV
            $data = readCSVFile($file['tmp_name']);
        }
        
        // Process data
        $processedData = processPoliceComplaints($data);
        
        return [
            'status' => 'success',
            'message' => 'File processed successfully!',
            'data' => $processedData,
            'fileName' => $file['name']
        ];
    } catch (Exception $e) {
        return [
            'status' => 'error',
            'message' => 'Error processing file: ' . $e->getMessage()
        ];
    }
}

/**
 * Read CSV file
 * @param string $filePath - Path to file
 * @return array - File data
 */
function readCSVFile($filePath) {
    $data = [];
    
    if (($handle = fopen($filePath, 'r')) !== false) {
        while (($row = fgetcsv($handle)) !== false) {
            $data[] = $row;
        }
        fclose($handle);
    }
    
    return $data;
}

/**
 * Process police complaints data
 * @param array $rawData - Raw data from spreadsheet
 * @return array - Processed summary data
 */
function processPoliceComplaints($rawData) {
    $summary = initializePoliceStationSummary();
    
    if (empty($rawData)) return $summary;
    
    // Skip header row
    $headers = array_shift($rawData);
    
    // Create header index map
    $headerMap = [];
    foreach ($headers as $key => $header) {
        $headerMap[strtolower(trim($header))] = $key;
    }
    
    foreach ($rawData as $row) {
        if (empty(array_filter($row))) continue; // Skip empty rows
        
        // Get values from row
        $psKey = array_search('police station', $headerMap);
        $categoryKey = array_search('category', $headerMap);
        $statusKey = array_search('complaint status', $headerMap);
        
        $psName = $psKey !== false && isset($row[$psKey]) ? trim($row[$psKey]) : '';
        $category = $categoryKey !== false && isset($row[$categoryKey]) ? trim($row[$categoryKey]) : 'Miscellaneous';
        $status = $statusKey !== false && isset($row[$statusKey]) ? trim($row[$statusKey]) : '';
        
        // Match police station
        $matchedPS = matchPoliceStation($psName);
        if (!$matchedPS) continue;
        
        // Match category
        $categoryName = matchCategory($category);
        
        // Update summary
        $summary[$matchedPS]['Total Complaints']++;
        $summary[$matchedPS][$categoryName]['Total']++;
        
        // Update status
        if (stripos($status, 'Completed') !== false) {
            $summary[$matchedPS][$categoryName]['Completed']++;
        }
        if (stripos($status, 'Filled') !== false) {
            $summary[$matchedPS][$categoryName]['Filled']++;
        }
        if (stripos($status, 'Pending') !== false) {
            $summary[$matchedPS][$categoryName]['Pending']++;
        }
        if (stripos($status, 'FIR') !== false) {
            $summary[$matchedPS][$categoryName]['FIR']++;
        }
        if (stripos($status, 'Rappat') !== false) {
            $summary[$matchedPS][$categoryName]['Rappat']++;
        }
    }
    
    return $summary;
}

/**
 * Initialize police station summary structure
 * @return array - Initialized summary array
 */
function initializePoliceStationSummary() {
    $summary = [];
    
    foreach (POLICE_STATIONS as $ps) {
        $summary[$ps] = [
            'Police Station' => $ps,
            'Total Complaints' => 0,
            'Crime Against Property' => [
                'Total' => 0,
                'Completed' => 0,
                'FIR' => 0,
                'Rappat' => 0,
                'Filled' => 0,
                'Pending' => 0
            ],
            'Crime Against Person' => [
                'Total' => 0,
                'Completed' => 0,
                'FIR' => 0,
                'Rappat' => 0,
                'Filled' => 0,
                'Pending' => 0
            ],
            'Local and Special Law' => [
                'Total' => 0,
                'Completed' => 0,
                'FIR' => 0,
                'Rappat' => 0,
                'Filled' => 0,
                'Pending' => 0
            ],
            'Miscellaneous' => [
                'Total' => 0,
                'Completed' => 0,
                'FIR' => 0,
                'Rappat' => 0,
                'Filled' => 0,
                'Pending' => 0
            ]
        ];
    }
    
    return $summary;
}

/**
 * Match police station name
 * @param string $inputPS - Input police station name
 * @return string|null - Matched police station or null
 */
function matchPoliceStation($inputPS) {
    if (empty($inputPS)) return null;
    
    $inputPS = strtolower($inputPS);
    
    foreach (POLICE_STATIONS as $ps) {
        if (stripos($inputPS, strtolower($ps)) !== false || 
            stripos(strtolower($ps), $inputPS) !== false) {
            return $ps;
        }
    }
    
    return null;
}

/**
 * Match crime category
 * @param string $inputCategory - Input category
 * @return string - Matched category
 */
function matchCategory($inputCategory) {
    if (empty($inputCategory)) return 'Miscellaneous';
    
    $inputCategory = strtolower($inputCategory);
    
    if (strpos($inputCategory, 'property') !== false) {
        return 'Crime Against Property';
    }
    if (strpos($inputCategory, 'person') !== false) {
        return 'Crime Against Person';
    }
    if (strpos($inputCategory, 'local') !== false || strpos($inputCategory, 'special') !== false) {
        return 'Local and Special Law';
    }
    
    return 'Miscellaneous';
}

/**
 * Calculate totals
 * @param array $summary - Summary array
 * @return array - Totals array
 */
function calculateTotals($summary) {
    $totals = [
        'Total Complaints' => 0
    ];
    
    foreach (CRIME_CATEGORIES as $category) {
        $totals[$category] = [
            'Total' => 0,
            'Completed' => 0,
            'FIR' => 0,
            'Rappat' => 0,
            'Filled' => 0,
            'Pending' => 0
        ];
    }
    
    foreach ($summary as $ps) {
        $totals['Total Complaints'] += $ps['Total Complaints'];
        
        foreach (CRIME_CATEGORIES as $category) {
            foreach (STATUS_TYPES as $status) {
                $totals[$category][$status] += $ps[$category][$status] ?? 0;
            }
        }
    }
    
    return $totals;
}