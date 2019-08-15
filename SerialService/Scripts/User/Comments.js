$(document).ready(function () {
    getCommentsContainer();
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
                hideComments(3);
                $('#show-all-comments-ref').on('click', function (e) {
                    e.preventDefault();
                    showAllComments();
                });
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

function prepareToEditCommentText(commentId) {
    cleanNewCommentForm();
    text = $('#comment-text-' + commentId).text().trim();
    $('#comment-text-input').val(text);
    $('#comment-text-submit').text("Изменить комментарий");
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
    result = '<div id="comment-editing-status-row">';
    result += '<div>Редактирование комментария:</div>';
    result += '<button id="close-status-row-btn">X</button>';
    result += '<div>' + oldText + '</div>';
    result += '</div>';
    return result;
}

function cleanNewCommentForm() {
    removeRowStatusEditingCommentFromDOM();
    $('#comment-text-input').val('');
    $('#comment-text-submit').text("Оставить комментарий");
    $('#comment-text-submit').off();
    $('#comment-text-submit').on('click', function (e) {
        e.preventDefault();
        addComment(null);
    });
    removeRowStatusResponceCommentFromDOM();

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
        $('#close-status-row-btn').on("click", function (e) {
            e.preventDefault();
            cleanNewCommentForm();
        });
    }
}

function getRowStatusResponceCommentHTML(parentText) {
    result = '<div id="comment-responce-status-row">';
    result += '<div>Ответ на:</div>';
    result += '<button id="close-status-row-btn">X</button>';
    result += '<div>' + parentText + '</div>';
    result += '</div>';
    return result;
}

function removeRowStatusResponceCommentFromDOM() {
    $('#comment-responce-status-row').remove();
}

function hideComments(visibleCount) {
    $('#comment-container').each(function () {
        $(this).find('.comment-block').slice(visibleCount).hide();
    });


}

function showAllComments() {
    $('#comment-container').each(function () {
        $(this).find('.comment-block').show();
    });

    $('#show-all-comments-ref').hide();
}