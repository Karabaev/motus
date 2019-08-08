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

function addCommentToDOM(userName, dateTime, text, commentId, parentUserName, parentText) {
    removeNoCommentsMessage();
    $('#comment-container').append(getCommentHTML(userName, dateTime, text, commentId, parentUserName, parentText));
}

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

    if ($('#comment-container li').length == 0) {
        addNoCommentsMessage();
    }
}

function addNoCommentsMessage() {
    if (!$('div').is('#no-comments-message')) {
        html = '<div id="no-comments-message">Еще никто не поделися своим мнением</div>';
        $('#comments-header').after(html);
    }


}

function prepareToEditCommentText(commentId) {
    text = $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.comment-text').text().trim();
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

function voteForComment(commentId, isPos) {
    model = {
        CommentID: commentId,
        Value: isPos
    };

    $.ajax({
        url: '/vote_comment',
        data: model,
        method: 'post',
        success: function (result) {
            if (result.success) {
                updateVoteCountOnDOM(commentId, result.success.PositiveMarkCount, result.success.NegativeMarkCount);
            } else if (result.error) {
                console.error(result.error);
                showError(result.error);
            }
        },
        error: function (jqxhr, status, errorMsg) {
            console.error(status + " | " + errorMsg + " | " + jqxhr);
        }
    });
}

function updateVoteCountOnDOM(commentId, pos, neg) {
    $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.votes-count').text('+' + pos + ' -' + neg);
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
    $('#parent-comment-id').val('');
    removeRowStatusResponceCommentFromDOM();

}

function prepareResponceCommentText(parentCommentId) {
    parentText = $('#comment-container').find('input[type="hidden"][value="' + parentCommentId + '"]').siblings('.comment-text').text();

   // $('#comment-form').find('#new-comment-text').val(authorName + ', ');
    addRowStatusResponceCommentToDOM(parentText);
    $('#comment-form').find('#parent-comment-id').val(parentCommentId);
}

function getRowStatusResponceCommentHTML(parentText) {
    return '<div id="comment-responce-status-row"><div >Ответ на:</div><button id="close-status-row-btn">X</button>'
        + '<div>' + parentText + '</div></div>';
}

function addRowStatusResponceCommentToDOM(oldText) {
    if (!$('div').is('#comment-responce-status-row')) {
        $('#comment-form').prepend(getRowStatusResponceCommentHTML(oldText));
        $('#close-status-row-btn').on("click", function (e) {
            e.preventDefault();
            cleanNewCommentForm();
        });
    }
}

function removeRowStatusResponceCommentFromDOM() {
    $('#comment-responce-status-row').remove();
}

function showError(text) {
    $('#error-text').text(text);
    $('#error-text').show(1000, function () {
        setTimeout(function () {
            $('#error-text').hide(500);
        }, 5000);
    });
}