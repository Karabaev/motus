function ShowPopover(refId, popId) {
    var popup = $('#' + popId);
    var ref = $('#' + refId);
    popup.show();
    var popper = new Popper(ref, popup, {
        placement: 'right',
        modifiers: {
            flip: {
                behavior: ['right', 'left']
            },
            offset: {
                enabled: true,
                offset: '0,10'
            }
        }
    });
    ref.mouseout(function () {
        popup.hide();
    })
}
function OnHoverShow() {
    $('.motus-popover').hide();
    var setTimeoutVar;
    $(".motus-film-card").hover(function () {
        var itemId = $(this).attr("id");
        setTimeoutVar = setTimeout(function () {
            ShowPopover(itemId, "po-" + itemId);
        },800);
    }, function () {
        clearTimeout(setTimeoutVar);
    })
}
$(document).ready(function () {
    OnHoverShow()
})