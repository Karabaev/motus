var IsEmailReset = true;

function EmailBtnClick() {
    IsEmailReset = true;
}

function ParoleBtnClick() {
    IsEmailReset = false;
}

// Модель сброса пароля по parole
function GenerateParoleResetModelJson() {
    var json = {
        Email: $("#email").val(),
        Parole: $("#parole").val(),
    };
    return json;
}

function Submit() {
    var json = $('#login-form').serialize();
    var action = "";

    if (IsEmailReset) {
        action = "/email_forgot";
    }
    else {
        action = "";
    }

    $.post(action,
        json,
        function (result) {
            if (result.error) {
                $('.error').html(result.error);
                ShowErrorMessage();
            }
            else if (result.success) {
                window.location.href = result.success;
            }
        },
        "json");
}

function ClearHidenForm(inputSelector) {
    $(inputSelector).keydown(function () {
        var changedInput = $(this).attr('Id');
        var unselectedInput = $(inputSelector).not(document.getElementById(changedInput));
        unselectedInput.val('');
        unselectedInput.parent().removeClass('is-invalid is-dirty');
    })
}

function ButtonManage(inputSelector) {
    var isValide = document.getElementsByClassName('is-invalid').length === 0;
    var isRequired = false;
    $(inputSelector).each(function () {
        if ($(this).val().length > 0) {
            isRequired = true;
        }
    });

    var button = document.getElementById('submit-btn');
    var attr = 'disabled';
    var battonIsDisabled = button.hasAttribute(attr);

    if (battonIsDisabled && isValide && isRequired) {
        button.removeAttribute(attr)
    }
    else if (!battonIsDisabled && (!isValide || !isRequired)) {
        button.setAttribute(attr, attr)
    }
}

$(document).ready(function () {
    $('#by-email-btn').click(function (e) {
        e.preventDefault();
        EmailBtnClick();
    });
    $('#by-parole-btn').click(function (e) {
        e.preventDefault();
        ParoleBtnClick();
    });
    $('#submit-btn').click(function (e) {
        e.preventDefault();
        Submit();
    });
    const forgonSelector = '.forgot-form-input';
    ClearHidenForm(forgonSelector);
    $(forgonSelector).change(function (e) {
        ButtonManage(forgonSelector);
    })
})