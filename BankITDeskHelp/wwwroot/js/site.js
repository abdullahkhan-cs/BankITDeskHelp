// BankITDeskHelp - Enterprise IT Support Portal
// JavaScript for common interactions and UI enhancements

document.addEventListener('DOMContentLoaded', function() {
    // Initialize form validation feedback
    initializeFormValidation();
    
    // Initialize mobile sidebar toggle
    initializeSidebarToggle();
    
    // Initialize table responsive behavior
    initializeResponsiveTables();
});

/**
 * Initialize form validation visual feedback
 */
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        const inputs = form.querySelectorAll('input, select, textarea');
        inputs.forEach(input => {
            input.addEventListener('blur', function() {
                if (this.checkValidity()) {
                    this.classList.remove('is-invalid');
                } else {
                    this.classList.add('is-invalid');
                }
            });
            
            input.addEventListener('input', function() {
                if (this.checkValidity()) {
                    this.classList.remove('is-invalid');
                }
            });
        });
    });
}

/**
 * Initialize mobile sidebar toggle functionality
 */
function initializeSidebarToggle() {
    // Add mobile menu toggle button if it doesn't exist
    const topNavbar = document.querySelector('.top-navbar');
    if (topNavbar && window.innerWidth < 768) {
        const toggleBtn = document.createElement('button');
        toggleBtn.className = 'btn btn-outline-primary btn-sm d-md-none';
        toggleBtn.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="21" y2="6"></line><line x1="3" y1="18" x2="21" y2="18"></line></svg>';
        toggleBtn.setAttribute('aria-label', 'Toggle navigation');
        toggleBtn.addEventListener('click', toggleSidebar);
        
        const brand = topNavbar.querySelector('.top-navbar-brand');
        if (brand) {
            brand.insertAdjacentElement('afterend', toggleBtn);
        }
    }
}

/**
 * Toggle sidebar visibility on mobile
 */
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (sidebar) {
        sidebar.classList.toggle('show');
    }
}

/**
 * Initialize responsive table behavior
 */
function initializeResponsiveTables() {
    const tables = document.querySelectorAll('.table');
    tables.forEach(table => {
        const wrapper = table.closest('.table-responsive');
        if (!wrapper) {
            const newWrapper = document.createElement('div');
            newWrapper.className = 'table-responsive';
            table.parentNode.insertBefore(newWrapper, table);
            newWrapper.appendChild(table);
        }
    });
}

/**
 * Show alert message (for dynamic notifications)
 */
function showAlert(message, type = 'info') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type}`;
    alertDiv.innerHTML = `<strong>${type.charAt(0).toUpperCase() + type.slice(1)}:</strong> ${message}`;
    
    const container = document.querySelector('.container-content');
    if (container) {
        container.insertBefore(alertDiv, container.firstChild);
        
        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }
}

/**
 * Confirm action with custom dialog
 */
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}
