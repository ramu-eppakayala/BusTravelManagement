// =========== Booking Scripts ===========

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        initSeatSelector();
        initPassengerForms();
        initPaymentMethods();
        initCouponApply();
        initBookingSteps();
    });

    // Seat Selector
    function initSeatSelector() {
        var container = document.getElementById('seat-selector');
        if (!container) return;

        var scheduleId = container.dataset.scheduleId;
        var journeyDate = container.dataset.journeyDate;
        var selectedSeats = [];
        var seatPrice = parseFloat(container.dataset.fare || 0);

        // Fetch seats
        fetch('/Search/GetAvailableSeats?scheduleId=' + scheduleId + '&journeyDate=' + journeyDate)
            .then(function (r) { return r.json(); })
            .then(function (seats) {
                renderSeatGrid(seats);
            });

        function renderSeatGrid(seats) {
            if (!seats || seats.length === 0) {
                container.innerHTML = '<div class="alert alert-warning">Seat layout not available.</div>';
                return;
            }

            var maxRow = 0, maxCol = 0;
            seats.forEach(function (s) {
                if (s.rowNumber > maxRow) maxRow = s.rowNumber;
                if (s.columnNumber > maxCol) maxCol = s.columnNumber;
            });

            var grid = document.createElement('div');
            grid.className = 'seat-grid';
            grid.style.gridTemplateColumns = 'repeat(' + (maxCol + 1) + ', auto)';

            // Add driver seat placeholder
            var driverLabel = document.createElement('div');
            driverLabel.className = 'driver-seat';
            driverLabel.textContent = 'DRIVER';
            grid.appendChild(driverLabel);

            for (var i = 1; i <= maxCol; i++) {
                grid.appendChild(document.createElement('div'));
            }

            for (var row = 1; row <= maxRow; row++) {
                for (var col = 0; col <= maxCol; col++) {
                    var seat = seats.find(function (s) {
                        return s.rowNumber === row && s.columnNumber === col;
                    });

                    if (seat) {
                        var el = document.createElement('div');
                        el.className = 'seat seat-' + (seat.status || 'available').toLowerCase();
                        el.textContent = seat.seatNumber;
                        el.dataset.seatId = seat.seatLayoutId;
                        el.dataset.price = seat.fare || seatPrice;

                        if (seat.status === 'Available' || seat.isLadiesSeat) {
                            el.addEventListener('click', function () {
                                toggleSeat(this);
                            });
                            if (seat.isLadiesSeat) {
                                el.classList.add('seat-ladies');
                            }
                        } else {
                            el.style.cursor = 'not-allowed';
                            el.title = seat.status === 'Booked' ? 'Already Booked' : 'Reserved';
                        }

                        grid.appendChild(el);
                    } else {
                        var empty = document.createElement('div');
                        empty.style.width = '40px';
                        empty.style.height = '40px';
                        grid.appendChild(empty);
                    }
                }
            }

            container.appendChild(grid);

            // Legend
            var legend = document.createElement('div');
            legend.className = 'seat-legend';
            legend.innerHTML = '' +
                '<div class="seat-legend-item"><div class="seat-legend-box" style="background:#dcfce7;border:1px solid #86efac"></div>Available</div>' +
                '<div class="seat-legend-item"><div class="seat-legend-box" style="background:#fee2e2;border:1px solid #fca5a5"></div>Booked</div>' +
                '<div class="seat-legend-item"><div class="seat-legend-box" style="background:#fce7f3;border:1px solid #f9a8d4"></div>Ladies</div>' +
                '<div class="seat-legend-item"><div class="seat-legend-box" style="background:#2563eb"></div>Selected</div>';
            container.appendChild(legend);

            updateSummary();
        }

        function toggleSeat(el) {
            var seatId = el.dataset.seatId;
            var idx = selectedSeats.indexOf(seatId);

            if (idx >= 0) {
                selectedSeats.splice(idx, 1);
                el.classList.remove('seat-selected');
                el.classList.add('seat-available');
            } else {
                if (selectedSeats.length >= 6) {
                    showToast('warning', 'Limit', 'Maximum 6 seats per booking');
                    return;
                }
                selectedSeats.push(seatId);
                el.classList.remove('seat-available');
                el.classList.add('seat-selected');
            }

            updateSummary();
        }

        function updateSummary() {
            var summary = document.getElementById('seat-summary');
            if (!summary) return;

            var count = selectedSeats.length;
            var total = 0;
            selectedSeats.forEach(function (id) {
                var el = container.querySelector('[data-seat-id="' + id + '"]');
                if (el) total += parseFloat(el.dataset.price || 0);
            });

            var fareInfo = document.getElementById('fare-info');
            if (fareInfo) {
                fareInfo.textContent = count + ' seat(s) selected | Total: ' + formatCurrency(total);
            }

            document.getElementById('selected-seats-input').value = selectedSeats.join(',');
        }
    }

    // Passenger Forms
    function initPassengerForms() {
        var container = document.getElementById('passengers-container');
        if (!container) return;

        container.querySelectorAll('.passenger-form').forEach(function (form, idx) {
            var header = form.querySelector('.passenger-header strong');
            if (header) header.textContent = 'Passenger ' + (idx + 1);
        });
    }

    // Payment Methods
    function initPaymentMethods() {
        var methods = document.querySelectorAll('.payment-method-btn');
        methods.forEach(function (btn) {
            btn.addEventListener('click', function () {
                methods.forEach(function (b) { b.classList.remove('active'); });
                this.classList.add('active');
                var methodInput = document.getElementById('PaymentMethod');
                if (methodInput) methodInput.value = this.dataset.method;
                toggleCardForm(this.dataset.method);
            });
        });
    }

    function toggleCardForm(method) {
        var cardForm = document.getElementById('card-form');
        if (cardForm) {
            cardForm.style.display = (method === 'Credit Card' || method === 'Debit Card') ? 'block' : 'none';
        }
    }

    // Coupon Apply
    function initCouponApply() {
        var applyBtn = document.getElementById('apply-coupon');
        if (!applyBtn) return;

        applyBtn.addEventListener('click', function () {
            var code = document.getElementById('coupon-code').value.trim();
            if (!code) return;

            var amountEl = document.getElementById('booking-amount');
            var amount = amountEl ? parseFloat(amountEl.value || 0) : 0;

            this.disabled = true;
            this.textContent = 'Validating...';

            var formData = new FormData();
            formData.append('code', code);
            formData.append('amount', amount);

            fetch('/Booking/ValidateCoupon', {
                method: 'POST',
                body: new URLSearchParams({ code: code, amount: amount }),
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
            })
                .then(function (r) { return r.json(); })
                .then(function (result) {
                    if (result.isValid) {
                        showToast('success', 'Coupon Applied', 'You save ' + formatCurrency(result.discountAmount));
                        document.getElementById('discount-amount').textContent = formatCurrency(result.discountAmount);
                        recalculateTotal();
                    } else {
                        showToast('error', 'Invalid Coupon', result.message);
                    }
                })
                .catch(function () {
                    showToast('error', 'Error', 'Failed to validate coupon');
                })
                .finally(function () {
                    applyBtn.disabled = false;
                    applyBtn.textContent = 'Apply';
                });
        });
    }

    function recalculateTotal() {
        // Recalculate total fare after coupon
        // This would be implemented with the full fare calculation
    }

    // Booking Steps Progress
    function initBookingSteps() {
        var steps = document.querySelectorAll('.step');
        var currentStepEl = document.getElementById('current-step');
        if (currentStepEl) {
            var current = parseInt(currentStepEl.value || 1);
            steps.forEach(function (step, idx) {
                var num = idx + 1;
                step.classList.remove('active', 'completed');
                if (num < current) step.classList.add('completed');
                else if (num === current) step.classList.add('active');
            });
        }
    }

})();
