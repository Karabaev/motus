//
//Файл скриптов счетчика лайков видеоматериалов.
//

// Страница загрузилась.
$(document).ready(function () {
    $.getJSON("get_marks/" + $("#VideoMaterialIDHdn").val(), null, function (data) {
        DisplayMarkCount(data.PositiveMarkCount, data.NegativeMarkCount);
    }).fail(function () { alert("Error"); });
});

// Отправить лайк/дизлайк на сервер.
function SendRequest(value) {
    mark = {
        Value: value,
        VideoMaterialID: $("#VideoMaterialIDHdn").val()
    };

    $.ajax({
        method: 'post',
        data: mark,
        url: '/User/AddMark',
        success: function (data) {
            DisplayMarkCount(data.PositiveMarkCount, data.NegativeMarkCount);
        }
    });
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