/// <reference path="jquery-1.11.3.min.js" />
$(function () {
    var $trackOrder = $('.track-order-form'),
        $saleReferenceID = $("#SaleReferenceID"),
        $btn = $(".track-order-form .btn-submit"),
        $messageBox = $('.message-box');

    $btn.on("click", function () {
        if (!$trackOrder.valid()) {
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $trackOrder.append($loading);

        $.ajax({
            type: "POST",
            url: "/TrackOrder/ShowStatus",
            data: {
                SaleReferenceID: $saleReferenceID.val(),
            },
            success: function (response) {
                if (response.Success) {
                    $trackOrder.hide();
                    $messageBox.removeClass('hide');

                    if (response.Data != null) {
                        var item = response.Data;
                        switch (item.SendStatus) {
                            case 0: $messageBox.addClass('alert-warning'); message = 'سفارش شما تاکنون مورد بررسی کارشناسان آنلاین استور قرار نگرفته است.'; break;
                            case 1:
                                $messageBox.addClass('alert-success');
                                message = 'سفارش شما ارسال شده است.<br/>';
                                if (item.BillNumber != null && item.BillNumber != '') {
                                    switch (item.SendMethodType) {
                                        case 1: message += "شماره قبض پست پیشتاز: " + item.BillNumber; break;
                                        case 2: message += "شماره قبض تیپاکس: " + item.BillNumber; break;
                                        default:
                                    }
                                }
                                break;
                            case 2: $messageBox.addClass('alert-success'); message = 'سفارش مورد نظر به مقصد رسیده و تحویل داده شده است.'; break;
                            case 3: $messageBox.addClass('alert-default'); message = 'سفارش مورد نظر لغو و هزینه ی آن برگشت داده شده است.'; break;
                            case 4: $messageBox.addClass('alert-info'); message = 'سفارش شما توسط کارشناسان فروش آنلاین استور بررسی شده است و در اسرع وقت ارسال خواهد شد.'; break;
                            default: break;
                        }
                    }
                    else {
                        var message = 'سفارش با کد رهگیری مورد نظر شما یافت نشد!';
                        $messageBox.addClass('alert-error');
                    }

                    $messageBox.append(message);
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'پیگیری سفارش');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'پیگیری سفارش');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });
});