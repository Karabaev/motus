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

function addComment() {
    text = $('#comment-text-input').val();

    if (text === '') {
        showError('Текст коментария не может быть пустым');
    }

    model = {
        VideoMaterialID: $('#material-id').val(),
        Text: text,
        ParentID: $('#new-comment-parent-id').val()
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