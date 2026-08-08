// =========== Admin Dashboard Charts Component ===========
// Mounted at: #dashboard-charts-root

(function (root) {
    'use strict';

    const { useState, useEffect } = React;

    function DashboardCharts(props) {
        const [revenueData, setRevenueData] = useState(props.initialRevenue || []);
        const [bookingData, setBookingData] = useState(props.initialBookings || []);
        const [period, setPeriod] = useState('weekly');

        useEffect(() => {
            if (props.fetchUrl) {
                fetch(`${props.fetchUrl}?period=${period}`)
                    .then(r => r.json())
                    .then(data => {
                        if (data.revenue) setRevenueData(data.revenue);
                        if (data.bookings) setBookingData(data.bookings);
                    })
                    .catch(() => {});
            }
        }, [period]);

        function renderBarChart(data, label, color) {
            if (!data || data.length === 0) {
                return React.createElement('p', { className: 'text-muted text-center p-3' }, 'No data available');
            }

            const maxVal = Math.max(...data.map(d => d.value), 1);
            const barHeight = 200;

            return React.createElement('div', {
                style: {
                    display: 'flex',
                    alignItems: 'flex-end',
                    height: `${barHeight + 30}px`,
                    gap: '4px',
                    padding: '0 10px'
                }
            },
                data.map((d, i) =>
                    React.createElement('div', {
                        key: i,
                        style: {
                            flex: 1,
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center'
                        }
                    },
                        React.createElement('div', {
                            style: {
                                width: '100%',
                                height: `${(d.value / maxVal) * barHeight}px`,
                                background: color,
                                borderRadius: '4px 4px 0 0',
                                transition: 'height 0.3s',
                                minHeight: d.value > 0 ? '4px' : '0'
                            },
                            title: `${label}: ${d.value}`
                        }),
                        React.createElement('span', {
                            style: {
                                fontSize: '0.6rem',
                                color: '#64748b',
                                marginTop: '4px',
                                textAlign: 'center',
                                overflow: 'hidden',
                                textOverflow: 'ellipsis',
                                maxWidth: '60px'
                            }
                        }, d.label)
                    )
                )
            );
        }

        function renderPeriodSelector() {
            const periods = [
                { value: 'weekly', label: 'Weekly' },
                { value: 'monthly', label: 'Monthly' },
                { value: 'yearly', label: 'Yearly' }
            ];

            return React.createElement('div', { className: 'btn-group btn-group-sm mb-3' },
                periods.map(p =>
                    React.createElement('button', {
                        key: p.value,
                        className: `btn ${period === p.value ? 'btn-primary' : 'btn-outline-primary'}`,
                        onClick: () => setPeriod(p.value)
                    }, p.label)
                )
            );
        }

        return React.createElement('div', null,
            renderPeriodSelector(),
            React.createElement('div', { className: 'row' },
                React.createElement('div', { className: 'col-md-6' },
                    React.createElement('div', { className: 'chart-container' },
                        React.createElement('h6', null, 'Revenue'),
                        renderBarChart(revenueData, 'Revenue', '#2563eb')
                    )
                ),
                React.createElement('div', { className: 'col-md-6' },
                    React.createElement('div', { className: 'chart-container' },
                        React.createElement('h6', null, 'Bookings'),
                        renderBarChart(bookingData, 'Bookings', '#16a34a')
                    )
                )
            )
        );
    }

    // Mount
    document.addEventListener('DOMContentLoaded', function () {
        Array.from(document.querySelectorAll('#dashboard-charts-root')).forEach(el => {
            const props = {
                initialRevenue: JSON.parse(el.dataset.revenue || '[]'),
                initialBookings: JSON.parse(el.dataset.bookings || '[]'),
                fetchUrl: el.dataset.fetchUrl
            };
            ReactDOM.render(React.createElement(DashboardCharts, props), el);
        });
    });

})(window);
