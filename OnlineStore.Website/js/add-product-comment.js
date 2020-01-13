$(function () {
    var $addComment = $('#AddComment'),
        productid = $('.product-view').data('productid'),
        $userName = $('#UserName'),
        $email = $('#Email'),
        $subject = $('#Subject'),
        $text = $('#Text'),
        $btn = $('.btn-comment'),
        $loading = $('.loading'),
        productname = $('.product-title h1').text();

    $loading.fadeOut();

    $btn.on('click', function () {
        if (!$addComment.valid()) {
            return;
        }
        $loading.fadeIn();

        $.ajax({
            type: 'POST',
            url: '/Products/AddComment',
            data: {
                ID: productid,
                UserName: $userName.val(),
                Email: $email.val(),
                Subject: $subject.val(),
                Text: $text.val(),
            },
            success: function (response) {
                if (response.Success) {
                    toastr.success('نظر شما برای محصول ' + productname + ' با موفقیت ارسال شد.', 'نظر محصول ' + productname);
                    clearForm();
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'نظر محصول ' + productname);
                }
            },
            error: function (response) {
                toastr.error(staticTexts.RequestError, 'نظر محصول ' + productname);
            },
            complete: function () {
                $loading.fadeOut();
            }
        });
    });

    function clearForm() {
        $userName.val('');
        $email.val('');
        $subject.val('');
        $text.val('');

    }
});