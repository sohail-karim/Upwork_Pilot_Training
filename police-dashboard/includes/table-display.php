<?php
/**
 * Table Display Template
 * Displays the processed complaint summary in table format
 * Format: Total | Completed (FIR, Rappat) | Filled | Pending
 */

if (!isset($processedData)) {
    return;
}

$totals = calculateTotals($processedData);
?>

<table class="summary-table">
    <thead>
        <tr>
            <th rowspan="3" class="sr-no">Sr No</th>
            <th rowspan="3" class="ps-name">Office/PS Name</th>
            
            <?php foreach (CRIME_CATEGORIES as $category): ?>
                <th colspan="5" class="category-header"><?php echo htmlspecialchars($category); ?></th>
            <?php endforeach; ?>
        </tr>
        <tr>
            <?php foreach (CRIME_CATEGORIES as $category): ?>
                <th class="total-header">Total</th>
                <th colspan="2" class="completed-header">Completed</th>
                <th class="filled-header">Filled</th>
                <th class="pending-header">Pending</th>
            <?php endforeach; ?>
        </tr>
        <tr>
            <?php foreach (CRIME_CATEGORIES as $category): ?>
                <th></th>
                <th class="subcategory-header">FIR</th>
                <th class="subcategory-header">Rappat</th>
                <th></th>
                <th></th>
            <?php endforeach; ?>
        </tr>
    </thead>
    <tbody>
        <?php
        $srNo = 1;
        foreach (POLICE_STATIONS as $psName):
            $row = $processedData[$psName];
            ?>
            <tr>
                <td class="sr-no"><?php echo $srNo; ?></td>
                <td class="ps-name"><?php echo htmlspecialchars($row['Police Station']); ?></td>
                
                <?php
                foreach (CRIME_CATEGORIES as $category):
                    $catData = $row[$category];
                    ?>
                    <td class="total-cell"><?php echo $catData['Total']; ?></td>
                    <td class="fir-cell"><?php echo $catData['FIR']; ?></td>
                    <td class="rappat-cell"><?php echo $catData['Rappat']; ?></td>
                    <td class="filled-cell"><?php echo $catData['Filled']; ?></td>
                    <td class="pending-cell"><?php echo $catData['Pending']; ?></td>
                    <?php
                endforeach;
                ?>
            </tr>
            <?php
            $srNo++;
        endforeach;
        ?>
        
        <!-- Total Row -->
        <tr class="total-row">
            <td colspan="2">Total</td>
            
            <?php
            foreach (CRIME_CATEGORIES as $category):
                $catTotals = $totals[$category];
                ?>
                <td class="total-cell"><?php echo $catTotals['Total']; ?></td>
                <td class="fir-cell"><?php echo $catTotals['FIR']; ?></td>
                <td class="rappat-cell"><?php echo $catTotals['Rappat']; ?></td>
                <td class="filled-cell"><?php echo $catTotals['Filled']; ?></td>
                <td class="pending-cell"><?php echo $catTotals['Pending']; ?></td>
                <?php
            endforeach;
            ?>
        </tr>
    </tbody>
</table>