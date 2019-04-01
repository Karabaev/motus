
function UsersSearch(e) {
    ClearSelected();
    $(e).addClass('selected-item');
    $.ajax({
        url: 'AdminTools/UsersSearch',
        type: 'post',
        success: function (result) {
            $('.search-area').html(result);
        }
    })
}
function FilmSearch(e) {
    ClearSelected();
    $(e).addClass('selected-item');
    $.ajax({
        url: 'AdminTools/FilmSearch',
        type: 'post',
        success: function (result) {
            $('.search-area').html(result);
        }
    })
}
function GetTable() {
    $('.search').change(function () {
        var value = $(this).val();
        if ($(this).attr("searchType") == 'film') {
            $.ajax({
                url: 'AdminTools/GetFilmsTable',
                type: 'post',
                data: { title: value },
                success: function (result) {
                    $('.search-result').html(result)
                }
            })
        };
        if ($(this).attr("searchType") == 'users') {
            $.ajax({
                url: 'AdminTools/GetUsersTable',
                type: 'post',
                data: { name: value },
                success: function (result) {
                    $('.search-result').html(result)
                }
            })
        };
    })
}
function ClearSelected() {
    $('.selected-item').removeClass('selected-item');
}


// скрипты обновления индекса Эластика
$(function () {
    $("#UpdateElasticBtn").click(function () {
        IndexElastic();
    });
});

function IndexElastic() {
    $.post("/AdminTools/UpdateElasticIndex",
        null,
        function (data) {
            alert(data.Message);
        },
        "json");
}