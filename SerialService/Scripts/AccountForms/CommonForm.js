function ShowErrorMessage() {
    var error = $('.error')
    if (error.is(":hidden")) {
        error.show();
    }
}

function HideErrorMessage() {
    var error = $('.error')
    if (error.is(":visible")) {
        error.hide();
    }
}

function SubmitButtonManage() {
    var fieldsIsValid = document.getElementsByClassName('is-invalid').length === 0
    var button = document.getElementById('submit-btn')
    var attr = 'disabled';
    var battonIsDisabled = button.hasAttribute(attr);
    if (battonIsDisabled && fieldsIsValid) {
        button.removeAttribute(attr)
    }
    else if (!battonIsDisabled && !fieldsIsValid) {
        button.setAttribute(attr, attr)
    }
}

function ValidatePassword() {
    var password = document.getElementById('password')
    var confPassword = document.getElementById('confitm-password')
    var confPasswordContainer = document.getElementById('confirm-password-conrainer')
    var invalid = 'is-invalid';
    if (password.value != confPassword.value) {
        confPasswordContainer.classList.add(invalid)
    }
    else {
        confPasswordContainer.classList.remove(invalid)
    }
}