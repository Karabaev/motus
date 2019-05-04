// Модель сброса пароля по parole
function GenerateParoleResetModelJson() {
    var json = {
        __RequestVerificationToken: $("input[name = '__RequestVerificationToken']").val(),
        Email: $("#identity-email").val(),
        Parole: $("#parole").val(),
    };
    return json;
}

function Submit() {
    var json = $('#login-form').serialize();
    var action = "";

    if ($('.is-active').first().attr('href') === '#by-email') {
        action = "/email_forgot";
    }
    else {
        json = GenerateParoleResetModelJson();
        action = "/parole_forgot";
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
        var hiddenTabs = $('.mdl-tabs__panel').filter(function (index) {
            return $(this).is(':hidden');
        });
        console.log(hiddenTabs.length);
        var hiddenInputs = hiddenTabs.find('input');
        hiddenInputs.val('');
        hiddenInputs.parent().removeClass('is-invalid is-dirty');
    })
}

function ButtonManage(inputSelector) {
    var isValide = document.getElementsByClassName('is-invalid').length === 0;
    var isRequired = true;
    $('.is-active input').each(function () {
        if ($(this).val().length === 0) {
            isRequired = false;
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
    const forgotSelector = '.forgot-form-input';
    ClearHidenForm(forgotSelector);
    $(forgotSelector).change(function (e) {
        ButtonManage(forgotSelector);
    })
})