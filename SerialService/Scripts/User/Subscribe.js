
$(document).ready(function () {

    $.getJSON("is_subscribed/" + $("#material-id").val(),
        null,
        function (data) {
            if (data.Success) {
                if (data.Result) {
                    $("#SubscribeBtn").hide();
                    $("#UnSubscribeBtn").show();
                }
                else {
                    $("#SubscribeBtn").show();
                    $("#UnSubscribeBtn").hide();
                }
            }
            else {
                console.error(data.Message);
            }
        }).fail(function () {
            console.error('subscribed request error');
        });
});

$('#SubscribeBtn').on('click', function (e) {
    var json = { id: $("#material-id").val() };
    $.post("/User/Subscribe",
        json,
        function (data) {
            if (data.Success) {
                $("#SubscribeBtn").hide();
                $("#UnSubscribeBtn").show();
            }
            else {
                console.error(data.Message);
            }
        },
        "json");
});

$('#UnSubscribeBtn').on('click', function (e) {
    var json = { id: $("#material-id").val() };
    $.post("/User/Unsubscribe",
        json,
        function (data) {
            if (data.Success) {
                $("#SubscribeBtn").show();
                $("#UnSubscribeBtn").hide();
            }
            else {
                console.error(data.Message);
            }
        },
        "json");
});