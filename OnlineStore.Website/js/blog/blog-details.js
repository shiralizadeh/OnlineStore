$(function () {
    var $addComment = $('#AddComment'),
        $id = $('.post-details').data('articleid'),
        $userName = $('#UserName'),
        $email = $('#Email'),
        $subject = $('#Subject'),
        $text = $('#Text'),
        $btn = $('.btn-comment'),
        $rate = $('.post-details .rate-comment .rate'),
        $totalScore = $('.post-details .rate-comment .total-score'),
        $countScore = $('.post-details .rate-comment .count-score');

    makeRaty();

    $btn.on('click', function (e) {
        e.preventDefault();
        if (!$addComment.valid()) {
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $addComment.append($loading);

        $.ajax({
            type: 'POST',
            url: '/Blog/Posts/AddComment',
            data: {
                ID: $id,
                UserName: $userName.val(),
                Email: $email.val(),
                Subject: $subject.val(),
                Text: $text.val(),
            },
            success: function (response) {
                if (response.Success) {
                    toastr.success('نظر شما با موفقیت ارسال شد.', 'ارسال نظر');
                    clearForm();
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'ارسال نظر');
                }
            },
            error: function (response) {
                toastr.error(staticTexts.RequestError, 'ارسال نظر');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });

    function clearForm() {
        $userName.val('');
        $email.val('');
        $subject.val('');
        $text.val('');

    }

    function makeRaty() {
        $rate.raty({
            half: true,
            readOnly: false,
            score: function () {
                return $(this).attr('data-score');
            },
            click: function (rate) {
                var $this = $(this);
                var $loading = $(staticTemplates.Loading);
                $this.append($loading);

                $.ajax({
                    type: 'POST',
                    url: '/Blog/Posts/InsertRate',
                    data: {
                        ArticleID: $id,
                        Rate: rate
                    },
                    success: function (result) {
                        if (result.Success) {
                            toastr.success('با تشکر از امتیاز شما', 'امتیازدهی');
                            $countScore.text(result.Data.CountScore);
                            $totalScore.text(result.Data.TotalScore);
                        }
                        else if (result.Errors[0] == "Repeat") {
                            toastr.error('قبلا برای این مطلب امتیاز ثبت کرده اید! ', 'امتیازدهی');
                        }
                    },
                    error: function (response) {
                        toastr.error(staticTexts.RequestError, 'امتیازدهی');
                    },
                    complete: function () {
                        $loading.remove();
                        $rate.raty({
                            readOnly: true,
                            score: function () {
                                return rate;
                            }
                        });
                    }
                });
            }
        });
    }

    $('#RelatedPosts').each(function () {
        $(this).owlCarousel({
            autoplay: true,
            loop: true,
            items: 1,
            smartSpeed: 450,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 2
                },
                1280: {
                    items: 4
                },
                1440: {
                    items: 5
                }
            }
        });
    });

});