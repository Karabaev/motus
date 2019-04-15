var IsEmailReset = true;

function EmailBtnClick() {
    IsEmailReset = true;
}

function ParoleBtnClick() {
    IsEmailReset = false;
}

// Модель сброса пароля по email
function GenerateEmailResetModelJson() {
    var json = {
        Email: $("#email").val(),
    };
    return json;
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
    if (IsEmailReset) {
        var json = GenerateEmailResetModelJson();
        $.post("/email_forgot",
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
    else {
        var json = GenerateParoleResetModelJson();
        $.post("",
            json,
            function (result) {

            },
            "json");
    }
}

$(document).ready(function () {
    //$('input').change(function () {
    //    SubmitButtonManage();
    //    ValidatePassword();
    //});
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
});