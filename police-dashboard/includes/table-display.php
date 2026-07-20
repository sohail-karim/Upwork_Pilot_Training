<?php
/**
 * Table Display Template
 * Displays the processed complaint summary in table format
 */

if (!isset($processedData)) {
    return;
}

$totals = calculateTotals($processedData);
?>

<table class="summary-table">
    <thead>
        <tr>
            <th rowspan="2" class="sr-no">Sr No</th>
            <th rowspan="2" class="ps-name">Office/PS Name</th>
            <th rowspan="2" class="total-col">Total Complaints</th>
            <th colspan="6" class="category-header">Crime Against Property</th>
            <th colspan="6" class="category-header">Crime Against Person</th>
            <th colspan="6" class="category-header">Local and Special Law</th>
            <th colspan="6" class="category-header">Miscellaneous</th>
        </tr>
        <tr>
            <?php 
            for ($i = 0; $i < 4; $i++) {
                ?>
                <th class="subcategory-header">Total</th>
                <th class="subcategory-header">Completed</th>
                <th class="subcategory-header">FIR</th>
                <th class="subcategory-header">Rappat</th>
                <th class="subcategory-header">Filled</th>
                <th class="subcategory-header">Pending</th>
                <?php
            }
            ?>
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
                <td class="total-col"><?php echo $row['Total Complaints']; ?></td>
                
                <?php
                foreach (CRIME_CATEGORIES as $category):
                    $catData = $row[$category];
                    ?>
                    <td><?php echo $catData['Total']; ?></td>
                    <td><?php echo $catData['Completed']; ?></td>
                    <td><?php echo $catData['FIR']; ?></td>
                    <td><?php echo $catData['Rappat']; ?></td>
                    <td><?php echo $catData['Filled']; ?></td>
                    <td><?php echo $catData['Pending']; ?></td>
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
            <td class="total-col"><?php echo $totals['Total Complaints']; ?></td>
            
            <?php
            foreach (CRIME_CATEGORIES as $category):
                $catTotals = $totals[$category];
                ?>
                <td><?php echo $catTotals['Total']; ?></td>
                <td><?php echo $catTotals['Completed']; ?></td>
                <td><?php echo $catTotals['FIR']; ?></td>
                <td><?php echo $catTotals['Rappat']; ?></td>
                <td><?php echo $catTotals['Filled']; ?></td>
                <td><?php echo $catTotals['Pending']; ?></td>
                <?php
            endforeach;
            ?>
        </tr>
    </tbody>
</table>