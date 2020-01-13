/// <reference path="jquery-1.11.3.min.js" />
$(function () {
    var $sendMessage = $('#SendMessage'),
        $fullName = $("#FullName"),
        $email = $("#Email"),
        $subject = $("#Subject"),
        $message = $("#Message"),
        $btn = $("#SendMessage .btn-submit"),
        $messageBox = $('.message-box');

    $btn.on("click", function () {
        if (!$sendMessage.valid()) {
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $sendMessage.append($loading);

        $.ajax({
            type: "POST",
            url: "/ContactUs/SendMessage",
            data: {
                FullName: $fullName.val(),
                Email: $email.val(),
                Subject: $subject.val(),
                Message: $message.val(),
            },
            success: function (response) {
                if (response.Success) {
                    toastr.success('پیام شما با موفقیت ارسال شد.', 'تماس با ما');
                    $sendMessage.hide();
                    $messageBox.removeClass('hide');
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'تماس با ما');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'تماس با ما');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });
});