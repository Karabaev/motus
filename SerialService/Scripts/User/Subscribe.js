
$(document).ready(function () {

    $.getJSON("/User/IsSubscribed/" + $("#VideoMaterialIDHdn").val(),
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
    var json = { id: $("#VideoMaterialIDHdn").val() };
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
    var json = { id: $("#VideoMaterialIDHdn").val() };
    $.post("/User/UnSubscribe",
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