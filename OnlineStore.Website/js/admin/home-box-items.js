var $key = $('.list-items #Search'),
    $addToList = $('#AddToList'),
    $homeBoxItemsUl = $('#Items ul'),
    $homeBoxItems = $('#Items'),
    $homeBoxID = $('#ID'),
    $loading = $('.list-items .loading'),
    $update = $('.list-items .icon-check-sign'),
    $successPanel = $('.messages-body .alert-success'),
    $errorPanel = $('.messages-body .alert-danger'),
    productList = '',
    productItems = [];

$loading.fadeOut();

// نمایش لیست
getList();

$homeBoxItemsUl.sortable({
    stop: function (event, ui) {
        var array = $(this).sortable('toArray', { attribute: 'id' });

        productItems = _.map(array, function (item_a) {
            return _.find(productItems, function (item_b) {
                return item_a == item_b.ID;
            });
        });

        productList = array.join(',');
        $update.removeClass('hide');
    }
});

$homeBoxItemsUl.disableSelection();

$homeBoxItemsUl.on('click', 'li .icon-remove-sign', function (e) {
    e.preventDefault();
    var $this = $(this);

    productItems = _.filter(productItems, function (item) {
        return item.ID != $this.data('id');
    });

    renderItems();
    $update.removeClass('hide');

});

$addToList.on('click', function () {
    var id = $key.data('id'),
        title = $key.val();

    if (id == -1) {
        showError('لطفا محصول مورد نظر را انتخاب نمایید.');
        return;
    }

    var repeat = _.find(productItems, function (item) {
        return item.ID == id
    });

    if (typeof (repeat) == 'undefined') {
        productItems.push({
            ID: id,
            Title: title
        });
    }
    else {
        showError('محصول مورد نظر در این بخش موجود است!');
        return;
    }

    renderItems();

    $update.removeClass('hide');
    $key.removeAttr('disabled').val('');

});

$update.on('click', function () {
    updateList(productList);
});

function getList() {

    productItems = [];
    $homeBoxItemsUl.empty();

    $.ajax({
        type: 'POST',
        url: '/HomeBoxProducts/Get',
        data: {
            HomeBoxID: $homeBoxID.val()
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length == 0) {
                    $homeBoxItemsUl.after('<span class="not-found">موردی یافت نشد.</span>');
                }
                else {
                    $homeBoxItemsUl.siblings('.not-found').remove();

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data;
                        var li = '<li id="' + item[i].ProductID + '"><i class="icon-sort"></i><span>' + item[i].Title + '</span><a class="icon-remove-sign" data-id="' + item[i].ProductID + '" title="' + item[i].Title + '"></a></li>';
                        $homeBoxItemsUl.append(li);

                        productItems.push({
                            ID: item[i].ProductID,
                            Title: item[i].Title
                        });
                    }
                }
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

function updateList(products) {
    $loading.fadeIn();

    $.ajax({
        type: 'POST',
        url: '/HomeBoxProducts/Update',
        data: {
            HomeBoxID: $homeBoxID.val(),
            Products: products
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
    $homeBoxItemsUl.empty();

    productList = _.map(productItems, function (item) { return item.ID; }).join(',');

    if (productItems.length <= 0) {
        $homeBoxItemsUl.after('<span class="not-found">موردی یافت نشد.</span>');
    }
    else {
        $homeBoxItemsUl.siblings('.not-found').remove();
        $homeBoxItems.fadeIn();

        for (var i in productItems) {
            var item = productItems[i];

            var li = '<li id="' + item.ID + '"><i class="icon-sort"></i><span>' + item.Title + '</span><a class="icon-remove-sign" data-id="' + item.ID + '" title="' + item.Title + '"></a></li>';

            $homeBoxItemsUl.append(li);

        }
    }
}

