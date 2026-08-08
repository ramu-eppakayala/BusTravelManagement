// =========== Seat Selector React Component ===========
// Mounted at: #seat-selector-root

(function (root) {
    'use strict';

    const { useState, useEffect, useCallback } = React;

    function SeatSelector(props) {
        const [seats, setSeats] = useState([]);
        const [selectedSeats, setSelectedSeats] = useState([]);
        const [loading, setLoading] = useState(true);
        const [error, setError] = useState(null);
        const [busInfo, setBusInfo] = useState(null);

        const scheduleId = props.scheduleId;
        const journeyDate = props.journeyDate;
        const maxSeats = parseInt(props.maxSeats) || 6;

        useEffect(() => {
            fetchSeats();
        }, []);

        function fetchSeats() {
            setLoading(true);
            fetch(`/Search/GetAvailableSeats?scheduleId=${scheduleId}&journeyDate=${journeyDate}`)
                .then(r => r.json())
                .then(data => {
                    setSeats(data);
                    setLoading(false);
                })
                .catch(err => {
                    setError('Failed to load seat layout');
                    setLoading(false);
                });
        }

        function toggleSeat(seat) {
            if (seat.status === 'Booked' || seat.status === 'Reserved') return;

            setSelectedSeats(prev => {
                const exists = prev.find(s => s.seatLayoutId === seat.seatLayoutId);
                if (exists) {
                    return prev.filter(s => s.seatLayoutId !== seat.seatLayoutId);
                }
                if (prev.length >= maxSeats) {
                    showToast('warning', 'Limit reached', `Maximum ${maxSeats} seats allowed`);
                    return prev;
                }
                return [...prev, { ...seat, status: 'Selected' }];
            });
        }

        function getSeatClass(seat) {
            if (selectedSeats.find(s => s.seatLayoutId === seat.seatLayoutId)) return 'seat seat-selected';
            switch (seat.status) {
                case 'Booked': return 'seat seat-booked';
                case 'Reserved': return 'seat seat-reserved';
                case 'Ladies': return 'seat seat-ladies';
                default: return seat.isLadiesSeat ? 'seat seat-ladies' : 'seat seat-available';
            }
        }

        function renderSeatGrid() {
            if (!seats || seats.length === 0) {
                return React.createElement('div', { className: 'alert alert-info' }, 'No seats available');
            }

            const maxRow = Math.max(...seats.map(s => s.rowNumber));
            const maxCol = Math.max(...seats.map(s => s.columnNumber));
            const columns = maxCol + 1;

            const gridStyle = {
                display: 'inline-grid',
                gridTemplateColumns: `repeat(${columns}, auto)`,
                gap: '6px',
                padding: '1rem',
                background: '#f8fafc',
                borderRadius: '8px'
            };

            const driverStyle = {
                width: '40px',
                height: '40px',
                background: '#e2e8f0',
                borderRadius: '6px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: '0.65rem',
                color: '#64748b'
            };

            const cells = [];

            // Driver cell + top padding
            cells.push(React.createElement('div', { key: 'driver', style: driverStyle }, 'DRIVER'));
            for (let i = 1; i <= maxCol; i++) {
                cells.push(React.createElement('div', { key: `top-${i}`, style: { width: '40px', height: '40px' } }));
            }

            for (let row = 1; row <= maxRow; row++) {
                for (let col = 0; col <= maxCol; col++) {
                    const seat = seats.find(s => s.rowNumber === row && s.columnNumber === col);
                    if (seat) {
                        const isBooked = seat.status === 'Booked' || seat.status === 'Reserved';
                        const seatEl = React.createElement('div', {
                            key: `seat-${seat.seatLayoutId}`,
                            className: getSeatClass(seat),
                            onClick: () => toggleSeat(seat),
                            title: `${seat.seatNumber} - ${seat.status}`
                        }, seat.seatNumber);
                        cells.push(seatEl);
                    } else {
                        cells.push(React.createElement('div', {
                            key: `empty-${row}-${col}`,
                            style: { width: '40px', height: '40px' }
                        }));
                    }
                }
            }

            return React.createElement('div', { style: gridStyle }, ...cells);
        }

        function renderSummary() {
            const totalFare = selectedSeats.reduce((sum, s) => sum + (s.fare || 0), 0);
            return React.createElement('div', { className: 'card mt-3' },
                React.createElement('div', { className: 'card-body' },
                    React.createElement('h6', null, 'Selected Seats'),
                    React.createElement('p', { className: 'mb-1' },
                        `${selectedSeats.length} seat(s): `,
                        selectedSeats.map(s => s.seatNumber).join(', ') || 'None'
                    ),
                    React.createElement('h5', { className: 'text-primary' },
                        `Total: \u20B9${totalFare.toFixed(2)}`
                    ),
                    selectedSeats.length > 0 && React.createElement('button', {
                        className: 'btn btn-primary w-100 mt-2',
                        onClick: () => {
                            document.getElementById('SeatLayoutIds').value =
                                selectedSeats.map(s => s.seatLayoutId).join(',');
                            document.getElementById('seat-form').submit();
                        }
                    }, 'Continue with Selected Seats')
                )
            );
        }

        function renderLegend() {
            const legend = [
                { cls: 'seat-available', label: 'Available' },
                { cls: 'seat-booked', label: 'Booked' },
                { cls: 'seat-ladies', label: 'Ladies' },
                { cls: 'seat-selected', label: 'Selected' }
            ];

            return React.createElement('div', { className: 'seat-legend' },
                legend.map(item =>
                    React.createElement('div', { key: item.label, className: 'seat-legend-item' },
                        React.createElement('div', {
                            className: 'seat-legend-box',
                            style: {
                                background: item.cls === 'seat-available' ? '#dcfce7' :
                                    item.cls === 'seat-booked' ? '#fee2e2' :
                                        item.cls === 'seat-ladies' ? '#fce7f3' : '#2563eb',
                                border: item.cls === 'seat-selected' ? 'none' : '1px solid #ccc'
                            }
                        }),
                        item.label
                    )
                )
            );
        }

        if (loading) {
            return React.createElement('div', { className: 'text-center p-5' },
                React.createElement('div', { className: 'spinner-border', role: 'status' }),
                React.createElement('p', { className: 'mt-2 text-muted' }, 'Loading seat layout...')
            );
        }

        if (error) {
            return React.createElement('div', { className: 'alert alert-danger' }, error);
        }

        return React.createElement('div', null,
            renderLegend(),
            React.createElement('div', { className: 'text-center' }, renderSeatGrid()),
            renderSummary()
        );
    }

    // Mount components
    document.addEventListener('DOMContentLoaded', function () {
        const mountPoints = document.querySelectorAll('#seat-selector-root');
        mountPoints.forEach(el => {
            const props = {
                scheduleId: el.dataset.scheduleId,
                journeyDate: el.dataset.journeyDate,
                maxSeats: el.dataset.maxSeats
            };
            ReactDOM.render(React.createElement(SeatSelector, props), el);
        });
    });

})(window);
