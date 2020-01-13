$(function () {
    // slider
    var $newsletterEmail = $('#Newsletter'),
        $newsBox = $('.home-newsletter'),
        $btnNewsLetter = $('.btn-newsletter');

    // OfferSlider
    var $discountSlider = $('#OfferSlider');

    $discountSlider.owlCarousel({
        autoplay: true,
        loop: true,
        autoplayHoverPause: true,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        items: 1,
        smartSpeed: 450
    });

    var $discountSliderCtrls = $('.offer-controls li');
    $discountSliderCtrls.on('click', function (e) {
        e.preventDefault();

        var $this = $(this),
            index = $this.data('index');

        $discountSlider.trigger('to.owl.carousel', index)

    });

    $discountSlider.on('changed.owl.carousel', function (e) {
        var index = e.page.index;

        $discountSliderCtrls.removeClass('active');
        $discountSliderCtrls.eq(index).addClass('active');
    });

    $discountSliderCtrls.eq(0).addClass('active');

    // products-list
    $('.products-list').each(function () {
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

    $('#Brands').owlCarousel({
        autoplay: true,
        autoplayHoverPause: true,
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
            }
        }
    });

    // newsletter
    $btnNewsLetter.on('click', function (e) {
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