// ========================================
// SITE.JS - Global Functions
// ========================================

// Auto-hide alerts after 5 seconds
$(document).ready(function () {

    $('form').submit(function (e) {
        if (!window.location.pathname.toLowerCase().includes("signup")) {
            return true;
        }

        e.preventDefault();

        var fullName = $("#FullName").val().trim();
        var email = $("#Email").val().trim();
        var password = $("#Password").val().trim();
        var confirmPassword = $("#ConfirmPassword").val().trim();
        var fullNameRegex = /^[A-Z][a-z]+(?: [A-Z][a-z]+)*$/;
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var passwordRegex = /^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{7,20}$/;

        // ================= FULL NAME
        if (fullName === "") return showError("Full Name is required");
        if (fullName.length < 3) return showError("Full Name must be at least 3 characters");
        if (fullName.length > 50) return showError("Full Name cannot exceed 50 characters");
        if (!fullNameRegex.test(fullName)) return showError("Full Name must start with capital letters");

        // ================= EMAIL
        if (email === "") return showError("Email is required");
        if (email.length < 6) return showError("Email must be at least 6 characters");
        if (email.length > 50) return showError("Email cannot exceed 50 characters");
        if (!emailRegex.test(email)) return showError("Invalid email format");

        // ================= PASSWORD
        if (password === "") return showError("Password is required");
        if (password.length < 7) return showError("Password must be at least 7 characters");
        if (password.length > 20) return showError("Password cannot exceed 20 characters");
        if (!passwordRegex.test(password))
            return showError("Password must start with capital letter, contain number & special char");

        // ================= CONFIRM PASSWORD
        if (confirmPassword === "") return showError("Confirm Password is required");
        if (password !== confirmPassword)
            return showError("Password and Confirm Password do not match");

    this.off('submit').submit();
});

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

function showError(msg) {
    showMessage(msg, "error");
    return false;
}

// Global function to show messages
function showMessage(message, type) {
    $('.alert').remove();
    var className = type === 'success' ? 'alert-success' : 'alert-error';
    var alertHtml = '<div class="alert ' + className + '">' + message + '</div>';
    var container = document.querySelector('.signup-container') || document.body;
    container.insertAdjacentHTML('afterbegin', alertHtml);
    setTimeout(function () {
        $('.alert').fadeOut('slow', function () {
            $(this).remove();
        });
    }, 3000);
}

// Confirm dialog
function confirmAction(message) {
    return confirm(message);
}


