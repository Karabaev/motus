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
 //   $('#comment-container').find('input[type="hidden"][value="' + commentId + '"]').siblings('.votes-count').text('+' + pos + ' -' + neg);
}