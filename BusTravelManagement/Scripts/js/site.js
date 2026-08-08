// =========== BusTravel Management - Site Scripts ===========

(function () {
    'use strict';

    // Toast notification system
    window.showToast = function (type, title, message) {
        var container = document.getElementById('toast-container');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        var toast = document.createElement('div');
        toast.className = 'toast-custom ' + type;
        toast.innerHTML = '<button type="button" class="toast-close" onclick="this.parentElement.remove()">&times;</button>' +
            '<div class="toast-title">' + escapeHtml(title) + '</div>' +
            '<div class="toast-message">' + escapeHtml(message) + '</div>';

        container.appendChild(toast);
        setTimeout(function () {
            toast.style.opacity = '0';
            toast.style.transition = 'opacity 0.3s';
            setTimeout(function () { toast.remove(); }, 300);
        }, 5000);
    };

    // TempData toast messages
    document.addEventListener('DOMContentLoaded', function () {
        var tempDataContainer = document.getElementById('tempdata-messages');
        if (tempDataContainer) {
            var messages = JSON.parse(tempDataContainer.getAttribute('data-messages') || '[]');
            messages.forEach(function (msg) {
                showToast(msg.type, msg.title, msg.message);
            });
        }
    });

    // City autocomplete
    window.initCityAutocomplete = function (inputId, hiddenId) {
        var input = document.getElementById(inputId);
        var hidden = document.getElementById(hiddenId);
        if (!input || !hidden) return;

        var dropdown = document.createElement('div');
        dropdown.className = 'autocomplete-dropdown';
        dropdown.style.cssText = 'position:absolute;z-index:1000;background:white;border:1px solid #e2e8f0;border-radius:8px;max-height:250px;overflow-y:auto;width:100%;display:none;box-shadow:0 4px 6px rgba(0,0,0,0.1);';
        input.parentElement.style.position = 'relative';
        input.parentElement.appendChild(dropdown);

        var debounceTimer;

        input.addEventListener('input', function () {
            clearTimeout(debounceTimer);
            var term = this.value.trim();

            if (term.length < 2) {
                dropdown.style.display = 'none';
                if (hidden) hidden.value = '';
                return;
            }

            debounceTimer = setTimeout(function () {
                fetch('/Search/SearchCities?term=' + encodeURIComponent(term))
                    .then(function (r) { return r.json(); })
                    .then(function (data) {
                        dropdown.innerHTML = '';
                        if (data.length === 0) {
                            dropdown.style.display = 'none';
                            return;
                        }
                        data.forEach(function (city) {
                            var item = document.createElement('div');
                            item.className = 'autocomplete-item';
                            item.textContent = city.text || city.displayName;
                            item.style.cssText = 'padding:10px 12px;cursor:pointer;border-bottom:1px solid #f1f5f9;font-size:0.9rem;';
                            item.addEventListener('mouseenter', function () { this.style.background = '#f8fafc'; });
                            item.addEventListener('mouseleave', function () { this.style.background = 'white'; });
                            item.addEventListener('click', function () {
                                input.value = city.text || city.displayName;
                                hidden.value = city.id;
                                dropdown.style.display = 'none';
                            });
                            dropdown.appendChild(item);
                        });
                        dropdown.style.display = 'block';
                    });
            }, 300);
        });

        input.addEventListener('blur', function () {
            setTimeout(function () { dropdown.style.display = 'none'; }, 200);
        });

        input.addEventListener('focus', function () {
            if (dropdown.children.length > 0) dropdown.style.display = 'block';
        });
    };

    // Utility: escape HTML
    function escapeHtml(text) {
        var div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Utility: format currency
    window.formatCurrency = function (amount) {
        return '\u20B9' + parseFloat(amount).toFixed(2);
    };

    // Utility: format date
    window.formatDate = function (dateStr) {
        var d = new Date(dateStr);
        return d.toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' });
    };

    // Utility: format time
    window.formatTime = function (timeStr) {
        var parts = timeStr.split(':');
        var h = parseInt(parts[0]);
        var m = parts[1];
        var ampm = h >= 12 ? 'PM' : 'AM';
        h = h % 12 || 12;
        return h + ':' + m + ' ' + ampm;
    };

    // Disable double-submit on forms
    document.addEventListener('submit', function (e) {
        var btn = e.target.querySelector('button[type="submit"]');
        if (btn && btn.dataset.submitted) {
            e.preventDefault();
            return;
        }
        if (btn) {
            btn.dataset.submitted = 'true';
            btn.disabled = true;
            setTimeout(function () { btn.disabled = false; delete btn.dataset.submitted; }, 5000);
        }
    });

})();
