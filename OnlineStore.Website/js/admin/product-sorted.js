/// <reference path="../jquery-1.11.3.min.js" />

var $groupID = $('#hfJSONGroups'),
    $resultBox = $('.result-box'),
    $btnSearch = $('.btn-search'),
    $productBox = $('.product-stored-box'),
    $productBox = $('.product-stored-box'),
    $btnRefresh = $('.icon-refresh');

$btnSearch.on('click', function () {
    getList();
    $btnRefresh.removeClass('hide');
});

var orderTimer = undefined,
    $orderAjax = undefined;
$(document).on('keyup paste', '.order', function () {
    var $this = $(this);
    var $li = $this.closest('li');

    if ($orderAjax)
        $orderAjax.abort();

    if (orderTimer)
        clearTimeout(orderTimer);

    orderTimer = setTimeout(function () {
        var $loading = $(staticTemplates.Loading);
        $li.append($loading);

        var pid = $li.data('id');

        $orderAjax = $.ajax({
            type: "POST",
            url: "/Admin/ProductsSorted/SetOrder",
            data: {
                ProductID: pid,
                OrderID: $this.val()
            },
            success: function (response) {
                if (!response.Success) {
                    toastr.error('فرمت عدد نامعتبر', 'خطا');
                }
            },
            error: function () {
                toastr.error('خطا در بروز رسانی', 'خطا');
            },
            complete: function () {
                $loading.remove();
            }
        });
    }, 400);
});

$btnRefresh.on('click', function (e) {
    e.preventDefault();
    getList();
});

function getList() {
    var $loading = $(staticTemplates.Loading);
    $productBox.append($loading);

    $.ajax({
        type: 'POST',
        url: '/ProductsSorted/GetProducts',
        data: {
            GroupIDs: eval($groupID.val())
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length == 0) {
                    $resultBox.html('<span class="not-found">موردی یافت نشد.</span>');
                }
                else {
                    $resultBox.empty();
                    var $ul = $('<ul class="sortable-grid">');

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data[i];

                        var li = '<li class="ui-sortable-handle" data-id="' + item.ID + '" title="' + item.Title + '">'
                                        + '<p class="index">' + (response.Data.length - i) + '</p>'
                                        + '<div class="img-box"><img src="' + item.ImageFile + '"></div>'
                                        + '<div class="title-box"><a href="/Products/A/B-' + item.ID + '" target="_blank">' + item.Title + '</a></div>'
                                        + '<div class="order-box">'
                                        + '<input class="order form-control" type="text" value="' + item.OrderID + '" name="product_' + item.ID + '" />'
                                        + '<p class="order-sum">' + '(' + (item.IsUnavailable && item.PriceStatus == 0 ? -1 : 1) + ') * (' + item.Weight + '+' + item.OrderID + ')' + ' = ' + (item.IsUnavailable && item.PriceStatus == 0 ? -1 : 1) * (item.Weight + item.OrderID) + '</p>'
                                        + '</div>'
                                    + '</li>';

                        $ul.append(li);
                    }

                    $resultBox.append($ul);
                }
            }
            else {
                //alert('خطا');
            }
        },
        error: function () {
            //alert('خطا');
        },
        complete: function () {
            $loading.remove();
        }

    });
}