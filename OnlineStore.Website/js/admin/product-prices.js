/// <reference path="../jquery-1.11.3.min.js" />

var $groupID = $('#hfJSONGroups'),
    $resultBox = $('.result-box'),
    $btnSearch = $('.btn-search'),
    $productBox = $('.product-stored-box'),
    $title = $('#Title');

$btnSearch.on('click', function () {
    getList();
});

function getList() {
    var $loading = $(staticTemplates.Loading);
    $productBox.append($loading);

    $.ajax({
        type: 'POST',
        url: '/ProductPrices/Search',
        data: {
            Title: $title.val(),
            GroupIDs: eval($groupID.val())
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length == 0) {
                    $resultBox.html('<span class="not-found">موردی یافت نشد.</span>');
                }
                else {
                    $resultBox.empty();
                    var $ul = $('<ul>');

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data[i];

                        var li = '<li data-id="' + item.ID + '" title="' + item.Title + '">' +
                                 '<a href="/Products/A/B-' + item.ID + '" target="_blank">' + item.Title + '</a>' +
                                 '</li>';

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