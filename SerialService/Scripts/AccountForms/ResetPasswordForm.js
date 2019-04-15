function GenerateModelJson() {
    var json = {
        Code: "12345", //$("#code").val(),
        Email: $("#email").val(),
        Password: $("#password").val(),
        ConfirmPassword: $("#confitm-password").val(),
    };
    return json;
}

function SubmitResetForm() {
    var json = $('#reset-form').serialize()
    //var json = GenerateModelJson();

    $.post("/reset_password",
        json,
        function (result) {
            if (result.error) {
                $('.error').html(result.error)
                ShowErrorMessage()
            }
            else if (result.success) {
                window.location.href = result.success
            }
        },
        "json");
}

$(document).ready(function () {
    $('input').change(function (e) {
        e.preventDefault()
        ValidatePassword()
        SubmitButtonManage()
        HideErrorMessage()
    })
    $('#submit-btn').click(function (e) {
        e.preventDefault();
        SubmitResetForm();
    })
})