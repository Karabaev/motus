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
    $(".motus-film-card").hover(function () {
        var itemId = $(this).attr("id");
        ShowPopover(itemId, "po-" + itemId)
    })
}
$(document).ready(function () {
    OnHoverShow()
})