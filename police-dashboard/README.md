# Police Complaint Summary Dashboard

A PHP-based web application for processing and summarizing police complaint data from Excel/CSV files.

## Features

- ✅ Upload Excel (.xlsx, .xls) or CSV files
- ✅ Automatic data processing and categorization
- ✅ Summary report generation by police station and crime category
- ✅ Export processed data back to CSV
- ✅ Responsive web interface
- ✅ Session-based data management
- ✅ No external dependencies (pure PHP)

## Requirements

- PHP 7.2 or higher
- No external libraries required

## Installation

1. Place the `police-dashboard` folder in your web server's root directory
2. Create the following directories (PHP will create them automatically):
   ```bash
   mkdir -p police-dashboard/uploads
   mkdir -p police-dashboard/temp
   mkdir -p police-dashboard/logs
   ```

3. Access the dashboard at: `http://localhost/police-dashboard/`

## Usage

1. Open the dashboard in your browser
2. Click "Choose Excel File" to select a complaint data file
3. Click "Process File" to generate the summary
4. View the results in the table
5. Click "Export to Excel" to download the summary as CSV
6. Click "Clear" to reset and process a new file

## Project Structure

```
police-dashboard/
├── index.php                 # Main entry point
├── config/
│   └── config.php           # Configuration settings
├── includes/
│   ├── functions.php        # Helper functions
│   └── table-display.php    # Table template
├── assets/
│   ├── css/
│   │   └── style.css        # Stylesheet
│   ├── js/
│   │   └── script.js        # Client-side scripts
│   └── download.php         # CSV export handler
├── uploads/                 # Uploaded files
├── temp/                    # Temporary files
├── logs/                    # Log files
└── README.md               # This file
```

## Configuration

Edit `config/config.php` to customize:

### Police Stations

The dashboard is configured with these police stations:

1. City Lodhran
2. Sadar Lodhran
3. Qureshi Wala
4. Galaywal
5. City Kahror Pacca
6. Saddar Kahror Pacca
7. Dhanot
8. City Dunya Pur
9. Sadar Dunya Pur
10. Jalla Arain

To modify, edit the `POLICE_STATIONS` constant:

```php
define('POLICE_STATIONS', [
    'Your Station 1',
    'Your Station 2',
    // ...
]);
```

### Crime Categories

Default categories:
- Crime Against Property
- Crime Against Person
- Local and Special Law
- Miscellaneous

### Maximum File Size

Default: 5MB. To change:

```php
define('MAX_FILE_SIZE', 10 * 1024 * 1024); // 10MB
```

## Input File Format

Your Excel/CSV file should contain at least these columns:

| Column Name | Description | Example |
|---|---|---|
| Police Station | Name of the police station | "City Lodhran" |
| Category | Type of complaint | "Crime Against Property" |
| Complaint Status | Current status | "Completed", "Pending", "FIR", etc. |

## Output Format

The dashboard generates a summary table with:

- **Sr No**: Serial number
- **Office/PS Name**: Police station name
- **Total Complaints**: Count of all complaints
- **Crime Categories**: For each category:
  - Total
  - Completed
  - FIR
  - Rappat
  - Filled
  - Pending

## Data Processing

### Station Matching

The system automatically matches input station names using fuzzy matching. For example:
- "lodhran city" matches "City Lodhran"
- "CITY LODHRAN" matches "City Lodhran"
- "Qureshi" matches "Qureshi Wala"

### Category Matching

Categories are matched by keywords:
- Contains "property" → Crime Against Property
- Contains "person" → Crime Against Person
- Contains "local" or "special" → Local and Special Law
- Default → Miscellaneous

### Status Processing

Multiple statuses can be counted per complaint. A complaint with "Completed and FIR" will increment both counts.

## Troubleshooting

### File Upload Fails
- Check file size (max 5MB)
- Ensure file format is .xlsx, .xls, or .csv
- Check PHP upload settings in php.ini

### Data Not Processing
- Verify column names in your Excel file match expected names
- Check that police station names are recognizable
- Review logs in `logs/error.log`

### Export Not Working
- Ensure session is active
- Check write permissions in the uploads directory
- Verify PHP can write files

## File Descriptions

### index.php
Main entry point. Handles file upload, session management, and displays the UI.

### config/config.php
Centralized configuration. Define police stations, categories, file limits, and paths.

### includes/functions.php
Core business logic for file processing, data validation, and summary generation.

### includes/table-display.php
HTML template for displaying the summary table.

### assets/css/style.css
Responsive styling for the dashboard.

### assets/js/script.js
Client-side functionality (file name display, message auto-hide).

### assets/download.php
Generates and downloads CSV export.

## Browser Support

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Mobile browsers

## License

Free to use and modify.

## Support

For issues or modifications, edit the respective PHP files directly. Each file is well-commented for easy understanding.
