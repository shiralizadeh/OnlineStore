/// <reference path="jquery-1.11.3.min.js" />
$(function () {
    var orgPriceBox = $('.product-view .price-box').html(),
        $availability = $('.product-availability-box'),
        $productVarients = $('.product-varients .row'),
        $tabs = $('.product-tabs') ,
        $controls = $('.product-controls'),
        availabile = '<div class="product-available"><i class="fa fa-check-circle"></i> موجود</div>',
        unAvailabile = '<div class="product-unavailable"><i class="fa fa-times-circle"></i> عدم موجودی</div>',
        $boxHeader = $('.product-tabs .box-header'),
        $boxContent = $('.product-tabs .box-content'),
        $tabBoxes = $('#TabBoxes'),

        $firstComment = $('.product-firstcomment'),
        $rate = $('.product-details .rate-comment .rate'),
        productID = $('.product-details').data('productid'),

        $countScore = $('.count-score'),
        $totalScore = $('.total-score'),

        $btnRequest = $('.btn-add-request'),
        $requestForm = $('.add-request-form'),
        $requestEmail = $('.req-email'),
        $requestMobile = $('.req-mobile'),
        $requestDesc = $('.req-description'),
        $btnSendRequest = $('.btn-send-request'),

        $sendToFriend = $('#SendFriendForm'),
        $friendEmail = $('#SendFriendForm #FriendEmail'),
        $friendMessage = $('#SendFriendForm #Message'),
        $btnSendToFriend = $(".btn-send-friend"),
        $btnFriendForm = $('.btn-send-to-friend'),
        productname = $('.product-title h1').text();

    makeRaty();

    // Product Gallery
    $(".cloudzoom").CloudZoom({
        zoomPosition: 12
    });

    $(".cloudzoom-gallery").CloudZoom();

    $('#ProductGallery').Thumbelina({
        $bwdBut: $('#ProductGallery .left'),
        $fwdBut: $('#ProductGallery .right')
    });

    // Tabs
    $boxContent.children('div').each(function () {
        var $this = $(this);
        var tagH2 = $this.find('h2'),
            $copyH2 = tagH2.clone();

        $copyH2.find('span').remove();

        $boxHeader.append('<a href="#' + $this.attr("ID") + '">' + $copyH2.text() + '</a>');

        tagH2.addClass('hide');
    });

    $boxHeader.append('<div class="clear"></div>');

    $boxHeader.children('a:first').addClass('selected');
    $boxContent.children('div:first').show()
    $(window).resize();

    $('.attr-op').on('click', function (e) {
        e.preventDefault();
        var $this = $(this),
            priceBox = $('.product-price-box'),
            priceHtml = '',
            availabilityHtml = '';

        if ($this.hasClass('disabled'))
            return;

        $this.addClass('selected')
             .siblings('.attr-op').removeClass('selected');

        var selectedCode = [];
        var $selected = $('.attr-op.selected'),
            $notSelected = $('.attr-op:not(.selected)');

        $notSelected = _.reject($notSelected, function (item) {
            return $(item).siblings('.selected').length;
        });

        if ($selected.length != $productVarients.length) {
            var left = productVarients;

            $selected.each(function () {
                var $this = $(this);

                var attributeID = $this.closest('.row').data('attributeid'),
                    optionID = $this.data('optionid');

                left = _.reject(left, function (varient) {
                    var result = _.where(varient.Attributes, { AttributeID: attributeID, AttributeOptionID: optionID });

                    if (result.length)
                        return false;
                    else
                        return true;
                });
            });

            _.each($notSelected, function (item) {
                var $item = $(item),
                    optionID = $item.data('optionid');

                var exists = false;
                _.each(left, function (varient) {
                    if (!exists)
                        if (_.where(varient.Attributes, { AttributeOptionID: optionID }).length)
                            exists = true;
                        else
                            exists = false;

                });

                if (exists)
                    $item.removeClass('disabled');
                else
                    $item.addClass('disabled');
            });

            priceHtml = orgPriceBox;
            availabilityHtml = availabile;
        }
        else {
            $selected.each(function () {
                var $this = $(this);

                var attributeID = $this.closest('.row').data('attributeid'),
                    optionID = $this.data('optionid');

                selectedCode.push(attributeID + ',' + optionID);
            });

            var selectedVarient;
            _.each(productVarients, function (varient) {
                var found = true;

                _.each(varient.Attributes, function (attr) {
                    if (selectedCode.indexOf(attr.AttributeID + ',' + attr.AttributeOptionID) == -1)
                        found = false;
                });

                if (found)
                    selectedVarient = varient;
            });

            if (selectedVarient) {
                if (selectedVarient.DiscountPercent > 0) {
                    priceHtml =
                        '<div class="row">' +
                        '    <div class="price-discount">' +
                        '        <div class="original-price">' +
                        toPrice(selectedVarient.Price, '{0} <span>{1}</span>') +
                        '        </div>' +
                        '        <div class="discount-price" title="' + selectedVarient.DiscountPercent + '% تخفیف">' +
                        toPrice(selectedVarient.Price - selectedVarient.DiscountPrice, '{0} <span>{1} تخفیف</span>') +
                        '        </div>' +
                        '    </div>' +
                        '    <div class="topay-price">' +
                        toPrice(selectedVarient.DiscountPrice, '{0} <span>{1}</span>') +
                        '    </div>' +
                        '</div>';
                }
                else {
                    priceHtml = '<div class="row"><div class="regular-price">' + toPrice(selectedVarient.Price, '{0} <span>{1}</span>') + '</div></div>';
                }

                availabilityHtml = availabile;
            }
            else {
                availabilityHtml = unAvailabile;
                priceHtml = '';
            }
        }

        priceBox.html(priceHtml);
        $availability.html(availabilityHtml);
    });

    $('.product-details .btn-add-cart').on('click', function (e) {
        e.preventDefault();

        var $this = $(this);
        //$productDetails = $this.closest('.product-details'),
        //productname = $productDetails.find('h1').text();

        if (window.productVarients) {
            var $selected = $('.attr-op.selected'),
                $notSelected = $('.attr-op:not(.selected)');

            if ($selected.length != $productVarients.length) {
                toastr.warning('لطفا نوع کالا را قبل از افزودن به سبد خرید مشخص کنید.', 'افزودن به سبد خرید');
            }
            else {
                var $loading = $(staticTemplates.Loading);
                $controls.append($loading);

                var selectedCode = [];
                var selectedAttrs = [];
                $selected.each(function () {
                    var $this = $(this);

                    var $row = $this.closest('.row'),
                        attributeID = $row.data('attributeid'),
                        optionID = $this.data('optionid');

                    selectedCode.push(attributeID + ',' + optionID);
                    selectedAttrs.push($row.find('label').text() + $row.find('.selected').text());
                });

                var selectedVarient;
                _.each(productVarients, function (varient) {
                    var found = true;

                    _.each(varient.Attributes, function (attr) {
                        if (selectedCode.indexOf(attr.AttributeID + ',' + attr.AttributeOptionID) == -1)
                            found = false;
                    });

                    if (found)
                        selectedVarient = varient;
                });

                $.ajax({
                    type: "POST",
                    url: "/Cart/Add",
                    data: {
                        ProductVarientID: selectedVarient.ID
                    },
                    success: function (response) {
                        if (response.Success) {
                            var result = response.Data;

                            if (result.Exists)
                                toastr.warning('شما \'' + productname + ' (' + selectedAttrs.join(' + ') + ')\'  را به <a href="/Cart">سبد خرید</a> اضافه کرده اید.', 'سبد خرید');
                            else
                                toastr.success('\'' + productname + ' (' + selectedAttrs.join(' + ') + ')\' به <a href="/Cart">سبد خرید</a> اضافه شد.', 'سبد خرید');

                            renderMiniCart(result);
                        }
                        else {
                            toastr.error(staticTexts.ResponseError, 'سبد خرید');
                        }
                    },
                    error: function () {
                        toastr.error(staticTexts.RequestError, 'سبد خرید');
                    },
                    complete: function () {
                        $loading.remove();
                    }
                });
            }
        }
        else {
            var $loading = $(staticTemplates.Loading);
            $controls.append($loading);

            $.ajax({
                type: "POST",
                url: "/Cart/Add",
                data: {
                    ProductID: productID
                },
                success: function (response) {
                    if (response.Success) {
                        var result = response.Data;

                        if (result.Exists)
                            toastr.warning('شما \'' + productname + '\'  را به <a href="/Cart">سبد خرید</a> اضافه کرده اید.', 'سبد خرید');
                        else
                            toastr.success('\'' + productname + '\' به <a href="/Cart">سبد خرید</a> اضافه شد.', 'سبد خرید');

                        renderMiniCart(result);
                    }
                    else {
                        toastr.error(staticTexts.ResponseError, 'سبد خرید');
                    }
                },
                error: function () {
                    toastr.error(staticTexts.RequestError, 'سبد خرید');
                },
                complete: function () {
                    $loading.remove();
                }
            });
        }
    });

    $boxHeader.on("click", 'a', function (e) {
        e.preventDefault();

        $boxContent.children('div').hide();
        $boxHeader.find('a').removeClass('selected');

        var $this = $(this);
        $this.addClass('selected');
        var tabID = $(this).attr('href');

        $('html,body').animate({
            scrollTop: $tabs.offset().top
        }, 600);

        $(tabID).show();
        $(window).resize();
    });

    $firstComment.on('click', 'a', function () {
        var commentLink = $boxHeader.find('a[href="#ProductComments"]');
        commentLink.click();

        $('html,body').animate({
            scrollTop: commentLink.offset().top
        }, 600)
    });

    $('.product-accessories, .similar-products').each(function () {
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

    $('.btn-send-to-friend, .btn-send-score, .btn-add-request').magnificPopup({
        type: 'inline',
        midClick: true,
        removalDelay: 300,
        mainClass: 'mfp-fade',
    });

    // Sticky
    $tabBoxes.sticky({ topSpacing: 0, bottomSpacing: 900 });
    $tabBoxes.on('sticky-start', function () {
        var elem = $('<div class="hidden-title">').append($('.product-features h1').children().clone());
        $(this).children('.clear').before(elem);
    });
    $tabBoxes.on('sticky-end', function () {
        $(this).children('div.hidden-title').remove();
    });

    //Wishes
    $controls.on('click', '.btn-wish', function (e) {
        e.preventDefault();

        var $loading = $(staticTemplates.Loading);
        $controls.append($loading);

        var $productDetails = $(this).closest('.product-details');
        $.ajax({
            type: "POST",
            url: "/UserWishes/Add",
            data: {
                ProductID: productID
            },
            success: function (response) {
                if (response.Success) {
                    var result = response.Data;

                    if (result.Login) {
                        if (result.Exists)
                            toastr.warning('شما \'' + productname + '\'  را به <a href="/My-Account/My-Wishes">آرزوهای من</a> اضافه کرده اید.', 'سبد خرید');
                        else
                            toastr.success('\'' + productname + '\' به <a href="/My-Account/My-Wishes">آرزوهای من</a> اضافه شد.', 'آرزوی من');
                    }
                    else {
                        toastr.warning('لطفا <a href="/ثبت-نام">ثبت نام</a> کنید یا <a href="/Login">وارد سایت</a> شوید.', 'آرزوی من');
                    }
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'آرزوی من');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'آرزوی من');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });

    //Request
    // Get User Info
    $btnRequest.on('click', function () {

        if ($btnSendRequest.hasClass('hide')) {
            $btnSendRequest.removeClass('hide').val('');
            $requestDesc.removeAttr('disabled').val('');
            $requestEmail.removeAttr('disabled').val('');
            $requestMobile.removeAttr('disabled');
        }

        $('.alert-request').remove();

        var $loading = $(staticTemplates.Loading);
        $requestForm.append($loading);

        $.ajax({
            type: "POST",
            url: "/Products/GetUserInfo",
            success: function (response) {
                if (response.Success) {
                    if (response.Data != null) {
                        var item = response.Data;
                        $requestEmail.val(item.Email);
                        $requestMobile.val(item.Mobile);
                    }
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'ارسال به دوست');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'ارسال به دوست');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });

    //Send Request
    $btnSendRequest.on('click', function (e) {

        e.preventDefault();

        if ($requestMobile.val() == '' && $requestEmail.val() == '') {
            toastr.error('لطفا شماره همراه یا ایمیل خود را وارد نمایید.');
            return;
        }

        if (!validateEmail($requestEmail.val())) {
            toastr.error('فرمت ایمیل نامعتبر است.');
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $requestForm.append($loading);

        $.ajax({
            type: "POST",
            url: "/Products/AddRequest",
            data: {
                ProductID: productID,
                Email: $requestEmail.val(),
                Mobile: $requestMobile.val(),
                Description: $requestDesc.val()
            },
            success: function (response) {
                if (response.Success) {
                    $requestForm.before('<div class="alert alert-success alert-request">درخواست شما با موفقیت ارسال شد.</div>');
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'درخواست محصول');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'درخواست محصول');
            },
            complete: function () {
                $loading.remove();
                $requestEmail.attr('disabled', 'disabled');
                $requestMobile.attr('disabled', 'disabled');
                $requestDesc.attr('disabled', 'disabled');
                $btnSendRequest.addClass('hide');
            }
        });
    });

    // Open & Close AttributeBox
    $('.attributes').on('click', 'h3 a.group-title', function (e) {
        e.preventDefault();

        var $groupBox = ($(this).closest('h3')),
        $attrsBox = $groupBox.next('ul');

        if (!$attrsBox.hasClass('close')) {
            $attrsBox.addClass('close');
            $groupBox.addClass('close');
        }
        else {
            $attrsBox.removeClass('close');
            $groupBox.removeClass('close');
        }

        $attrsBox.slideToggle();
    });

    //Send To Friend
    $btnSendToFriend.on('click', function () {
        if ($friendEmail.val() == '') {
            toastr.error('لطفا ایمیل دوستتان را وارد نمایید.');
            return;
        }

        if (!validateEmail($friendEmail.val())) {
            toastr.error('فرمت ایمیل نامعتبر است.');
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $sendToFriend.append($loading);
        $.ajax({
            type: 'POST',
            url: '/Products/SendToFriend',
            data: {
                ID: productID,
                FriendEmail: $friendEmail.val(),
                Message: $friendMessage.val(),
            },
            success: function (response) {
                if (response.Success) {
                    $sendToFriend.before('<div class="alert alert-success alert-sendtofriend">ایمیل شما با موفقیت ارسال شد.</div>');
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'ارسال به دوست');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'ارسال به دوست');
            },
            complete: function () {
                $loading.remove();
                $friendEmail.attr('disabled', 'disabled');
                $friendMessage.attr('disabled', 'disabled');
                $btnSendToFriend.addClass('hide');
            }
        });
    });
    $btnFriendForm.on('click', function () {
        if ($btnSendToFriend.hasClass('hide')) {
            $friendEmail.removeAttr('disabled').val('');
            $friendMessage.removeAttr('disabled').val('');
            $btnSendToFriend.removeClass('hide');
        }

        $('.alert-sendtofriend').remove();
    });

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
                    url: '/Products/InsertRate',
                    data: {
                        ProductID: productID,
                        Rate: rate
                    },
                    success: function (result) {
                        if (result.Success) {
                            toastr.success('با تشکر از امتیاز شما', 'امتیازدهی');
                            $countScore.text(result.Data.CountScore);
                            $totalScore.text(result.Data.TotalScore);
                        }
                        else if (result.Errors[0] == "Repeat") {
                            toastr.error('قبلا برای این محصول امتیاز ثبت کرده اید! ', 'امتیازدهی');
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

    $('.free-delivery').on('mouseenter', function () {
        var $this = $(this);
        $this.addClass('bounceIn');
    });
});
