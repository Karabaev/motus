function GetGroupList() {
    var filters = []
    $('.group').each(function () {
        if ($(this).is(':checked')) {
            filters.push($(this).attr("group"))
        }
    });
    return filters;
}
function GroupRequest(selector) {
    $(selector).change(function () {
        $.ajax({
            type: "GET",
            url: "GetSortTable",
            data: {
                filters: JSON.stringify(GetGroupList()),
                searchFilter: $('#search').val()
            },
            success: function (result) {
                $('#item-table').html(result);
            }
        })
    })
}
function SearchRequest(selector) {
    $(selector).change(function () {
        var serchParam = $(this).val();
        $.ajax({
            type: "GET",
            url: "GetSortTable",
            data: { searchFilter: serchParam, filters: JSON.stringify(GetGroupList()) },
            success: function (result) {
                $('#item-table').html(result);
            }
        })
    })
}

function SortRequest(selector) {
    $(selector).click(function () {
        var sortType = $(this).attr("name");
        $.ajax({
            type: "GET",
            url: "GetSortTable",
            data: {
                sortOrder: sortType,
                searchFilter: $('#search').val(),
                filters: JSON.stringify(GetGroupList())
            },
            success: function (result) {
                $('#item-table').html(result);
            }
        })
    })
}
function GoToPage(selector, isInModeratorPermition) {
    $(selector).click(function () {
        var status = $(this).attr('status');
        if (status == 'confirmed' && !isInModeratorPermition) {
            window.location.href = '../../user/VideoMaterialDetailPage/' + $(this).attr("baseid")
        }
        else {
            window.location.href = '../RedactorTools/FilmEditor?id=' + $(this).attr("baseid")
        }
    })
};