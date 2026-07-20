/**
 * Police Dashboard JavaScript
 * Client-side functionality
 */

// Update file name display
document.addEventListener('DOMContentLoaded', function() {
    const fileInput = document.getElementById('excelFile');
    const fileNameSpan = document.getElementById('fileName');
    
    if (fileInput) {
        fileInput.addEventListener('change', function(e) {
            if (this.files && this.files[0]) {
                fileNameSpan.textContent = this.files[0].name;
            }
        });
    }
    
    // Auto-hide messages after 5 seconds
    const messages = document.querySelectorAll('.error-message, .success-message');
    messages.forEach(msg => {
        setTimeout(() => {
            msg.style.display = 'none';
        }, 5000);
    });
});