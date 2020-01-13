/// <reference path="jquery-1.11.3.min.js" />

var $sendScore = $('#SendScore'),
    productid = $('.product-details').data('productid'),
    $text = $('#SendScore #Text'),
    $btnSend = $('#SendScore .btn-submit'),
    $parameters = $('#SendScore .score-parameter'),
    $comments = $('.score-comments .score-parameter'),
    $average = $('.average .score-parameter'),
    $totalScore = $('.product-shop #totalScore'),
    $commentItems = $('.score-comments .comment-item'),
    productname = $('.product-features h1 .title-fa').text(),
    values = [],
    items = [];

getUserScores();
showAverage();
bindScoreComments();

$btnSend.on('click', function (e) {
    e.preventDefault();

    if ($text.val() == '') {
        toastr.error('لطفا متن نظر خود را وارد نمایید.', 'امتیاز محصول ' + productname);
        return;
    }
    $parameters.children('li').each(function () {
        var $this = $(this);

        items.push($this.find('select').data('id'));
        values.push($this.find('.br-current').data('rating-value'));
    });

    sendScore(items.join(','), values.join(','));

});

function clearForm() {
    $text.val('');
    $btnSend.remove();
}

function sendScore(items, values) {

    var $loading = $(staticTemplates.Loading);
    $sendScore.append($loading);

    $.ajax({
        type: 'POST',
        url: '/Products/SendScore',
        data: {
            ID: productid,
            ScoreParameters: items,
            Values: values,
            Text: $text.val(),
        },
        success: function (response) {
            if (response.Success) {
                toastr.success('امتیاز شما برای محصول ' + productname + ' با موفقیت ارسال شد.', 'امتیاز محصول ' + productname);
                clearForm();
            }
            else {
                toastr.error(staticTexts.ResponseError, 'امتیاز محصول ' + productname);
            }
        },
        error: function (response) {
            toastr.error(staticTexts.RequestError, 'امتیاز محصول ' + productname);
        },
        complete: function () {
            $loading.remove();
            getUserScores();
        }
    });
}

function getUserScores() {
    $.ajax({
        type: 'POST',
        url: '/Products/GetScoreValues',
        data: {
            ID: productid,
        },
        success: function (response) {
            if (response.Success) {
                // کاربر قبلا امتیاز ثبت کرده است
                if (response.Data != null) {

                    $text.val(response.Data.Text);
                    $text.attr('disabled', 'disabled');
                    $btnSend.remove();

                    $parameters.find('select').barrating({
                        theme: 'bars-movie'
                    });

                    for (var i = 0; i < response.Data.ScoreValues.length; i++) {
                        var item = response.Data.ScoreValues;
                        var $param = $parameters.find('#Parameter' + item[i].ScoreParameterID);
                        $param.barrating('set', item[i].Rate);
                        $param.barrating('readonly', true);
                    }
                }
                else {
                    $parameters.find('select').barrating({
                        theme: 'bars-movie',
                        readonly: false
                    });
                }
            }
            else {
                toastr.error(staticTexts.ResponseError, 'امتیاز محصول ' + productname);
            }
        },
        error: function (response) {
            toastr.error(staticTexts.RequestError, 'امتیاز محصول ' + productname);
        },
    });
}

function bindScoreComments() {

    $comments.find('select').barrating({
        theme: 'bars-movie',
        readonly: true
    });

    $comments.find('select').each(function () {
        var $param = $(this);
        $param.barrating('set', $param.data('rate'));
    });
}

function showAverage() {
    $average.find('select').barrating({
        theme: 'bars-movie',
        readonly: true
    });

    $average.find('select').each(function () {
        var $param = $(this);
        var rate = $param.data('rate');

        $param.barrating('set', parseInt(rate));
        $param.find('+div div.br-current-rating').text(rate);
    });
}

$commentItems.on('click', '#btnLike', function () {
    if (!isAuthenticated) {
        toastr.warning('کاربر گرامی، جهت امتیازدهی لطفا <a href="/Login">وارد سایت</a> شوید.', 'ارزیابی نظر ' + productname);
        return;
    }

    var $this = $(this);

    var commentID = $this.closest('.comment-rates').data('id');
    var commentItem = $this.closest('.comment-item');

    evaluationProductComment(commentID, true, commentItem);
});

$commentItems.on('click', '#btnDisLike', function () {
    if (!isAuthenticated) {
        toastr.warning('کاربر گرامی، جهت امتیازدهی لطفا <a href="/Login">وارد سایت</a> شوید.', 'ارزیابی نظر ' + productname);
        return;
    }

    var $this = $(this);

    var commentID = $this.closest('.comment-rates').data('id');
    var commentItem = $this.closest('.comment-item');

    evaluationProductComment(commentID, false, commentItem);
});

// دریافت تعداد Like , DisLike
function evaluationProductComment(commentID, likeClick, commentItem) {

    var $loading = $(staticTemplates.Loading);
    commentItem.append($loading);

    $.ajax({
        type: 'POST',
        url: '/Products/EvaluationProductComment',
        data: {
            CommentID: commentID,
            IsLikeClick: likeClick
        },
        success: function (response) {
            if (response.Success) {
                var element = $commentItems.find('div[data-id=' + commentID + ']');
                element.find('span.like-count').text(response.Data.LikesCount);
                element.find('span.dislike-count').text(response.Data.DisLikesCount);
            }
            else {
            }
        },
        error: function (response) {
            toastr.error(staticTexts.RequestError, 'ارزیابی نظر ' + productname);
        },
        complete: function () {
            $loading.remove();
        }
    });
}