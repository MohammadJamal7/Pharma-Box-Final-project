// cartManager.js

class CartManager {
    constructor() {
        this.cart = JSON.parse(localStorage.getItem('cart')) || [];
        this.currentBranch = localStorage.getItem('currentBranch') || null;
    }

    // Check if attempting to add items from a different branch
    checkBranch(branchId) {
        if (!this.currentBranch) {
            this.currentBranch = branchId;
            localStorage.setItem('currentBranch', branchId);
            return true;
        }

        return this.currentBranch === branchId;
    }

    // Add item to cart with branch validation
    async addToCart(item, branchId) {
        if (!this.checkBranch(branchId)) {
            const confirmed = await this.showBranchChangeConfirmation();
            if (confirmed) {
                this.clearCart();
                this.currentBranch = branchId;
                localStorage.setItem('currentBranch', branchId);
            } else {
                return false;
            }
        }

        const existingItem = this.cart.find(i => i.medicineId === item.medicineId);
        if (existingItem) {
            existingItem.quantity += item.quantity;
        } else {
            this.cart.push(item);
        }

        this.saveCart();
        return true;
    }

    // Clear cart and branch
    clearCart() {
        this.cart = [];
        this.saveCart();
    }

    // Save cart to localStorage
    saveCart() {
        localStorage.setItem('cart', JSON.stringify(this.cart));
    }

    // Show confirmation dialog for branch change
    showBranchChangeConfirmation() {
        return new Promise((resolve) => {
            const result = confirm(
                "You are attempting to add items from a different branch. " +
                "Your current cart will be cleared if you proceed. " +
                "Would you like to continue?"
            );
            resolve(result);
        });
    }

    // Get current branch ID
    getCurrentBranch() {
        return this.currentBranch;
    }

    // Get cart items
    getCart() {
        return this.cart;
    }
}

// Initialize cart manager globally
window.cartManager = new CartManager();