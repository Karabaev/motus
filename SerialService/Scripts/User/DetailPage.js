﻿var sended = false;
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

var isCommentLike = false;

function createComment() {
    text = $('#comment-form').find('#new-comment-text').val();
    $('#comment-form').find('#new-comment-text').val('');
    parentId = $('#comment-form').find('#parent-comment-id').val();

    if (text == '') {
        showError('Текст коментария не может быть пустым');
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
        +   '<div class="comment-text">' + text + '</div>'
        +   '<div>+0 -0</div>'
        +   '<div><button id="comment-like-btn">+</button><button id="comment-dislike-btn">-</button></div>'
        +   '<a href="#" onclick="prepareToEditCommentText(' + commentId +')">Редактировать</a> <a href="#" onclick="removeComment(' + commentId + ')">Удалить</a>'
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

function prepareToEditCommentText(commentId) {
    text = $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.comment-text').text();
    $('#comment-form').find('#new-comment-text').val(text);
    $('#add-comment-btn').text("Изменить комментарий");
    $('#add-comment-btn').off();
    $('#add-comment-btn').on('click', function (e) {
        e.preventDefault();
        editComment(commentId);
    });
    addRowStatusEditingCommentToDOM(text);
}

function editComment(commentId) {

    text = $('#comment-form').find('#new-comment-text').val();

    if (text == '') {
        showError('Текст коментария не может быть пустым');
    }

    model = {
        CommentID: commentId,
        NewText: text
    };
    $.ajax({
        url: '/edit_comment',
        method: 'post',
        data: model,
        success: function (result) {
            if (result.success) {
                editCommentOnDOM(result.success.CommentID, result.success.NewText);
                cleanNewCommentForm();
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

function editCommentOnDOM(commentId, newText) {
    $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.comment-text').text(newText);
}

function getRowStatusEditingCommentHTML(oldText) {
    return '<div id="comment-editing-status-row"><div >Редактирование комментария:</div><button id="close-status-row-btn">X</button>'
    + '<div>' + oldText + '</div></div>';
}

function addRowStatusEditingCommentToDOM(oldText) {
    if (!$('div').is('#comment-editing-status-row')) {
        $('#comment-form').prepend(getRowStatusEditingCommentHTML(oldText));
        $('#close-status-row-btn').on("click", function (e) {
            e.preventDefault();
            cleanNewCommentForm();
        });
    } 
}

function removeRowStatusEditingCommentFromDOM() {
    $('#comment-editing-status-row').remove();
}

function cleanNewCommentForm() {
    removeRowStatusEditingCommentFromDOM();
    $('#new-comment-text').val('');
    $('#add-comment-btn').text("Оставить комментарий");
    $('#add-comment-btn').off();
    $('#add-comment-btn').on('click', function (e) {
        e.preventDefault();
        createComment();
    });
}

function showError(text) {
    $('#error-text').text(text);
    $('#error-text').show(1000, function () {
        setTimeout(function () {
            $('#error-text').hide(500);
        }, 5000);
    });
}