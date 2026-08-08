// =========== Search Page Scripts ===========

(function () {
    'use strict';

    // Initialize city autocomplete on search page
    document.addEventListener('DOMContentLoaded', function () {
        if (typeof initCityAutocomplete === 'function') {
            initCityAutocomplete('source-city', 'SourceCityId');
            initCityAutocomplete('dest-city', 'DestinationCityId');
        }

        initDatePickers();
        initFilterToggle();
        initSortOptions();
    });

    function initDatePickers() {
        var dateInput = document.getElementById('journey-date');
        if (dateInput) {
            var tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            if (!dateInput.value) {
                dateInput.value = tomorrow.toISOString().split('T')[0];
            }
            dateInput.min = new Date().toISOString().split('T')[0];
        }
    }

    function initFilterToggle() {
        var toggleBtn = document.getElementById('toggle-filters');
        var filterPanel = document.getElementById('filter-panel');
        if (toggleBtn && filterPanel) {
            toggleBtn.addEventListener('click', function () {
                filterPanel.classList.toggle('d-none');
                toggleBtn.innerHTML = filterPanel.classList.contains('d-none') ?
                    '<i class="bi-funnel"></i> Show Filters' :
                    '<i class="bi-funnel-fill"></i> Hide Filters';
            });
        }
    }

    function initSortOptions() {
        var sortSelect = document.getElementById('sort-by');
        if (sortSelect) {
            sortSelect.addEventListener('change', function () {
                var url = new URL(window.location.href);
                url.searchParams.set('sortBy', this.value);
                window.location.href = url.toString();
            });
        }
    }

    // Swap source/destination cities
    window.swapCities = function () {
        var srcInput = document.getElementById('source-city');
        var dstInput = document.getElementById('dest-city');
        var srcHidden = document.getElementById('SourceCityId');
        var dstHidden = document.getElementById('DestinationCityId');

        if (srcInput && dstInput) {
            var tempVal = srcInput.value;
            srcInput.value = dstInput.value;
            dstInput.value = tempVal;

            if (srcHidden && dstHidden) {
                var tempId = srcHidden.value;
                srcHidden.value = dstHidden.value;
                dstHidden.value = tempId;
            }
        }
    };

    // Apply filter
    window.applyFilters = function () {
        document.getElementById('search-form').submit();
    };

    // Clear filters
    window.clearFilters = function () {
        document.querySelectorAll('#filter-panel input, #filter-panel select').forEach(function (el) {
            if (el.type === 'checkbox') el.checked = false;
            else if (el.type === 'number') el.value = '';
            else el.value = '';
        });
    };

})();
