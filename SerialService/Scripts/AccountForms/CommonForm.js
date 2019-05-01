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
    setTimeout(function () {
        var fieldsIsValid = document.getElementsByClassName('is-invalid').length === 0;
        var requieredFields = document.getElementsByClassName('required-field');
        Array.prototype.forEach.call(requieredFields, function (field) {
            console.log(field.value);
            if (!field.value) {
                fieldsIsValid = false;
            }
        });
        var button = document.getElementById('submit-btn');
        var attr = 'disabled';
        var battonIsDisabled = button.hasAttribute(attr);
        if (battonIsDisabled && fieldsIsValid) {
            button.removeAttribute(attr)
        }
        else if (!battonIsDisabled && !fieldsIsValid) {
            button.setAttribute(attr, attr)
        }
    }, 50)    
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