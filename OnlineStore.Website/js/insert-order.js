$(function () {
    var $btnNextStep = $('.btn-nextstep'),
        $btnPrevStep = $('.btn-prevstep'),
        step = 1,
        $stepsBox = $('.steps-box'),

        $userInfo = $('.user-info'),
        $methods = $('.methods'),
        $cartBox = $(".cart-box"),
        $payment = $("#Payment"),
        $btnPay = $('.btn-pay'),
        $bankMellat = $('#BankMellat'),

        $sendtype = $('.sendtype'),
        $paymenttype = $('.paymenttype'),

        $firstname = $('#Firstname'),
        $lastname = $('#Lastname'),
        $email = $('#Email'),
        $phone = $('#Phone'),
        $mobile = $('#Mobile'),

        $deliveryInfo = $('.delivery-info'),

        $postalCode = $('#PostalCode'),
        $address = $('#HomeAddress'),
        $userDescription = $('#UserDescription'),
        $state = $('#States'),
        $city = $('#Cities');

    $btnNextStep.on('click', function (e) {
        e.preventDefault();
        var $this = $(this);

        if (step == 2 || step == 3) {
            if (!$payment.valid()) {
                return;
            }

            if (step == 2) {
                var price = parseInt($('.step-content-1 .total-topay').data('price'));

                if (isFreeDelivery(price)) {
                    $deliveryInfo.text(staticTexts.MaxPriceFreeDelivery);
                }
                else {
                    $deliveryInfo.text('هزینه ثابت 10 هزار تومان');
                }
            }

            copyCart();
        }
        else if (step == 4) {
            var $loading = $(staticTemplates.Loading);
            $cartBox.append($loading);

            var postData = {
                SendMethodType: $sendtype.find(':checked').val(),
                PaymentMethodType: $paymenttype.find(':checked').val(),
                UserDescription: $userDescription.val()
            };

            if (!isAuthenticated) {
                postData.Firstname = $firstname.val();
                postData.Lastname = $lastname.val();
                postData.Email = $email.val();
                postData.Phone = $phone.val();
                postData.Mobile = $mobile.val();
                postData.StateID = $state.val();
                postData.CityID = $city.val();
                postData.PostalCode = $postalCode.val();
                postData.HomeAddress = $address.val();
            }

            $.ajax({
                type: 'POST',
                url: '/Cart/Payment',
                data: postData,
                success: function (result) {
                    if (result.Success) {
                        var data = result.Data;

                        if (postData.PaymentMethodType == 0) {
                            $bankMellat.attr('action', data.PgwSite);
                            $bankMellat.find('#RefID').val(data.RefID);

                            $bankMellat.submit();

                            toastr.success('در حال ارسال به بانک', 'پرداخت سبد خرید');
                        }
                        else if (postData.PaymentMethodType == 1 || postData.PaymentMethodType == 2) {
                            toastr.success('اطلاعات شما با موفقیت ثبت شد.', 'ثبت سفارش');

                            step++;

                            $('.step-content').hide();
                            $('.step-content-' + step).show();
                            $stepsBox.find('.btn-step').removeClass('active').eq(step - 1).addClass('active').prevAll().addClass('success');
                            $btnNextStep.hide();
                            $btnPrevStep.hide();

                            if (postData.PaymentMethodType == 1) {
                                $('.step-content-5 .price').html(toPrice(data.ToPayPrice, '{0} <span class="price-unit">{1}</span>'));

                                $('.step-content-5 .type_1').show();
                                $('.step-content-5 .type_2').hide();
                            }
                            else if (postData.PaymentMethodType == 2) {
                                $('.step-content-5 .type_2').show();
                                $('.step-content-5 .type_1').hide();
                            }
                        }
                    }
                    else {
                        toastr.error(staticTexts.ResponseError, 'پرداخت سبد خرید');
                    }
                },
                error: function (response) {
                    toastr.error(staticTexts.RequestError, 'پرداخت سبد خرید');
                },
                complete: function () {
                    $loading.remove();
                }
            });

            return;
        }

        step++;

        $('.step-content').hide();
        $('.step-content-' + step).show();

        $stepsBox.find('.btn-step').removeClass('active').eq(step - 1).addClass('active').prevAll().addClass('success');

        if (step == 2) {
            $btnNextStep.find('span').text('دریافت اطلاعات پستی');
            $btnPrevStep.find('span').text('مشاهده سبد خرید');

            $btnNextStep.show();
            $btnPrevStep.show();
        }
        else if (step == 3) {
            $btnNextStep.find('span').text('پرداخت سبد خرید');
            $btnPrevStep.find('span').text('دریافت اطلاعات شخصی');

            $btnNextStep.show();
            $btnPrevStep.show();
        }
        else if (step == 4) {

            var nextText = '';
            if ($paymenttype.find(':checked').val() == '0')
                nextText = 'ارسال به بانک';
            else
                nextText = 'ثبت سفارش';

            $btnNextStep.find('span').text(nextText);
            $btnPrevStep.find('span').text('دریافت اطلاعات پستی');

            $btnNextStep.show();
            $btnPrevStep.show();
        }
        else {
            if (!isAuthenticated) {
                if (!$payment.valid()) {
                    return;
                }

                if ($state.val() == -1 || $city.val() == -1) {
                    toastr.error("تکمیل فیلد استان و شهر الزامی است");
                    return;
                }
            }
            else {
                if ($userInfo.find('i').hasClass('fa-times-circle')) {
                    toastr.error('لطفا اطلاعات خود را تکمیل نمایید');
                    return;
                }
            }

            $userInfo.hide();
            $methods.show();
        }

    });

    $btnPrevStep.on('click', function (e) {
        e.preventDefault();
        step--;

        $('.step-content').hide();
        $('.step-content-' + step).show();

        $stepsBox.find('.btn-step').removeClass('active success').eq(step - 1).addClass('active').prevAll().addClass('success');

        if (step == 1) {
            $btnNextStep.find('span').text('دریافت اطلاعات شخصی');
            $btnPrevStep.find('span').text('مشاهده سبد خرید');

            $btnNextStep.show();
            $btnPrevStep.hide();
        }
        if (step == 2) {
            $btnNextStep.find('span').text('دریافت اطلاعات پستی');
            $btnPrevStep.find('span').text('مشاهده سبد خرید');

            $btnNextStep.show();
            $btnPrevStep.show();
        }
        else if (step == 3) {
            $btnNextStep.find('span').text('پرداخت سبد خرید');
            $btnPrevStep.find('span').text('دریافت اطلاعات شخصی');

            $btnNextStep.show();
            $btnPrevStep.show();
        }

    });

    $state.val('');
    $city.val('');

    $state.on('change', cityChanged);
    $city.on('change', cityChanged).change();

    function cityChanged(e) {
        var $this = $(this);

        if ($city.val() == '468') {
            // SendType
            $('#SendType_0').prop({
                checked: true,
                disabled: false
            });
            $('#SendType_1, #SendType_2').prop({
                checked: false,
                disabled: true
            });

            // PaymentType
            $('#PaymentType_2').prop({
                disabled: false
            });
        }
        else {
            // SendType
            $('#SendType_0').prop({
                checked: false,
                disabled: true
            });
            $('#SendType_1').prop({
                disabled: false,
                checked: true
            });
            $('#SendType_2').prop({
                disabled: false,
            });

            //if (!$('#SendType_1, #SendType_2').is(':checked')) {
            //    $('#SendType_2').prop({
            //        checked: true
            //    });
            //}

            // PaymentType
            $('#PaymentType_2').prop({
                checked: false,
                disabled: true
            });

            if (!$('#PaymentType_0, #PaymentType_1').is(':checked')) {
                $('#PaymentType_0').prop({
                    checked: true
                });
            }
        }
    }
});