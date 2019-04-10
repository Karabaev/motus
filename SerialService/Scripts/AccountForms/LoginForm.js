function CheckBoxManage() {
    var checkboxContainer = $('.mdl-checkbox')
    checkboxContainer.click(function () {
        setTimeout(function () {
            var checkbox = $('#remember')
            if (checkboxContainer.hasClass('is-checked')) {
                checkbox.val('true')
            }
            else {
                checkbox.val('false')
            }
        }, 50)
    })
}
function SubmitLoginForm() {
    var formObj = $('#login-form').serialize()
    console.log(formObj)
    $.ajax({
        method: 'post',
        data: formObj,
        url: 'login',
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
    CheckBoxManage()
    $('input').change(function (e) {
        e.preventDefault()
        SubmitButtonManage()
        HideErrorMessage()
    })
    $('#submit-btn').click(function (e) {
        e.preventDefault();
        SubmitLoginForm();
    })
})