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
        var emailRegex = /^(?=.{5,50}$)[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var passwordRegex = /^(?=.*[A-Z])(?=.*[0-9])(?=.*[@$!%*?&])[A-Za-z0-9@$!%*?&]{5,20}$/;

        // ================= FULL NAME
        if (fullName === "") return showError("Full Name is required");
        if (fullName.length < 3) return showError("Full Name must be at least 3 characters");
        if (fullName.length > 50) return showError("Full Name cannot exceed 50 characters");
        if (!fullNameRegex.test(fullName)) return showError("Each word must start with a capital letter and contain only alphabets");

        // ================= EMAIL
        if (email === "") return showError("Email is required");
        if (email.length < 5) return showError("Email must be at least 5 characters");
        if (email.length > 50) return showError("Email cannot exceed 50 characters");
        if (!emailRegex.test(email)) return showError("Invalid email format");

        // ================= PASSWORD
        if (password === "") return showError("Password is required");
        if (password.length < 5) return showError("Password must be at least 5 characters");
        if (password.length > 20) return showError("Password cannot exceed 20 characters");
        if (!passwordRegex.test(password)) return showError("Password must contain at least one uppercase letter, one number and one special character");

        // ================= CONFIRM PASSWORD
        if (confirmPassword === "") return showError("Confirm Password is required");
        if (password !== confirmPassword)
            return showError("Password and Confirm Password do not match");

    $(this).off('submit').submit();
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


document.addEventListener("click", function (e) {
    var iconWrapper = e.target.closest(".toggle-password");
    if (!iconWrapper) return;

    var id = iconWrapper.getAttribute("data-target");
    var input = document.getElementById(id);
    var icon = iconWrapper.querySelector("i");

    if (!input) return;

    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        input.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
});


document.getElementById("DateOfBirth").addEventListener("change", function () {

    if (this.value === "") {
        document.getElementById("Age").value = "";
        return;
    }

    var dob = new Date(this.value);
    var today = new Date();

    var age = today.getFullYear() - dob.getFullYear();

    var monthDifference = today.getMonth() - dob.getMonth();

    if (monthDifference < 0 ||
        (monthDifference === 0 && today.getDate() < dob.getDate())) {
        age--;
    }

    document.getElementById("Age").value = age ;
});
