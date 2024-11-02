let productIdToRemove = null;

function removeProduct(id) {
    productIdToRemove = id; // Store the ID of the product to remove
    document.getElementById("removeModal").style.display = "flex"; // Show the confirmation modal
}

function closeRemoveModal() {
    document.getElementById("removeModal").style.display = "none"; // Hide the modal
    productIdToRemove = null; // Clear the stored ID
}

function confirmRemove() {
    // Make a fetch request to the server to remove the product
    fetch(`/Admin/DeleteProduct/${productIdToRemove}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                // Optionally reload the page or update the product list
                window.location.reload();
            } else {
                console.error('Error removing product:', response.statusText);
            }
        })
        .catch(error => console.error('Fetch error:', error));
}

// Close modal when clicking outside of it
window.onclick = function (event) {
    const modal = document.getElementById("removeModal");
    if (event.target === modal) {
        closeRemoveModal();
    }
}
