function MainAutocomplete() {
    var search = $('#search')
    var searchForm = search.closest('.search-form')
    var autocomplete = $('.autocomplete')
    var autocompleteValues = []

    function autocompleteRender(arr) {
        autocomplete.html('')

        arr.forEach(el => {
            autocomplete.append(`<li class='autocomplete-item'>${el}</li>`)
        })

        autocompleteItemsListeners()

        showAutocomplete()
    }

    function autocompleteItemsListeners() {
        var autoCompleteItem = $('.autocomplete-item')
        autoCompleteItem.on('click', function () {
            changeSearchValue($(this).html())
            hideAutocomplete()
        })
    }

    function showAutocomplete() {
        searchForm.addClass('autocomplete-active')
    }

    function hideAutocomplete() {
        searchForm.removeClass('autocomplete-active')
    }

    function changeSearchValue(val) {
        search.val(val)
    }

    search.keyup(function (e) {
        var query = search.val()
        $.ajax({
            method: 'post',
            url: '../../User/GetSuggest',
            data: { part: query },
            success: function (result) {
                if (result.Array) {
                    autocompleteValues = result.Array
                }
            }
        })
        if (autocompleteValues.length > 0) {
            autocompleteRender(autocompleteValues)
        }
    })

    $(document).on('click', function (e) {
        if (!$(e.target).closest('.autocomplete').length) {
            hideAutocomplete()
        };
    })
}