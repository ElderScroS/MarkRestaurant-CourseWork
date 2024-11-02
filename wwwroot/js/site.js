var openModal = document.getElementById("openModal");
var modalLog = document.getElementById("modalLog");
var modalReg = document.getElementById("modalReg");
var modalForgotPassword = document.getElementById("modalForgotPassword");

var closeModalLog = document.getElementById("closeModalLog");
var closeModalReg = document.getElementById("closeModalReg");
var closeModalForgot = document.getElementById("closeModalForgot");

var signupbtn = document.getElementById("signupbtn");
var signinbtn = document.getElementById("signinbtn");
var signinbtnForgot = document.getElementById("signinbtnForgot");

var forgotPasswordLink = document.getElementById("forgotPasswordLink");
var signupbtnForgot = document.getElementById("signupbtnForgot");

forgotPasswordLink.addEventListener("click", function () {
    modalForgotPassword.style.display = "block";
    modalLog.style.display = "none";
    modalReg.style.display = "none";
});

closeModalForgot.addEventListener("click", function () {
    modalForgotPassword.style.display = "none";
});

if (signinbtnForgot) {
    signinbtnForgot.addEventListener("click", function () {
        modalForgotPassword.style.display = "none";
        modalLog.style.display = "block";
    });
}

if (signupbtnForgot) {
    signupbtnForgot.addEventListener("click", function () {
        modalForgotPassword.style.display = "none";
        modalReg.style.display = "block";
    });
}

openModal.addEventListener("click", function () {
    modalLog.style.display = "block";
    modalReg.style.display = "none";
    modalForgotPassword.style.display = "none";
});

signinbtn.addEventListener("click", function () {
    modalLog.style.display = "block";
    modalReg.style.display = "none";
    modalForgotPassword.style.display = "none";
});

document.getElementById("forgotPasswordLinkReg").addEventListener("click", function () {
    modalForgotPassword.style.display = "block";
    modalReg.style.display = "none";
    modalLog.style.display = "none";
});

signupbtn.addEventListener("click", function () {
    modalReg.style.display = "block";
    modalLog.style.display = "none";
    modalForgotPassword.style.display = "none";
});

closeModalLog.addEventListener("click", function () {
    modalLog.style.display = "none";
});

closeModalReg.addEventListener("click", function () {
    modalReg.style.display = "none";
});


document.addEventListener('DOMContentLoaded', function () {
    const modalReg = document.querySelector('#modalReg');
    const passwordInput = modalReg.querySelector('#PasswordHash');
    const signupButton = modalReg.querySelector('.btnSignup button');
    const passwordHint = modalReg.querySelector('.password-hint');

    signupButton.style.opacity = '0.5';
    signupButton.disabled = true;

    passwordInput.addEventListener('input', function () {
        const password = passwordInput.value;
        const hasDigit = /\d/.test(password);
        const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        if (password.length >= 7 && hasDigit && hasSpecialChar) {
            passwordHint.textContent = 'Password is strong';
            passwordHint.style.color = 'rgba(0, 255, 0, 0.7)';
            signupButton.disabled = false;
            signupButton.style.opacity = '1';
        } else {
            passwordHint.textContent = 'Password is weak';
            passwordHint.style.color = '#a62d24';
            signupButton.disabled = true;
            signupButton.style.opacity = '0.5';
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const filterItems = document.querySelectorAll('.filter-list .flist-item');
    const products = document.querySelectorAll('.products .product');

    filterItems.forEach(item => {
        item.addEventListener('click', function () {

            filterItems.forEach(item => item.classList.remove('active'));
            this.classList.add('active');

            const category = this.getAttribute('data-category');

            products.forEach(product => {
                const productCategory = product.classList.contains(category) || category === 'All';
                if (productCategory) {
                    product.style.display = 'block';
                } else {
                    product.style.display = 'none';
                }
            });
        });
    });
});

function openBasketModal() {
    document.getElementById("basketModal").style.display = "flex";
}

function closeBasketModal() {
    document.getElementById("basketModal").style.display = "none";
}

function showReceiptModal() {
    var receiptModal = document.getElementById("receiptModal");
    receiptModal.style.display = "flex";

    setTimeout(function () {
        receiptModal.style.display = "none";
        openBasketModal();
    }, 2000);
}

document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('basketOrderModal');
    var closeBtn = document.querySelector('.basket-modal .basket-close');
    var finishOrderForm = document.getElementById('finishOrderForm');
    var ratingStars = document.querySelectorAll('.stars input');

    finishOrderForm.addEventListener('submit', function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        fetch(this.action, {
            method: this.method,
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    modal.style.display = 'block';
                }
            })
            .catch(error => {
                console.error('Error finishing the order:', error);
            });
    });

    closeBtn.addEventListener('click', function () {
        modal.style.display = 'none';
    });

    window.addEventListener('click', function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    });

    ratingStars.forEach(star => {
        star.addEventListener('change', function () {
            var ratingValue = this.id.replace('star', '');
            var basketClearForm = document.createElement('form');
            basketClearForm.method = 'post';
            basketClearForm.action = '/Account/ClearProductsInBasket';

            var emailField = document.createElement('input');
            emailField.type = 'hidden';
            emailField.name = 'email';
            emailField.value = '@Model.UserName';
            basketClearForm.appendChild(emailField);

            document.body.appendChild(basketClearForm);
            basketClearForm.submit();

            window.location.href = '/Account/LIndex';
        });
    });
});

