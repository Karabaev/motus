function SubmitRegisterForm() {
    var formObj = $('#reg-form').serialize()
    $.ajax({
        method: 'post',
        data: formObj,
        url: 'registration',
        success: function (result) {
            if (result.error) {
                $('.error').html(result.error)
                ShowErrorMessage()
            }
            else if (result.success) {
                window.location.href = result.success
            }
        }
    })
}

$(document).ready(function () {
    $('input').change(function (e) {
        e.preventDefault()
        ValidatePassword()
        SubmitButtonManage()
        HideErrorMessage()
    });
    $('#submit-btn').click(function (e) {
        e.preventDefault();
        SubmitRegisterForm();
    });

    $('#confirmCheck').change(function () {
        $('#confirmHidden').val(this.checked);
    });
})