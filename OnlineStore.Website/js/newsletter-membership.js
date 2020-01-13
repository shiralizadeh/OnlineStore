$(function () {
    var $newsletterEmail = $('.newsletter-box input'),
        $newsBox = $('.newsletter-box'),
        $btnJoin = $('.newsletter-box a');

    $btnJoin.on('click', function (e) {
        e.preventDefault();

        if ($newsletterEmail.val() == '') {
            toastr.error('لطفا ایمیل یا شماره موبایل خود را وارد نمایید.');
            return false;
        }

        if (!validateEmail($newsletterEmail.val()) && !validateMobile($newsletterEmail.val())) {
            toastr.error('فرمت نامعتبر است.');
            return false;
        }

        var $loading = $(staticTemplates.Loading);
        $newsBox.append($loading);

        $.ajax({
            type: 'POST',
            url: '/NewsLetterMembership/AddMember',
            data: {
                Email: $newsletterEmail.val(),
            },
            success: function (response) {
                if (response.Success) {
                    toastr.success('عضویت شما با موفقیت انجام شد.', 'عضویت در خبرنامه');
                    $newsletterEmail.val('');
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'عضویت در خبرنامه');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'عضویت در خبرنامه');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });
});