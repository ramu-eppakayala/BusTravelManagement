// =========== Client-side Validation ===========

(function () {
    'use strict';

    // Bootstrap 5 form validation
    document.addEventListener('DOMContentLoaded', function () {
        var forms = document.querySelectorAll('.needs-validation');
        forms.forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });

        initLiveValidation();
        initPasswordStrength();
        initPhoneValidation();
    });

    function initLiveValidation() {
        // Real-time validation on blur
        document.querySelectorAll('input[required], select[required], textarea[required]').forEach(function (el) {
            el.addEventListener('blur', function () {
                validateField(this);
            });
            el.addEventListener('input', function () {
                if (this.classList.contains('is-invalid')) {
                    validateField(this);
                }
            });
        });

        // Email validation
        document.querySelectorAll('input[type="email"]').forEach(function (el) {
            el.addEventListener('blur', function () {
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (this.value && !emailRegex.test(this.value)) {
                    showFieldError(this, 'Please enter a valid email address');
                } else {
                    clearFieldError(this);
                }
            });
        });

        // Confirm password
        var passwordField = document.getElementById('Password');
        var confirmField = document.getElementById('ConfirmPassword');
        if (passwordField && confirmField) {
            confirmField.addEventListener('input', function () {
                if (this.value && this.value !== passwordField.value) {
                    showFieldError(this, 'Passwords do not match');
                } else {
                    clearFieldError(this);
                }
            });
        }
    }

    function validateField(el) {
        if (!el.value || el.value.trim() === '') {
            showFieldError(el, 'This field is required');
            return false;
        }

        if (el.type === 'email') {
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(el.value)) {
                showFieldError(el, 'Invalid email format');
                return false;
            }
        }

        if (el.type === 'number' || el.type === 'tel') {
            if (isNaN(el.value)) {
                showFieldError(el, 'Please enter a valid number');
                return false;
            }
        }

        clearFieldError(el);
        return true;
    }

    function showFieldError(el, message) {
        el.classList.add('is-invalid');
        el.classList.remove('is-valid');
        var feedback = el.nextElementSibling;
        if (feedback && feedback.classList.contains('invalid-feedback')) {
            feedback.textContent = message;
        }
    }

    function clearFieldError(el) {
        el.classList.remove('is-invalid');
        el.classList.add('is-valid');
    }

    function initPasswordStrength() {
        var passwordInput = document.getElementById('Password');
        if (!passwordInput) return;

        var meter = document.createElement('div');
        meter.className = 'password-strength mt-1';
        meter.innerHTML = '<div class="progress" style="height:4px;"><div class="progress-bar" role="progressbar" style="width:0%"></div></div>' +
            '<small class="text-muted" id="password-hint">Use uppercase, lowercase, number & special character</small>';
        passwordInput.parentElement.appendChild(meter);

        passwordInput.addEventListener('input', function () {
            var val = this.value;
            var strength = 0;
            if (val.length >= 8) strength++;
            if (val.length >= 12) strength++;
            if (/[a-z]/.test(val)) strength++;
            if (/[A-Z]/.test(val)) strength++;
            if (/\d/.test(val)) strength++;
            if (/[^a-zA-Z\d]/.test(val)) strength++;

            var bar = meter.querySelector('.progress-bar');
            var percent = Math.min(strength * 16.67, 100);
            bar.style.width = percent + '%';

            if (percent < 33) { bar.className = 'progress-bar bg-danger'; }
            else if (percent < 66) { bar.className = 'progress-bar bg-warning'; }
            else { bar.className = 'progress-bar bg-success'; }
        });
    }

    function initPhoneValidation() {
        document.querySelectorAll('input[type="tel"], input[name$="Phone"], input[name$="PhoneNumber"]').forEach(function (el) {
            el.addEventListener('input', function () {
                this.value = this.value.replace(/[^0-9+\-\s()]/g, '');
            });
        });
    }

    // Age validation
    window.validateAge = function (input) {
        var age = parseInt(input.value);
        if (isNaN(age) || age < 1 || age > 120) {
            showFieldError(input, 'Age must be between 1 and 120');
            return false;
        }
        clearFieldError(input);
        return true;
    };

    // Date validation
    window.validateDate = function (input) {
        var date = new Date(input.value);
        var today = new Date();
        today.setHours(0, 0, 0, 0);
        if (isNaN(date.getTime())) {
            showFieldError(input, 'Invalid date');
            return false;
        }
        clearFieldError(input);
        return true;
    };

})();
