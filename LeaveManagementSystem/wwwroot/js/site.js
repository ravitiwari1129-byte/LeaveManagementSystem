// ========================================
// SITE.JS - Global Functions
// ========================================

// Auto-hide alerts after 5 seconds
$(document).ready(function () {
    // Hide success/error alerts after 5 seconds
    setTimeout(function () {
        $('.alert').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000);

    // Add active class to sidebar links
    $('.sidebar a').on('click', function () {
        $('.sidebar a').removeClass('active');
        $(this).addClass('active');
    });
});

// Global function to show messages
function showMessage(message, type) {
    var className = type === 'success' ? 'alert-success' : 'alert-error';
    var alertHtml = '<div class="alert ' + className + '" style="position: fixed; top: 80px; right: 20px; z-index: 9999; min-width: 250px; padding: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.2);">' + message + '</div>';
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    setTimeout(function () {
        var alert = document.querySelector('.alert');
        if (alert) alert.remove();
    }, 3000);
}

// Confirm dialog
function confirmAction(message) {
    return confirm(message);
}