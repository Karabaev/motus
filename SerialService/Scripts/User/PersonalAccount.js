// Привязка событий кнопок.
$(function () {
    $("#SaveChangesBtn").click(function () {
        SendRequest();
    });
});

//Генерация json. 
function GenerateModelJson() {
   // alert($("#NewPublicNameTxt").val());
    var json = {
        ID: $("#AccountIDHdn").val(),
        NewPublicName: $("#NewPublicNameTxt").val(),
        NewPassword: $("#NewPasswordTxt").val(),
        ConfirmPassword: $("#ConfirmPasswordTxt").val(),
        NewParole: $("#NewParoleTxt").val(),
        NewEmail: $("#NewEmailTxt").val(),
        CurrentPassword: $("#CurrentPasswordTxt").val()
    };
    return json;
}

function SendRequest() {
    var json = GenerateModelJson();

    if (json.NewPassword !== "" && json.NewPassword !== json.ConfirmPassword) {
        alert("Пароли не совпадают!");
    }
    $.post("/User/PersonalAccountSaveChanges",
        json,
        function (data) {
            DisplayData(data);
        },
        "json");
}

//вывод информации на экран
function DisplayData(json) {
    if (json.Success) {
        $("#CurrentUserNameDiv").text(json.CurrentUserName);
        $("#CurrentPublicNameDiv").text(json.CurrentPublicName);
        $("#CurrentEmailDiv").text(json.CurrentEmail);
        $("#CurrentAvatarURLImg").text(json.CurrentAvatarURL);
    }
    alert(json.Message);
}

//загрузка аватарки
$('#UploadBtn').on('click', function (e) {
    e.preventDefault();
    var fileInput = document.getElementById('InputFile');
    var files = fileInput.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }
            $.ajax({
                type: "POST",
                url: '/User/UploadAvatar',
                contentType: false,
                processData: false,
                data: data,
                success: function (result) {
                    if (result.Success) {
                        $("#CurrentAvatarURLImg").attr("src", result.AvatarPath);
                        fileInput.value = "";
                    }
                    else {
                        alert(result.Message);
                    }
                },
                error: function (xhr, status, p3) {
                    alert(xhr.responseText);
                }
            });
        }
        else {
            alert("Браузер не поддерживает загрузку файлов HTML5!");
        }
    }
});



