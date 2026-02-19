(function () {
    function startOrderStatusPolling(orderId) {
        let pollInterval;
        let timeoutId;

        async function checkStatus() {
            try {
                const response = await fetch(`/api/orders/${orderId}/status`);

                if (!response.ok) {
                    if (response.status === 401) {
                        window.location.href =
                            '/Account/Login?returnUrl=' +
                            encodeURIComponent(window.location.pathname);
                        return;
                    }
                    throw new Error(`HTTP ${response.status}`);
                }

                const data = await response.json();

                if (data.status === 'Success' || data.status === 'Failed') {
                    clearInterval(pollInterval);
                    clearTimeout(timeoutId);
                    location.reload();
                }
            } catch (err) {
                console.error('Order status polling failed:', err);
            }
        }

        pollInterval = setInterval(checkStatus, 2000);
        timeoutId = setTimeout(() => {
            clearInterval(pollInterval);
            alert(
                'Payment verification timed out. Please refresh the page or contact support.'
            );
        }, 120000);

        window.addEventListener('beforeunload', () => {
            clearInterval(pollInterval);
            clearTimeout(timeoutId);
        });
    }

    // Expose a small public API
    window.OrderStatusPoller = {
        start: startOrderStatusPolling
    };
})();
