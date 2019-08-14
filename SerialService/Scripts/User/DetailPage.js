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

window.addEventListener('DOMContentLoaded', function (e) {
    SetPlayerWindowSize();
});


/// БЛОК ФУНКЦИЙ ДЛЯ КОММЕНТОВ

function removeNoCommentsMessage() {
    $('#no-comments-message').remove();
}

function getCommentHTML(userName, dateTime, text, commentId, parentUserName, parentText) {

    result = '<li>'
    result += '<input type="hidden" value="' + commentId + '"/>'
    result += '<div class="comment-header">'
    result += '<span>' + dateTime + '</span> '
    result += '<span class="comment-author-name">' + userName + '</span>'
    result += '</div>';

    if (parentUserName !== '' && parentText !== '' && parentUserName !== null && parentText !== null) {
        result += '<div>Ответ на:</div>';
        result += '<div><bold>' + parentUserName + ': </bold>"' + parentText + '"</div>';
    }

    result += '<div class="comment-text">' + text + '</div>'
    result += '<div class="votes-count">+0 -0</div>'
    result += '<div><button id="comment-like-btn" onclick="voteForComment(' + commentId + ', true)">+</button><button id="comment-dislike-btn" onclick="voteForComment(' + commentId + ', false)">-</button></div>'
    result += '<a href="#" onclick="prepareToEditCommentText(' + commentId +')">Редактировать</a> <a href="#" onclick="removeComment(' + commentId + ')">Удалить</a>'
    result += '<br/>'
    result += '<a href="#" value="' + commentId + '" onclick="prepareResponceCommentText(' + commentId + ')">Ответить</a>'
    result += '</li>';

    return result;
}

function addNoCommentsMessage() {
    if (!$('div').is('#no-comments-message')) {
        html = '<div id="no-comments-message">Еще никто не поделися своим мнением</div>';
        $('#comments-header').after(html);
    }


}



//function editCommentOnDOM(commentId, newText) {
//    $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.comment-text').text(newText);
//}













