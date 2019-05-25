function Slide() {
    const slider = document.getElementById('recommendations');
    let leftIsPress = false;
    let startX;
    let scrollLeft;
    slider.addEventListener('mousedown', (e) => {
        leftIsPress = true;
        slider.classList.add('active');
        startX = e.pageX - slider.offsetLeft;
        scrollLeft = slider.scrollLeft;
    })
    slider.addEventListener('mouseleave', () => {
        leftIsPress = false;
        slider.classList.remove('active');
    })
    slider.addEventListener('mouseup', () => {
        leftIsPress = false;
        slider.classList.remove('active');
    })
    slider.addEventListener('mousemove', (e) => {
        if (leftIsPress) {
            e.preventDefault();
            const x = e.pageX - slider.offsetLeft;
            const trac = (x - startX) * 3;
            slider.scrollLeft = scrollLeft - trac;
        }
    })
}
var time = null;
var bar = $('#motus-info-bar');

function HideBar() {
    time = setTimeout(function () {
        bar.slideUp();
    }, 3000);
};


$('.player').on('mouseover', function (e) {
    bar.slideDown("fast");
    !time || clearTimeout(time);
    HideBar();
});

document.addEventListener('DOMContentLoaded', function () {
    function RenderPlayerInfoBar() {
        HideBar();
        Slide();
    }

    RenderPlayerInfoBar();
})

var counter = 0;
var episodeData;

// ФУНКЦИЯ ДЛЯ СОХРАНЕНИЯ ВРЕМЕНИ ПРОСМОТРА
function onPlayerTimeUpdate(player_time) {
    counter++;

    if (counter >= 59) { // получается пример каждые 15 сек если counter >= 59
        counter = 0;

        if (episodeData !== undefined) {
            model = {
                TimeSec: Math.round(player_time),
                SeasonNumber: episodeData.season,
                EpisodeNumber: episodeData.episode,
                TranslatorName: episodeData.translator,
                VideoMaterialID: $("#VideoMaterialIDHdn").val()
            };
        }
        else {
            model = {
                TimeSec: Math.round(player_time),
                VideoMaterialID: $("#VideoMaterialIDHdn").val()
            };
        }

       

        //if ("season" in episodeData) {
        //    model.SeasonNumber = episodeData.season;
        //}

        //if ("episode" in episodeData) {
        //    model.EpisodeNumber = episodeData.episode;
        //}

        //if ("translator" in episodeData) {
        //    model.TranslatorName = episodeData.translator;
        //}

        $.ajax({
            method: 'post',
            data: model,
            url: '/User/SaveViewTime',
            success: function (data) {
                if (data.success) {
                    console.log(data.success);
                }
                else {
                    if (data.error) {
                        console.error(data.error);
                    }
                }
            }
        });
    }
}; 

// ФУНКЦИЯ ДЛЯ СОХРАНЕНИЯ ВРЕМЕНИ ПРОСМОТРА
function mwPlayerMessageReceive(event) {
    if (event.data && event.data.message == 'MW_PLAYER_TIME_UPDATE') {
        onPlayerTimeUpdate(event.data.value);
    } else {
        if (event.data && event.data.message == 'MW_PLAYER_SELECT_EPISODE') {
            onPlayerEpisodeSelected(event.data.value);
        }
    }
};

// ФУНКЦИЯ ДЛЯ СОХРАНЕНИЯ ВРЕМЕНИ ПРОСМОТРА
$(function () {
    if (window.addEventListener) {
        window.addEventListener('message', mwPlayerMessageReceive);
    } else {
        window.attachEvent('onmessage', mwPlayerMessageReceive);
    }
});


function onPlayerEpisodeSelected(episode_data) {
    console.log('onPlayerEpisodeSelected', episode_data);
    episodeData = episode_data;
}

$(function () {
    if (window.addEventListener) {
        window.addEventListener('message', mwPlayerMessageReceive);
    } else {
        window.attachEvent('onmessage', mwPlayerMessageReceive);
    }
});