var sended = false;
var episodeData;

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

var isCommentLike = false;

$(document).ready(function (){
    $('#like-btn').click(function () {
        isCommentLike = true;
    });
    $('#dislike-btn').click(function () {
        isCommentLike = false;
    });
})

function Vote() {
    var formObj = $('#comment-vote-form').serialize()
    $.ajax({
        method: 'post',
        data: formObj,
        url: 'vote_for_comment',
        statusCode: {
            200: function () {
                alert("Коммент добавлен");
            },
            500: function () {
                alert("Коммент не добавлен");
            }
        }
    })
}

$('#add-comment-btn').on('click', function (e) {
    e.preventDefault();
    createComment();
});

function createComment() {
    text = $('#comment-form').find('#new-comment-text').val();
    $('#comment-form').find('#new-comment-text').val('');
    parentId = $('#comment-form').find('#parent-comment-id').val();

    if (text == '') {
        showError('Текст коментария пустой');
    }

    addComment(parentId, text);
}

function addComment(parentId, text) {
    model = {
        VideoMaterialID: $('#material-id').val(),
        Text: text,
        ParentID: parentId
    };
    $.ajax({
        url: '/add_comment',
        method: 'post',
        data: model,
        success: function (result) {
            if (result.success) {
                object = result.success;
                addCommentToDOM(object.UserName, object.CreateDateTime, object.Text, object.CommentID,
                                object.ParentAuthor, object.ParentText);
            }
            else if (result.error) {
                console.error(result.error);
                showError(result.error);
            }
        },
        error: function (jqxhr, status, errorMsg) {
            console.error(status + " | " + errorMsg + " | " + jqxhr); 
        }
    });
}

function addCommentToDOM(userName, dateTime, text, commentId, parentUserName, parentText) {
    $('#comment-container').append(getCommentHTML(userName, dateTime, text, commentId, parentUserName, parentText));
}

function getCommentHTML(userName, dateTime, text, commentId, parentUserName, parentText) {
    return  '<li>' 
        +   '<input type="hidden" value="' + commentId + '"/>'
        +   '<div>' + dateTime + ' ' + userName + '</div>'
        +   '<div>' + text + '</div>'
        +   '<div>+0 -0</div>'
        +   '<div><button id="comment-like-btn">+</button><button id="comment-dislike-btn">-</button></div>'
        +   '<a href="#">Редактировать</a> <a href="#" onclick="removeComment(' + commentId + ')">Удалить</a>'
        +   '<br/>'
        +   '<a href="#" value="' + commentId +'">Ответить</a>'
        +   '</li>';
}

function removeComment(commentId) {
    model = {
        CommentID: commentId
    };

    $.ajax({
        url: "/remove_comment",
        method: "post",
        data: model,
        success: function (result) {
            if (result.success) {
                removeCommentFromDOM(commentId);
            }
            else if (result.error) {
                console.error(result.error);
                showError(result.error);
            }
        },
        error: function (jqxhr, status, errorMsg) {
            console.error(status + " | " + errorMsg + " | " + jqxhr);
        }
    });
}

function removeCommentFromDOM(commentId) {
    $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').closest('li').remove();
}

function showError(text) {
    $('#error-text').text(text);
}