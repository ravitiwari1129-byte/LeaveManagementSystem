// ========================================
// SITE.JS - Global Functions
// ========================================

// Auto-hide alerts after 5 seconds
$(document).ready(function () {

    $('form').submit(function (e) {
        if (window.location.pathname.toLowerCase().includes("signup")) {

            var fullName = $("#FullName").val().trim();
            var fullNameRegex = /^[A-Z][a-z]+(?: [A-Z][a-z]+)*$/;
            if (fullName === "") {
                showMessage("Full Name is required", "error");
                e.preventDefault();
                return false;
            }
            if (fullName.length < 3) {
                showMessage("Full Name must contain at least 3 characters", "error");
                e.preventDefault();
                return false;
            }

            if (fullName.length > 50) {
                showMessage("Full Name cannot exceed 50 characters", "error");
                e.preventDefault();
                return false;
            }
            if (!fullNameRegex.test(fullName)) {
                showMessage("Full Name must contain only letters and every word must start with a capital letter");
                e.preventDefault();
                return false;
            }


            var email = $("#Email").val().trim();
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (email === "") {
                showMessage("Email is required", "error");
                e.preventDefault();
                return false;
            }
            if (email.length < 6) {
                showMessage("Email must contain at least 6 characters", "error");
                e.preventDefault();
                return false;
            }
            if (email.length > 30) {
                showMessage("Email cannot exceed 30 characters", "error");
                e.preventDefault();
                return false;
            }
            if (!emailRegex.test(email)) {
                showMessage(
                    "Please enter a valid email address","error");
                e.preventDefault();
                return false;
            }


            var password = $("#Password").val().trim();
            var passwordRegex = /^(?=.*[0-9])(?=.*[@$!%*?&])[A-Z][A-Za-z0-9@$!%*?&]*$/;
            if (password === "") {
                showMessage("Password is required", "error");
                e.preventDefault();
                return false;
            }
            if (password.length < 7) {
                showMessage("Password must contain at least 7 characters","error");
                e.preventDefault();
                return false;
            }
            if (password.length > 20) {
                showMessage("Password cannot exceed 20 characters","error");
                e.preventDefault();
                return false;
            }
            if (!passwordRegex.test(password)) {
                showMessage("Password must start with a capital letter and contain at least one number and one special character.","error");
                e.preventDefault();
                return false;
            }


            var confirmPassword = $("#ConfirmPassword").val().trim();
            if (confirmPassword === "") {
                showMessage("Confirm Password is required", "error");
                e.preventDefault();
                return false;
            }
            if (confirmPassword.length < 7) {
                showMessage("Confirm Password must contain at least 7 characters", "error");
                e.preventDefault();
                return false;
            }
            if (confirmPassword.length > 20) {
                showMessage("Confirm Password cannot exceed 20 characters", "error");
                e.preventDefault();
                return false;
            }
            if (!passwordRegex.test(confirmPassword)) {
                showMessage("Confirm Password must start with a capital letter and contain at least one number and one special character.","error");
                e.preventDefault();
                return false;
            }
            if (password !== confirmPassword) {
                showMessage("Password and Confirm Password do not match", "error");
                e.preventDefault();
                return false;
            }

        }
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


