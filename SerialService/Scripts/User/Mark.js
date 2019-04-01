//
//Файл скриптов счетчика лайков видеоматериалов.
//

// Страница загрузилась.
$(document).ready(function () {
    $.getJSON("/User/GetMarks/" + $("#VideoMaterialIDHdn").val(), null, function (data) {
        DisplayMarkCount(data.PositiveMarkCount, data.NegativeMarkCount);
    }).fail(function () { alert("Error"); });
});

// Отправить лайк/дизлайк на сервер.
function SendRequest(value) {
    mark = {
        Value: value,
        VideoMaterialID: $("#VideoMaterialIDHdn").val()
    };
    $.post("/User/AddMark",
        mark,
        function (data) {
            DisplayMarkCount(data.PositiveMarkCount, data.NegativeMarkCount);
        },
        "json");
}

// Привязка событий кнопок.
$(function () {
    $("#ThumbUpBtn").click(function () {
        SendRequest(true);
    });

    $("#ThumbDownBtn").click(function () {
        SendRequest(false);
    });
    $("#ThumbUpBtnShort").click(function () {
        SendRequest(true);
    });

    $("#ThumbDownBtnShort").click(function () {
        SendRequest(false);
    });
});

// Вывести лайки/дизлайки на экран.
function DisplayMarkCount(positiveCount, negativeCount) {
    $("#ThumbUpTxt").text(positiveCount);
    $("#ThumbDownTxt").text(negativeCount);
    $("#ThumbUpTxtShort").text(positiveCount);
    $("#ThumbDownTxtShort").text(negativeCount);
}