var sended = false;
var episodeData;

$(document).ready(function () {
    $('#like-btn').click(function () {
        isCommentLike = true;
    });
    $('#dislike-btn').click(function () {
        isCommentLike = false;
    });

    cleanNewCommentForm();
})

// ФУНКЦИЯ ДЛЯ СОХРАНЕНИЯ ВРЕМЕНИ ПРОСМОТРА
function onPlayerTimeUpdate(player_time) {

    timeToSend = Math.round(player_time);

    if (timeToSend % 10 === 0) { // каждые 10 секунд
        if (sended === false) {
            if (episodeData !== undefined) {
                model = {
                    TimeSec: timeToSend,
                    SeasonNumber: episodeData.season,
                    EpisodeNumber: episodeData.episode,
                    TranslatorName: episodeData.translator,
                    VideoMaterialID: $("#material-id").val()
                };
            }
            else {
                model = {
                    TimeSec: Math.round(player_time),
                    VideoMaterialID: $("#material-id").val()
                };
            }

            $.ajax({
                method: 'post',
                data: model,
                success: function (data) {
                url: '/User/SaveViewTime',
                    sended = true;
                    if (data.success) {
                       // console.log(data.success);
                    }
                    else {
                        if (data.error) {
                          //  console.error(data.error);
                        }
                    }
                }
            });
        }
    }
    else {
        sended = false;
    }
}; 

// ФУНКЦИЯ ДЛЯ СОХРАНЕНИЯ ВРЕМЕНИ ПРОСМОТРА
function mwPlayerMessageReceive(event) {
    if (event.data && event.data.message === 'MW_PLAYER_TIME_UPDATE') {
        onPlayerTimeUpdate(event.data.value);
    } else {
        if (event.data && event.data.message === 'MW_PLAYER_SELECT_EPISODE') {
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









