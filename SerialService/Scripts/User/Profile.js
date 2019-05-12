function ConfirmChangesShow() {
    var confirmField = $('#confirm-changes');
    var haveCur = $("#have-pass").val();

    if (haveCur == "True") {
        $('input:not(#InputFile, #upload-file)').change(function () {
            if (confirmField.is(':visible')) {
                return;
            }
            confirmField.slideDown();
        });
    }
    else {
        $("#password-confirm").removeAttr("required pattern");
    }
    
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
    });
}

function readURL(input) {
    var files = input.files;

    if (files && files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var data = new FormData();

            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({
                type: "POST",
                url: '/personal_account/upload_avatar',
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    if (result.error) {
                        $('.error').html(result.error);
                        ShowErrorMessage();
                    }
                    else if (result.success) {
                        $("#CurrentAvatarURLImg").attr("src", result.success);
                        input.value = "";
                    }
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                }
            });
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function AvatarBtnManage() {
    var btn = document.getElementsByClassName('avatar-btn')[0];
    var input = document.getElementById('InputFile');
    btn.addEventListener('click', function () {
        input.click();
    });
}

function AvatarPanelManage() {
    var continer = document.getElementsByClassName('avatar-container')[0];
    var panel = document.getElementsByClassName('avatar-panel')[0];
    continer.addEventListener('mouseover', function () {
        panel.style.display = 'block';
    })
    continer.addEventListener('mouseout', function () {
        panel.style.display = 'none';
    });
}

//Генерация json. 
function GenerateModelJson() {
    var json = {
        ID: $("#account-id").val(),
        IsHaveCurrentPassword: $("#have-pass").val(),
        NewUserName: $("#username").val(),
        NewPassword: $("#password").val(),
        ConfirmPassword: $("#confitm-password").val(),
        NewEmail: $("#email").val(),
        CurrentPassword: $("#password-confirm").val(),
    };
    return json;
}

function ShowChanges(email, name) {
    $("#user-name-title").text(name);
    $("#username").text(name);
    $("#email").text(email);
}

function SendChanges() {
    var formObj = GenerateModelJson();

    $.post("/personal_account/save_changes",
        formObj,
        function (result) {
            if (result.message) {
                alert(result.message);
            }

            if (result.error) {
                $('.error').html(result.error);
                ShowErrorMessage();
            }
            else if (result.success) {
                window.location.reload();
            }
        },
        "json");
}

function AvatarBtnManage() {
    var btn = document.getElementsByClassName('avatar-btn')[0];
    var input = document.getElementById('upload-file');
    btn.addEventListener('click', function () {
        input.click();
    });
}

function CropperManage() {
    var vanilla;
    $('#upload-file').change(function () {
        var el = document.getElementById('cropper');
        if (!vanilla) {
            vanilla = new Croppie(el, {
                viewport: { width: 200, height: 200, type: 'circle' },
                showZoomer: false,
            });
        }
        var imgUrl = URL.createObjectURL(document.querySelector('input[type=file]').files[0]);
        vanilla.bind({
            url: imgUrl,
        });
    })

    $('#crop-btn').click(function () {
        var settings = {
            circle: false,
            type: 'base64',
            format: 'jpeg',
            quality: 1,
            size: { width: 700, height: 700 }
        };

        vanilla.result(settings).then(function (base64) {
            $.ajax({
                type: "POST",
                url: '/personal_account/upload_avatar',
                data: { base64: base64 },
                success: function (result) {
                    if (result.error) {
                        $('.error').html(result.error);
                        ShowErrorMessage();
                    }
                    else if (result.success) {
                        $("#CurrentAvatarURLImg").attr("src", result.success);
                        input.value = "";
                    }
                },
            });
        });
    });
}

function ShowCropperDialog() {
    var dialog = document.getElementById('crop-dialog');
    if (!dialog.showModal) {
        dialogPolyfill.registerDialog(dialog);
    }
    $('#upload-file').change(function () {
        dialog.showModal();
    })
    $(document).mouseup(function (e) {
        var container = $("#crop-dialog");

        if (container.is(e.target) && dialog.showModal) {
            $('#upload-file').val('');
            dialog.close();
        }
    });
}

$(document).ready(function () {
    ConfirmChangesShow();
    ShowCropperDialog();
    CropperManage();
    AvatarPanelManage();
    AvatarBtnManage();
    ProfilePanelManage();
    $('#InputFile').change(function () {
        readURL(this);
    });
    $('input').change(function () {
        SubmitButtonManage();
        ValidatePassword();
    });
    $('#submit-btn').click(function (e) {
        e.preventDefault();
        SendChanges();
    });
});

