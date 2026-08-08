// =========== Admin Dashboard Scripts ===========

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initCharts();
        initUserSearch();
        initRoleAssignment();
        initConfirmActions();
    });

    function initCharts() {
        // Revenue Chart
        var revenueCanvas = document.getElementById('revenue-chart');
        if (revenueCanvas && typeof Chart !== 'undefined') {
            var revenueData = JSON.parse(revenueCanvas.dataset.values || '[]');
            new Chart(revenueCanvas, {
                type: 'bar',
                data: {
                    labels: revenueData.map(function (d) { return d.label; }),
                    datasets: [{
                        label: 'Revenue',
                        data: revenueData.map(function (d) { return d.value; }),
                        backgroundColor: '#2563eb',
                        borderRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true, ticks: { callback: function (v) { return '\u20B9' + v; } } }
                    }
                }
            });
        }

        // Booking Chart
        var bookingCanvas = document.getElementById('booking-chart');
        if (bookingCanvas && typeof Chart !== 'undefined') {
            var bookingData = JSON.parse(bookingCanvas.dataset.values || '[]');
            new Chart(bookingCanvas, {
                type: 'line',
                data: {
                    labels: bookingData.map(function (d) { return d.label; }),
                    datasets: [{
                        label: 'Bookings',
                        data: bookingData.map(function (d) { return d.value; }),
                        borderColor: '#16a34a',
                        backgroundColor: 'rgba(22,163,74,0.1)',
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } }
                }
            });
        }
    }

    function initUserSearch() {
        var searchInput = document.getElementById('user-search');
        if (!searchInput) return;

        searchInput.addEventListener('input', function () {
            var term = this.value.toLowerCase().trim();
            document.querySelectorAll('.user-row').forEach(function (row) {
                var text = row.textContent.toLowerCase();
                row.style.display = text.includes(term) ? '' : 'none';
            });
        });
    }

    function initRoleAssignment() {
        document.querySelectorAll('.assign-role-btn').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var userId = this.dataset.userId;
                var userName = this.dataset.userName;
                var modal = document.getElementById('role-modal');
                if (modal) {
                    document.getElementById('role-user-id').value = userId;
                    document.getElementById('role-user-name').textContent = userName;
                    var roleModal = new bootstrap.Modal(modal);
                    roleModal.show();
                }
            });
        });
    }

    function initConfirmActions() {
        document.querySelectorAll('[data-confirm]').forEach(function (el) {
            el.addEventListener('click', function (e) {
                if (!confirm(this.dataset.confirm || 'Are you sure?')) {
                    e.preventDefault();
                }
            });
        });
    }

    window.initOperatorCharts = function (revenueData, occupancyData) {
        var revCanvas = document.getElementById('operator-revenue-chart');
        if (revCanvas && typeof Chart !== 'undefined' && revenueData) {
            new Chart(revCanvas, {
                type: 'line',
                data: {
                    labels: revenueData.map(function (d) { return d.label; }),
                    datasets: [{
                        label: 'Revenue',
                        data: revenueData.map(function (d) { return d.value; }),
                        borderColor: '#2563eb',
                        backgroundColor: 'rgba(37,99,235,0.1)',
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { display: false } }
                }
            });
        }

        var occCanvas = document.getElementById('occupancy-chart');
        if (occCanvas && typeof Chart !== 'undefined' && occupancyData) {
            new Chart(occCanvas, {
                type: 'bar',
                data: {
                    labels: occupancyData.map(function (d) { return d.label; }),
                    datasets: [{
                        label: 'Occupancy %',
                        data: occupancyData.map(function (d) { return d.value; }),
                        backgroundColor: '#16a34a',
                        borderRadius: 4
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: { y: { min: 0, max: 100 } }
                }
            });
        }
    };

})();
