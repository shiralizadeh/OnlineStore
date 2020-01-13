var $fromDate = $('#FromDate'),
    $toDate = $('#ToDate'),
    $btnSearch = $('#btnSearch'),
    $resultBox = $('.price-list-logs'),
    $alertBox = $('.alert-danger');

$btnSearch.on('click', function () {
    var $loading = $(staticTemplates.Loading);
    $resultBox.append($loading);

    $resultBox.empty();

    $.ajax({
        url: '/Admin/PriceListLogs/GetLogs',
        type: 'Post',
        data: {
            FromDate: $fromDate.val(),
            ToDate: $toDate.val()
        },
        success: function (response) {
            if (response.Success) {
                if (!response.Data.length) {
                    $alertBox.removeClass('hide');
                }
                else {
                    $alertBox.addClass('hide');
                }
                for (var i = 0; i < response.Data.length; i++) {
                    var item = response.Data[i];
                    var $li = $('<li><span class="product-title">' + item.ProductTitle + '</span></li>');

                    if (item.PriceListLogs.length) {
                        var $ul = $('<ul class="logs">');
                        for (var j = 0; j < item.PriceListLogs.length; j++) {
                            var log = item.PriceListLogs[j];

                            var detail = '<span class="price-list-field">' + log.PriceListFieldName + '</span>' +
                                         '<span class="old-value">' + log.OldValue + '</span>' +
                                         '<i class="icon-arrow-left"></i>' +
                                         '<span class="new-value">' + log.NewValue + '</span>' +
                                         toPersianDate(log.LastUpdate);

                            if (log.ColorClass != null) {
                                $ul.append('<li class="' + log.ColorClass + '">' + detail + '</li>');
                            }
                            else {
                                $ul.append('<li>' + detail + '</li>');
                            }
                        }

                        $li.append($ul);
                    }

                    $resultBox.append($li);
                }
            }
        },
        error: function () {
            toastr.error('بروز خطا در بارگذاری اطلاعات', 'خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });
});