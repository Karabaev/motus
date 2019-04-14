function SelectSearchRow() {
    var searchSelector = '.mdh-expandable-search>input';
    var activeClassName = 'active-search-row';
    $(searchSelector).click(function () {
        if ($(this).hasClass(activeClassName)) {
            return;
        }
        $(this).addClass()
    })
    $(window).click(function (event) {
        event.stopPropagation();
        if ($(searchSelector).hasClass(activeClassName)) {
            $(searchSelector).removeClass(activeClassName)
        }
    });
}

$(document).ready(function () {
    const selectedClassName = 'selected';
    RenderBadge(selectedClassName);
    SelectFilterRow(selectedClassName);
    SelectFilterRow();
    SliderListen();
    initialFilters();
    SubmitFilter('#filter-btn', selectedClassName);
    FilterButtonMange();
    SelectSearchRow();
    MainAutocomplete();
    $('.mdh-toggle-search').click(function () {
        if ($(this).find('i').text() === 'search') {
            $(this).find('i').text('cancel');
            $(this).removeClass('mdl-cell--hide-desktop');

            $('.mdl-layout-title,.mdl-layout-spacer,.mdl-navigation').hide();
            $('.mdl-layout__header-row').css('padding-left', '16px');
            $('.mdh-expandable-search').removeClass('mdl-cell--hide-phone mdl-cell--hide-tablet').css('margin', '0 16px 0 0');

        }
        else {
            $(this).find('i').text('search');
            $(this).addClass('mdl-cell--hide-desktop');

            $('.mdl-layout-title,.mdl-layout-spacer,.mdl-navigation').show();
            $('.mdl-layout__header-row').css('padding-left', '');
            $('.mdh-expandable-search').addClass('mdl-cell--hide-phone mdl-cell--hide-tablet').css('margin', '0 50px');
        }
    });
    FilterPanelManage('exp-filter-arrow');
});

function Base64Encode(str, encoding = 'utf-8') {
    var bytes = new (TextEncoder || TextEncoderLite)(encoding).encode(str);
    return base64js.fromByteArray(bytes);
}