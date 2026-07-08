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


var dobElement = document.getElementById("DateOfBirth");

if (dobElement) {
    dobElement.addEventListener("change", function () {

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

        document.getElementById("Age").value = age;
    });
}
