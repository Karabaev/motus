$("#close-redactor-menu").click(function () {
    document.querySelector("#for-redactor-modal").close();
});

function InputChecking() {
    if ($(".is-invalid:not(#url-for-kp)").length == 0) {
        $("#send-film-form").prop("disabled", false)
    }
    else {
        $("#send-film-form").prop("disabled", true)
    }
}

$("input").change(function () {
    InputChecking();
})
$("#send-film-form").click(function () {
    if ($(".is-invalid:not(#url-for-kp)").length == 0) {
        ShowLoader();
        var obj = GenerateFilmObj();
        $.ajax({
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(obj),
            url: "../RedactorTools/CreateNewVideoItem",
            success: function (result) {
                CloseLoader();
                if (!result.error) {
                    CleanForm();
                }
                alert(result.message)
            }
        })
    }
    else {
        alert("Ошибка валидации. Необходимо корректно заполнить выделенные поля.")
    }
})