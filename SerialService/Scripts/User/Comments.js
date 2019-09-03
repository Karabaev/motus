$(document).ready(function () {
    getCommentsContainer();
    $('#comment-form-selector-toggle').on('click', function (e) {
        toggleNewCommentBlock();
    });
});

function getCommentsContainer() {
    data = {
        videoMaterialId: $('#material-id').val()
    };

    $.ajax({
        url: '/comments',
        method: 'get',
        data: data,
        success: function (result) {
            if (result) {
                $('#comments').html(result);
                $('#comment-text-submit').off();
                $('#comment-text-submit').on('click', function (e) {
                    e.preventDefault();
                    addComment(null);
                });
                componentHandler.upgradeDom();
                readmoreSelected($('#comment-container [id^="comment-text-"]'), 90);  
                readmoreSelected($('.motus-comment-responce-parent-quote__text'), 50); 
            }
        },
        error: function (jqxhr, status, errorMsg) {
            console.error(status + ' | ' + errorMsg + ' | ' + jqxhr);
        }
    });
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
    $('#positive-vote-count-' + commentId).text(pos);
    $('#negative-vote-count-' + commentId).text(neg);
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
                getCommentsContainer();
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

function addComment(parentId) {
    text = $('#comment-text-input').val();

    if (text === '') {
        showError('Текст коментария не может быть пустым');
        return;
    }

    if (text.length > 350) {
        showError('Текст коментария не может быть длиннее 350 символов');
        return;
    }

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
                getCommentsContainer();
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

function editComment(commentId) {

    text = $('#comment-text-input').val();

    if (text === '') {
        showError('Текст коментария не может быть пустым');
        return;
    }

    if (text.length > 350) {
        showError('Текст коментария не может быть длиннее 350 символов');
        return;
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
                getCommentsContainer();
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

function prepareToEditCommentText(commentId) {
    cleanNewCommentForm();
    showNewCommentBlock();
    text = $('#comment-text-' + commentId).text().trim();
    $('#comment-text-input').val(text);
    $('#comment-text-submit').text("Изменить");
    $('#comment-text-submit').off();
    $('#comment-text-submit').on('click', function (e) {
        e.preventDefault();
        editComment(commentId);
    });
    addRowStatusEditingCommentToDOM(text);
}

function addRowStatusEditingCommentToDOM(oldText) {
    if (!$('div').is('#comment-editing-status-row')) {
        $('#comment-form-block').prepend(getRowStatusEditingCommentHTML(oldText));
        readmoreSelected($('#edit-old-text'), 90);
        $('#close-status-row-btn').on("click", function (e) {
            e.preventDefault();
            cleanNewCommentForm();
        });
    }
}

function removeRowStatusEditingCommentFromDOM() {
    $('#comment-editing-status-row').remove();
}

function getRowStatusEditingCommentHTML(oldText) {
    result = '<div id="comment-editing-status-row" class="mdl-grid mdl-cell--12-col-desktop">';
    result += '<div class="mdl-cell mdl-cell--3-col-desktop">Исходный текст:</div>';
    result += '<div class="mdl-cell mdl-cell--8-offset-desktop mdl-cell--1-col-desktop">';
    result += '<a id ="close-status-row-btn" class="mdl-button mdl-button--icon mdl-button--colored">';
    result += '<i class="material-icons">close</i>';
    result += '</a>';
    result += '</div>';
    result += '<div id="edit-old-text" class="mdl-card__supporting-text"><p>' + oldText + '</p></div>';
    result += '</div>';
    return result;
}

function cleanNewCommentForm() {
    removeRowStatusEditingCommentFromDOM();
    $('#comment-text-input').val('');
    $('#comment-text-submit').text("Отправить");
    $('#comment-text-submit').off();
    $('#comment-text-submit').on('click', function (e) {
        e.preventDefault();
        addComment(null);
    });
    removeRowStatusResponceCommentFromDOM();
    $('#comment-form-block .readmore-ref').remove();  

}

function showError(text) {
    $('#error-text').text(text);
    $('#error-text').show(1000, function () {
        setTimeout(function () {
            $('#error-text').hide(500);
        }, 5000);
    });
}

function prepareResponceCommentText(parentCommentId) {
    cleanNewCommentForm();
    showNewCommentBlock();
    parentText = $('#comment-text-' + parentCommentId).text().trim();
    $('#comment-text-submit').text("Ответить");
    $('#comment-text-submit').off();
    $('#comment-text-submit').on('click', function (e) {
        e.preventDefault();
        addComment(parentCommentId);
    });
    addRowStatusResponceCommentToDOM(parentText);
}

function addRowStatusResponceCommentToDOM(oldText) {
    if (!$('div').is('#comment-responce-status-row')) {
        $('#comment-form-block').prepend(getRowStatusResponceCommentHTML(oldText));
        readmoreSelected($('#responce-parent-text'), 90);
        $('#close-status-row-btn').on("click", function (e) {
            e.preventDefault();
            cleanNewCommentForm();
        });
    }
}

function getRowStatusResponceCommentHTML(parentText) {
    result = '<div id="comment-responce-status-row" class="mdl-grid mdl-cell--12-col-desktop">';
    result += '<div class="mdl-cell mdl-cell--2-col-desktop">В ответ на:</div>';
    result += '<div class="mdl-cell mdl-cell--9-offset-desktop mdl-cell--1-col-desktop">';
    result += '<a id ="close-status-row-btn" class="mdl-button mdl-button--icon mdl-button--colored">';
    result += '<i class="material-icons">close</i>';
    result += '</a>';
    result += '</div>';
    result += '<div id="responce-parent-text" class="mdl-card__supporting-text"><p>' + parentText + '</p></div>';
    result += '</div>';
    return result;
}

function removeRowStatusResponceCommentFromDOM() {
    $('#comment-responce-status-row').remove();
}

function showNewCommentBlock() {
    $('#comment-form-block').show(0, function () {
        $('#comments').addClass('mdl-cell--3-offset');
    });
}

function toggleNewCommentBlock() {
    $('#comment-form-block').toggle(0, function () {
        if ($('#comment-form-block').is(':visible')) {
            $('#comments').addClass('mdl-cell--3-offset');
        } else {
            $('#comments').removeClass('mdl-cell--3-offset');
        }
    });
}

function readmoreSelected(selected, maxHeight) {
    selected.readmore({
        maxHeight: maxHeight,
        speed: 200,
        moreLink: '<a href="#" class="readmore-ref mdl-cell--4-offset-desktop mdl-cell--4-desktop">Просмотреть полностью</a>',
        lessLink: '<a href="#" class="readmore-ref mdl-cell--5-offset-desktop mdl-cell--3-desktop">Свернуть</a>'
    });
}