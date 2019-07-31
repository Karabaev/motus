
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
                alert(data.Message);
            }
    }).fail(function () { alert("Error"); });
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
                alert(data.Message);
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
                alert(data.Message);
            }
        },
        "json");
});