function FilterButtonMange() {
    var menu = $('.not-close-menu');
    var attrName = 'disabled';
    var btn = $('#filter-btn');
    function CallBack() {
        var btnIsDisabled = btn.is(':' + attrName);
        var filterNotEmpty = CheckEmptines();
        if (btnIsDisabled && filterNotEmpty) {
            btn.removeAttr(attrName);
        }
        else if (!btnIsDisabled && !filterNotEmpty) {
            btn.attr(attrName, attrName);
        }
    }
    menu.click(CallBack);
    menu.keyup(CallBack);
    $('input').change(CallBack);
}

function CheckEmptines() {
    var selectedItemsNotEmpty = $('.selected').length > 0;
    var yearInputsNotEmpty = $('#from').val() != '' || $('#to').val() != '';
    var imdbNotEmpty = $('#imdb-input').val() > 0;
    var kinopoiskNotEmpty = $('#kinopoisk-input').val() > 0;
    var isClear = $('.filter-panel').find('.is-invalid').length === 0;
    return (selectedItemsNotEmpty || yearInputsNotEmpty
        || imdbNotEmpty || kinopoiskNotEmpty) && isClear;
}

function RenderBadge(selectedClassName) {
    var menu = $('.selectable-menu');
    menu.click(function () {
        var selectedCount = $(this).find('.' + selectedClassName).length;
        var tabId = $(this).attr('for');
        var tabSpan = $('#' + tabId).find('span');
        if (selectedCount > 0) {
            tabSpan.attr('data-badge', selectedCount)
        }
        else {
            tabSpan.removeAttr('data-badge');
        }
    })
}

function SelectFilterRow(selectedClassName) {
    var selectableItem = $('.selectable-criterion');
    const clearClassName = 'clear-selectable';
    selectableItem.click(function () {
        if ($(this).hasClass(selectedClassName)) {
            $(this).removeClass(selectedClassName);
            $(this).addClass(clearClassName);
        }
        else {
            $(this).removeClass(clearClassName);
            $(this).addClass(selectedClassName);
        }
    })
}

function SliderListen() {
    $(".rating-slider").change(function (e) {
        var rating = $(this).val();
        var displayNumber = $(this.closest('.mdl-menu')).find('.filter-rating-number');
        displayNumber.html(rating);
    });
}

function initialFilters() {
    var filters = document.querySelectorAll('.not-close-menu')
    var each = [].forEach

    each.call(filters, function (el) {
        el.addEventListener('click', function (event) {
            event.stopPropagation();
        })
    })

}

function FilterPanelManage(id) {
    const rotatedClass = 'rotated';
    var arrow = document.getElementById(id);
    var pannel = $('.filter-panel');
    var isRotated = false;
    arrow.onclick = function () {
        if (isRotated) {
            arrow.classList.remove(rotatedClass);
            pannel.slideUp();
            isRotated = false;
        }
        else {
            arrow.classList.add(rotatedClass);
            pannel.slideDown();
            isRotated = true;
        }
    }
}

function FilterDataCollect(selectedClassName) {
    var genres = MapToStringArr('.genres-item', selectedClassName);
    var countries = MapToStringArr('.country-item', selectedClassName);
    var translations = MapToStringArr('.translation-item', selectedClassName);
    var yearFrom = $('#from').val();
    var yearTo = $('#to').val();
    var kinopoiskMin = $('#kinopoisk-input').val();
    var imdbMin = $('#imdb-input').val();
    var result = {
        Genres: genres,
        Countries: countries,
        Translations: translations,
        MinImdb: imdbMin,
        MinKinopoisk: kinopoiskMin,
        MinReliseDateValue: yearFrom,
        MaxReliseDateValue: yearTo
    }
    return result;
}

function MapToStringArr(listSelector, selectedClassName) {
    return $.map($(listSelector + '.' + selectedClassName).find('span'), function (i) {
        return i.innerHTML.trim();
    })
}

function SubmitFilter(selector, selectedClassName) {
    $(selector).click(function (e) {
        e.preventDefault();
        var dataObj = FilterDataCollect(selectedClassName);
        window.location.href = '../../User/Filter?json=' + JSON.stringify(dataObj);
    })
}