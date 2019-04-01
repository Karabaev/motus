function ConfirmChangesShow() {
    var confirmField = $('#confirm-changes')
    $('input').change(function () {
        if (confirmField.is(':visible')) {
            return
        }
        confirmField.slideDown();
    })
}
function PanelShow(id) {
    var panel = $('#' + id)
    if (panel.is(':visible')) {
        panel.slideUp()
    }
    else {
        panel.slideDown()
    }
}
function ProfilePanelManage() {
    const rotatedClass = 'rotated'
    var arrow = $('.exp-panel-arrow')
    arrow.click(function () {
        var isRotated = $(this).hasClass(rotatedClass)
        if (isRotated) {
            $(this).removeClass(rotatedClass)
            isRotated = false
        }
        else {
            $(this).addClass(rotatedClass);
            isRotated = true;
        }
        PanelShow($(this).attr('for'));
    })
}
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#CurrentAvatarURLImg').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}
function AvatarBtnManage() {
    var btn = document.getElementsByClassName('avatar-btn')[0]
    var input = document.getElementById('InputFile')
    btn.addEventListener('click', function () {
        input.click()
    })
}
function AvatarPanelManage() {
    var continer = document.getElementsByClassName('avatar-container')[0]
    var panel = document.getElementsByClassName('avatar-panel')[0]
    continer.addEventListener('mouseover', function () {
        panel.style.display = 'block'
    })
    continer.addEventListener('mouseout', function () {
        panel.style.display = 'none'
    })
}
$(document).ready(function () {
    ConfirmChangesShow()
    AvatarPanelManage()
    AvatarBtnManage()
    ProfilePanelManage()
    $('#InputFile').change(function () {
        readURL(this)
    })
    $('input').change(function () {
        SubmitButtonManage()
        ValidatePassword()
    })
})