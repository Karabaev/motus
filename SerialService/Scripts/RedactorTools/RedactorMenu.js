const nameRegx = /^[А-Я,A-Z.'-]+\s?[А-Я,A-Z.'-]+/ui;
const urlRegx = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;
const seasonRegx = /^[А-Я,A-Z.'-]+\s?[А-Я,A-Z.'-]+\s?[0-9]/ui;
function ParseRequestAsync() {
    $("#form-for-parse").submit(function (e) {
        ShowLoader();
        e.preventDefault();
        $.ajax({
            type: "POST",
            dataType: "json",
            data: { kinopoiskId: $('#url').val() },
            url: "../RedactorTools/Parse",
            success: function (result) {
                CloseLoader();
                if (result && result.error) {
                    alert(result.message);
                }
                else {
                    AutoFilling(result);
                    $(".kpparser").removeClass("is-active");
                    $(".manual").addClass("is-active");
                    InputChecking();
                }
            }
        });
    })
}

function SaveUpdateMaterial(id) {
    $('#save').click(function (e) {
        ShowLoader();
        e.preventDefault();
        $.ajax({
            type: "POST",
            data: {
                id: id,
                model: GenerateFilmObj(),
            },
            url: "../RedactorTools/EditMaterial",
            success: function (result) {
                CloseLoader();
                if (result) {
                    alert(result.message);
                }
                else {
                    alert("Ошибка")
                }
            }
        });
    })
}
function AutoFilling(someObj) {
    $("#title").val(someObj.Title);
    $("#origin-title").val(someObj.OriginalTitle);
    $("#kinopoisk-id").val(someObj.KinopoiskID);
    $("#year").val(someObj.ReleaseDate);
    $("#poster").val(someObj.PosterHref);
    $("#duration").val(someObj.Duration);
    $("#idmb").val(typeof someObj.IDMB=="string"?someObj.IDMB.replace(',','.'):someObj.IDMB);
    $("#tagline").val(someObj.Tagline);
    $("#kinopoisk-rating").val(typeof someObj.KinopoiskRating == "string" ?
        someObj.KinopoiskRating.replace(',', '.') : someObj.KinopoiskRating);
    someObj.Actors.forEach(function (item) {
        AddChip("#actors-list", nameRegx, item);
    });
    someObj.Genres.forEach(function (item) {
        AddChip("#genre-list", nameRegx, item);
    });
    someObj.Countries.forEach(function (item) {
        AddChip("#countries-list", nameRegx, item);
    });
    someObj.FilmMakers.forEach(function (item) {
        AddChip("#filmMakers-list", nameRegx, item);
    });
    if (someObj.Pictures) {
        someObj.Pictures.forEach(function (item) {
            AddChip("#pictures-list", nameRegx, item);
        });
    };    
    if (someObj.Themes) {
        someObj.Themes.forEach(function (item) {
            AddChip("#themes-list", nameRegx, item);
        });
    };
    if (someObj.Text) {
        $("#text").val(someObj.Text)
    }
    $(".is-invalid, .film-info-field").each(function (item) {
        $(this).removeClass("is-invalid is-upgraded");
        $(this).addClass("is-dirty");
    })
};
function AddChip(listId, regx, value) {
    var id = GetHashCode(value);
    if (value != '' && value != undefined && NameValidation(value, regx)) {
        $(listId).find(' > li:nth-last-child(1)').before('<li id="' + id + '"><span class="mdl-chip mdl-chip--deletable"><span class="mdl-chip__text">' + value + '</span><button type="button" onclick = "Remove(\'' + id + '\')" class="mdl-chip__action"><i class="material-icons">cancel</i></button></span></li>');
    }
}
function AddChipFromInput(listId, itemId, regx) {
    var value = $(itemId).val();
    AddChip(listId, regx, value);
    $(itemId).val("");
}
function Remove(id) {
    $('#' + id).remove();
}
function NameValidation(text, regx) {
    return regx.test(text)
}
function GetHashCode(str) {
    return str.split("").reduce(function (a, b) { a = ((a << 5) - a) + b.charCodeAt(0); return a & a }, 0);
}
function DeletableChips(listId, itemId, regx) {
    $(listId).keyup(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            AddChipFromInput(listId, itemId, regx);
        }
    });
    $(listId).keydown(function (event) {
        var thisInput = $(listId).find('.chip-input').first();
        var lastItem = $(listId).find(' > li:nth-last-child(2)').attr('id');
        if (thisInput.val() == '' && event.keyCode == 8) {
            Remove(lastItem);
        }
    })
}
function GenerateFilmObj() {
    var obj = {
        "Title": $("#title").val(),
        "OriginalTitle": $("#origin-title").val(),
        "PosterHref": $("#poster").val(),
        "KinopoiskID": $("#kinopoisk-id").val(),
        "ReleaseDate": $("#year").val(),
        "Duration": $("#duration").val(),
        "IDMB": $("#idmb").val(),
        "Actors": GetAllItems("#actors-list"),
        "Genres": GetAllItems("#genre-list"),
        "Countries": GetAllItems("#countries-list"),
        "Translations": GetAllItems("#translation-list"),
        "Pictures": GetAllItems("#pictures-list"),
        "FilmMakers": GetAllItems("#filmMakers-list"),
        "Themes": GetAllItems("#themes-list"),
        "Text": $("#text").val(),
        "KinopoiskRating": $("#kinopoisk-rating").val(),
        "Tagline": $("#tagline").val(),
    }
    return obj;
}
function GetAllItems(listId) {
    var result = [];
    var list = $(listId).find('li')
    list.each(function () {
        if ($(this).attr('class') != "chip-input-item") {
            var text = $(this).find(".mdl-chip").first().find(".mdl-chip__text").text()
            result.push(text)
        }
    })
    return result;
}

function CleanForm() {
    var allInputs = $('input')
    allInputs.each(function () {
        if ($(this).attr('type') != 'submit') {
            this.value = "";
        }
    })
    var allLists = $('.mdl-list')
    allLists.each(function () {
        $(this).find('li').each(function () {
            if ($(this).attr('class') != 'chip-input-item') {
                $(this).remove();
            }
        })
    })
    $(".is-dirty").each(function (item) {
        $(this).removeClass("is-dirty");
        $(this).addClass("is-invalid is-upgraded");
    })
    $("#send-film-form").prop("disabled", true);
}

$(document).ready(function () {
    ParseRequestAsync();
    DeletableChips("#actors-list", "#actor-name", nameRegx);
    DeletableChips("#genre-list", "#genre", nameRegx);
    DeletableChips("#countries-list", "#country", nameRegx);
    DeletableChips("#translation-list", "#translation", nameRegx);
    DeletableChips("#pictures-list", "#picture", urlRegx);
    DeletableChips("#filmMakers-list", "#filmMaker", nameRegx);
    DeletableChips("#themes-list", "#theme", nameRegx);
    DeletableChips("#seasons-list", "#season", seasonRegx);
})