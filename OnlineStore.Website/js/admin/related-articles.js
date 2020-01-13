var $key = $('.list-items #Search'),
    $addToList = $('#AddToList'),
    $relatedArticlesUl = $('#Items ul'),
    $relatedArticles = $('#Items'),
    $articleID = $('#ArticleID'),
    $loading = $('.list-items .loading'),
    $update = $('.list-items .icon-check-sign'),
    $successPanel = $('.messages-body .alert-success'),
    $errorPanel = $('.messages-body .alert-danger'),
    articleList = '',
    articleItems = [];

$loading.fadeOut();

// نمایش لیست
getList();

$relatedArticlesUl.sortable({
    stop: function (event, ui) {
        var array = $(this).sortable('toArray', { attribute: 'id' });

        articleItems = _.map(array, function (item_a) {
            return _.find(articleItems, function (item_b) {
                return item_a == item_b.ID;
            });
        });

        articleList = array.join(',');
        $update.removeClass('hide');
    }
});

$relatedArticlesUl.disableSelection();

$relatedArticlesUl.on('click', 'li .icon-remove-sign', function (e) {
    e.preventDefault();
    var $this = $(this);

    articleItems = _.filter(articleItems, function (item) {
        return item.ID != $this.data('id');
    });

    console.log($this.data('id'));

    renderItems();
    $update.removeClass('hide');

});

$addToList.on('click', function () {
    var id = $key.data('id'),
        title = $key.val();

    if (id == -1) {
        showError('لطفا مطلب مورد نظر را انتخاب نمایید.');
        return;
    }

    var repeat = _.find(articleItems, function (item) {
        return item.ID == id
    });

    if (typeof (repeat) == 'undefined') {
        articleItems.push({
            ID: id,
            Title: title
        });
    }
    else {
        showError('مطلب مورد نظر در لیست موجود است!');
        return;
    }

    renderItems();

    $update.removeClass('hide');
    $key.removeAttr('disabled').val('');

});

$update.on('click', function () {
    updateList(articleList);
});

function getList() {

    articleItems = [];
    $relatedArticlesUl.empty();

    $.ajax({
        type: 'POST',
        url: '/RelatedArticles/Get',
        data: {
            ArticleID: $articleID.val()
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length == 0) {
                    $relatedArticlesUl.after('<span class="not-found">موردی یافت نشد.</span>');
                }
                else {
                    $relatedArticlesUl.siblings('.not-found').remove();

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data;
                        var li = '<li id="' + item[i].RelationID + '"><i class="icon-sort"></i><span>' + item[i].Title + '</span><a class="icon-remove-sign" data-id="' + item[i].RelationID + '" title="' + item[i].Title + '"></a></li>';
                        $relatedArticlesUl.append(li);

                        articleItems.push({
                            ID: item[i].RelationID,
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

function updateList(articles) {
    $loading.fadeIn();

    $.ajax({
        type: 'POST',
        url: '/RelatedArticles/Update',
        data: {
            ArticleID: $articleID.val(),
            Articles: articles
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
    $relatedArticlesUl.empty();

    articleList = _.map(articleItems, function (item) { return item.ID; }).join(',');

    if (articleItems.length <= 0) {
        $relatedArticlesUl.after('<span class="not-found">موردی یافت نشد.</span>');
    }
    else {
        $relatedArticlesUl.siblings('.not-found').remove();
        $relatedArticles.fadeIn();

        for (var i in articleItems) {
            var item = articleItems[i];

            var li = '<li id="' + item.ID + '"><i class="icon-sort"></i><span>' + item.Title + '</span><a class="icon-remove-sign" data-id="' + item.ID + '" title="' + item.Title + '"></a></li>';

            $relatedArticlesUl.append(li);

        }
    }
}
