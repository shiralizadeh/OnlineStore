var $key = $('.list-items #Search'),
    $startDate = $('#StartDate'),
    $endDate = $('#EndDate'),
    $addToList = $('#AddToList'),
    $productGiftsUl = $('#Items ul'),
    $productGifts = $('#Items'),
    $productID = $('#ProductID'),
    $loading = $('.list-items .loading'),
    $update = $('.list-items .icon-check-sign'),
    $successPanel = $('.messages-body .alert-success'),
    $errorPanel = $('.messages-body .alert-danger'),
    productItems = [];

$loading.fadeOut();

// نمایش لیست
getList();

$productGiftsUl.sortable({
    stop: function (event, ui) {
        var array = $(this).sortable('toArray', { attribute: 'id' });

        productItems = _.map(array, function (item_a) {
            return _.find(productItems, function (item_b) {
                return item_a == item_b.GiftID;
            });
        });

        $update.removeClass('hide');
    }
});

$productGiftsUl.disableSelection();

$productGiftsUl.on('click', 'li .icon-remove-sign', function (e) {
    e.preventDefault();
    var $this = $(this);

    productItems = _.filter(productItems, function (item) {
        return item.GiftID != $this.data('id');
    });

    console.log($this.data('id'));

    renderItems();
    $update.removeClass('hide');

});

$addToList.on('click', function () {
    var id = $key.data('id'),
        title = $key.val();

    if (id == -1 || $startDate.val() == '' || $endDate.val() == '') {
        showError('لطفا تمامی فیلدها را تکمیل نمایید.');
        return;
    }

    var repeat = _.find(productItems, function (item) {
        return item.GiftID == id
    });

    if (typeof (repeat) == 'undefined') {
        productItems.push({
            GiftID: id,
            Title: title,
            PersianStartDate: $startDate.val(),
            PersianEndDate: $endDate.val()
        });
    }
    else {
        showError('محصول مورد نظر در لیست موجود است!');
        return false;
    }

    renderItems();

    $update.removeClass('hide');
    $key.removeAttr('disabled').val('');
    $startDate.val('');
    $endDate.val('');

});

$update.on('click', function () {
    updateList();
});

function getList() {

    productItems = [];
    $productGiftsUl.empty();

    $.ajax({
        type: 'POST',
        url: '/ProductGifts/Get',
        data: {
            ProductID: $productID.val()
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length == 0) {
                    $productGiftsUl.after('<span class="not-found">موردی یافت نشد.</span>');
                }
                else {
                    $productGiftsUl.siblings('.not-found').remove();

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data;
                        var li = '<li id="' + item[i].GiftID + '"><i class="icon-sort"></i><span>' + item[i].Title + '</span><span class="date">' + item[i].PersianStartDate + '-' + item[i].PersianEndDate + '</span><a class="icon-remove-sign" data-id="' + item[i].GiftID + '" title="' + item[i].Title + '"></a></li>';
                        $productGiftsUl.append(li);

                        productItems.push({
                            GiftID: item[i].GiftID,
                            Title: item[i].Title,
                            PersianStartDate: item[i].PersianStartDate,
                            PersianEndDate: item[i].PersianEndDate
                        });
                    }
                }

                console.log(productItems);
            }
            else {
                alert('خطا');
            }
        },
        error: function () {
            alert('خطا');
        },
    });
}

function updateList() {
    $loading.fadeIn();
    $.ajax({
        type: 'POST',
        url: '/ProductGifts/Update',
        data: {
            ProductID: $productID.val(),
            Products: productItems
        },
        success: function (response) {
            if (response.Success) {
                $successPanel.fadeIn().removeClass('hide').children('span').text('عملیات بروزرسانی با موفقیت انجام شد.');
                $update.addClass('hide');
            }
            else {
                showError(response.Errors);
            }
        },
        error: function (response) {
            showError(response.Errors);
        },
        complete: function () {
            $loading.fadeOut();
        }
    });
}

function showError(errors) {
    errorList = $errorPanel.removeClass("hide").fadeIn().children('ul');
    errorList.empty();
    if (typeof (errors) != 'string') {
        for (var i in errors) {
            errorList.append("<li>" + errors[i] + "</li>");
        }
    }
    else {
        errorList.append("<li>" + errors + "</li>");
    }

    $key.removeAttr('disabled').val('');
}

function renderItems() {
    $successPanel.fadeOut();
    $errorPanel.fadeOut();
    $productGiftsUl.empty();

    if (productItems.length <= 0) {
        $productGiftsUl.after('<span class="not-found">موردی یافت نشد.</span>');
    }
    else {
        $productGiftsUl.siblings('.not-found').remove();
        $productGifts.fadeIn();

        for (var i in productItems) {
            var item = productItems[i];

            var li = '<li id="' + item.GiftID + '"><i class="icon-sort"></i><span>' + item.Title + '</span><span class="date">' + item.PersianStartDate + '-' + item.PersianEndDate + '</span><a class="icon-remove-sign" data-id="' + item.GiftID + '" title="' + item.Title + '"></a></li>';

            $productGiftsUl.append(li);

        }
    }
}
