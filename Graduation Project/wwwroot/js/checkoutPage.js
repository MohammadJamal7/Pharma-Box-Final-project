The provided code snippet seems to be a well - structured function for placing an order(placeOrder) and a DOMContentLoaded event listener for populating the cart table.Here's how we can integrate this code into separate files:

1. checkout.js:

JavaScript

async function placeOrder() {
    const address = document.getElementById('c_diff_address').value.trim();
    const cityId = parseInt(document.getElementById('c_diff_city').value, 10);
    const cartItems = JSON.parse(localStorage.getItem('cart'));

    if (!cartItems || cartItems.length === 0) {
        alert('Your cart is empty!');
        return;
    }

    if (!address || isNaN(cityId)) {
        alert('Please provide a valid address and city.');
        return;
    }

    const totalAmount = cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const orderData = {
        status: "Preparing",
        orderDate: new Date().toISOString(),
        totalAmount: totalAmount,
        branchId: cartItems[0].branchId || 1, // Fallback to a default branch if necessary
        orderItems: cartItems.map(item => ({
            quantity: item.quantity,
            price: item.price,
            medicineId: item.medicineId
        }))
    };

    try {
        const response = await fetch('/Cart/CreateOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(orderData)
        });

        if (response.ok) {
            localStorage.removeItem('cart');
            const result = await response.json();
            alert('Order placed successfully! Redirecting...');
            window.location.href = `/Home/Index`;
        } else {
            const errorText = await response.text();
            alert('Failed to place order: ' + errorText);
        }
    } catch (error) {
        console.error('Error placing order:', error);
        alert('An error occurred. Please try again later.');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const orderTableBody = document.querySelector('.site-block-order-table tbody');
    let total = 0;

    // First, clear any existing non-total rows
    while (orderTableBody.children.length > 1) {
        orderTableBody.removeChild(orderTableBody.firstChild);
    }

    // Add cart items
    cart.forEach(item => {
        const row = document.createElement('tr');
        row.innerHTML = `
                    <td>${item.name} <strong class="mx-2">x</strong> ${item.quantity}</td>
                    <td>${(item.price * item.quantity).toFixed(2)} JOD</td>
                `;
        // Insert before the last row (total row)
        orderTableBody.insertBefore(row, orderTableBody.lastElementChild);
        total += item.price * item.quantity;
    });

    // Update the total - more specific selector
    const totalCell = orderTableBody.querySelector('tr:last-child td:last-child strong');
    if (totalCell) {
        totalCell.textContent = `${total.toFixed(2)}`;
    }
});
