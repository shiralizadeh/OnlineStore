var firstState = _.clone(productsListOptions),
    $productItemTemplate = $('#ProductItemTemplate'),
    $productslist = $('.products-list'),
    $pagingBox = $('.paging-box'),
    $paging = $('.paging'),
    $alertEmpty = $('.alert-empty');

var filters = $('.filter-item input').on('change', function () {
    var $this = $(this);

    productsListOptions.Producers = [];
    productsListOptions.Attributes = [];

    var chFilters = $('.filter-item input:checked');
    chFilters.each(function () {
        var $this = $(this),
            $filteritem = $this.closest('.filter-item'),
            attrID = $filteritem.data('id'),
            $li = $this.closest('li'),
            opID = $li.data('id');

        if ($filteritem.is('.producers')) {
            productsListOptions.Producers.push(opID);
        }
        else
            productsListOptions.Attributes.push(attrID + '-' + opID);
    });

    productsListOptions.PageIndex = 1;
    refresh(true);
});

$('.btn-more').on('click', function (e) {
    e.preventDefault();

    var $this = $(this),
        $prev = $this.prev();

    if (!$prev.hasClass('open')) {
        $prev.addClass('open').css({ height: '100%' });
        $(window).resize();

        $this.text('- کمتر');
    }
    else {
        $prev.removeClass('open').animate({ height: '154px' }, {
            complete: function () {
                $(window).resize();
            }
        });

        $this.text('+ بیشتر');
    }
});

$paging.on('click', 'a', function (e) {
    e.preventDefault();

    var $this = $(this),
        index = $this.data('index');

    productsListOptions.PageIndex = index;
    refresh(true);
});

$(window).on("popstate", function (e) {
    var state = e.originalEvent.state;


    if (!state) {
        state = _.clone(firstState);
    }

    productsListOptions = state;

    $('.filter-item input:checkbox').prop('checked', false);

    _.each(productsListOptions.Producers, function (item) {

        $('.producers li[data-id="' + item + '"] input:checkbox').prop('checked', true);
    });

    _.each(productsListOptions.Attributes, function (item) {

        var attrID = item.split('-')[0],
            opID = item.split('-')[1];

        $('.filter-item[data-id="' + attrID + '"] li[data-id="' + opID + '"] input:checkbox').prop('checked', true);
    });

    refresh(false);
});

function refresh(pushState) {
    loadProducts(pushState);
}

function loadProducts(pushState) {
    var $loading = $(staticTemplates.Loading);
    $productslist.append($loading);

    $('html, body').animate({ 'scrollTop': $('.page-title-box').position().top });

    $.ajax({
        url: groupUrl,
        type: 'POST',
        data: productsListOptions,
        success: function (result) {
            if (result.Success) {
                var data = result.Data;
                var $ul = $productslist.find('ul');

                $ul.empty();

                var products = data.Products;
                var totalPages = data.TotalPages;

                for (var i in products) {
                    var item = products[i],
                        tmp = $productItemTemplate.text();

                    var minPrice = getMinPrice(item);
                    var isUnavailable = item.IsUnavailable;
                    var priceStatus = item.PriceStatus;
                    var commentBox = '';

                    if (item.CommentCount > 0) {
                        commentBox = "<a href='" + item.Url + "' class='comment'>" + item.CommentCount + " نظر</a>";
                    }

                    tmp = tmp.replace(/{{ID}}/g, item.ID);
                    tmp = tmp.replace(/{{Title}}/g, item.Title);
                    tmp = tmp.replace(/{{Url}}/g, item.Url);
                    tmp = tmp.replace(/{{ImageFile}}/g, item.ImageFile);
                    tmp = tmp.replace(/{{DisplayTitle}}/g, item.DisplayTitle);
                    tmp = tmp.replace(/{{DisplayTitleOther}}/g, item.ToolTip);
                    tmp = tmp.replace(/{{ProductVarientID}}/g, (item.HasVarients && minPrice != undefined ? minPrice.ID : ''));
                    tmp = tmp.replace(/{{Price}}/g, renderPrice(minPrice, isUnavailable, priceStatus));
                    tmp = tmp.replace(/{{ToolTip}}/g, item.ToolTip);
                    tmp = tmp.replace(/{{QuickViewUrl}}/g, item.QuickViewUrl);
                    tmp = tmp.replace(/{{CompareUrl}}/g, getCompareUrl(item.ID));
                    tmp = tmp.replace(/{{CommentBox}}/g, commentBox);

                    tmp = tmp.replace(/{{GroupTitle}}/g, item.GroupTitle);
                    tmp = tmp.replace(/{{GroupUrl}}/g, normalizeForUrl(item.GroupTitle_En));

                    tmp = tmp.replace(/{{Score}}/g, renderScore(item.SumScore, item.ScoreCount, item.ProductScore));
                    tmp = tmp.replace(/{{Marks}}/g, getMarks(item));

                    $ul.append(tmp);
                }

                if (products.length == 0) {
                    $alertEmpty.removeClass('hide');
                    $pagingBox.addClass('hide');
                }
                else {
                    $alertEmpty.addClass('hide');
                    $pagingBox.removeClass('hide');
                }

                $paging.empty();
                var paging = data.Paging;
                var originalUrl = data.OriginalUrl;
                var currentPageIndex = data.CurrentPageIndex;
                var totalPages = data.TotalPages;

                if (currentPageIndex > 0) {
                    $paging.append('<li class="page-prev">' +
                        '    <a href="' + originalUrl + '/' + currentPageIndex + '" data-index="' + currentPageIndex + '">' +
                        '        <i class="fa fa-chevron-right"></i>' +
                        '        قبلی' +
                        '    </a>' +
                        '</li>')
                }

                if (currentPageIndex - 5 > 0) {
                    $paging.append('<li class="page-last">' +
                        '    <a href="' + originalUrl + '/1" data-index="1">' +
                        '        1' +
                        '    </a>' +
                        '</li>');
                }

                if (currentPageIndex - 6 > 0) {
                    $paging.append('<li class="page-last">' +
                        '    <a href="' + originalUrl + '/' + (currentPageIndex - 5) + '" data-index="' + (currentPageIndex - 5) + '">' +
                        '        ...' +
                        '    </a>' +
                        '</li>');
                }

                for (var i in paging) {
                    var item = paging[i];

                    if (currentPageIndex + 1 == item) {
                        $paging.append('<li class="current"> ' + item + '</li>');
                    }
                    else {
                        $paging.append('<li><a href="' + originalUrl + '/' + item + '" data-index="' + item + '">' + item + '</a></li>');
                    }
                }

                if (totalPages > currentPageIndex + 7) {
                    $paging.append('<li class="page-last">' +
                        '    <a href="' + originalUrl + '/' + (currentPageIndex + 7) + '" data-index="' + (currentPageIndex + 7) + '">' +
                        '        ...' +
                        '    </a>' +
                        '</li>');
                }

                if (totalPages > currentPageIndex + 6) {
                    $paging.append('<li class="page-last">' +
                        '    <a href="' + originalUrl + '/' + totalPages + '" data-index="' + totalPages + '">' +
                        totalPages +
                        '    </a>' +
                        '</li>');
                }

                if (totalPages > currentPageIndex + 1) {
                    $paging.append('<li class="page-next">' +
                        '    <a href="' + originalUrl + '/' + (currentPageIndex + 2) + '" data-index="' + (currentPageIndex + 2) + '">' +
                        '        بعدی' +
                        '        <i class="fa fa-chevron-left"></i>' +
                        '    </a>' +
                        '</li>');
                }

                var title = data.PageTitle;
                document.title = productsListOptions.PageTitle = title;
                var stitle = title.replace(title.substr(title.lastIndexOf('-') - 1, title.length - title.lastIndexOf('-') + 1), '');
                $('.page-title h1').text(stitle);
                $('.breadcrumbs li:last span').text(stitle);

                var url = data.CanonicalUrl;

                var state = _.clone(productsListOptions);

                if (pushState) {
                    history.pushState(state, title, url);
                }
            }
            else {
                toastr.error('رخداد خطا در نمایش محصولات', 'فیلتر محصولات');
            }
        },
        error: function () {
            toastr.error('رخداد خطا در نمایش محصولات', 'فیلتر محصولات');
        },
        complete: function () {
            $loading.remove();
        }
    });
}

function getMinPrice(item) {
    var prices = item.Prices,
        minPrice = undefined;

    if (prices != null && prices.length > 0) {
        minPrice = _.min(prices, function (item) {
            return item.Price;
        });
    }

    return minPrice;
}

function renderPrice(minPrice, isUnavailable, priceStatus) {
    var result;

    var isUnavailable = (isUnavailable || !minPrice || minPrice.Price == 0);

    if (isUnavailable) {

        switch (priceStatus) {
            case 0:
                result = '<div class="row">' +
                    '    <div class="price-isunavailable">' +
                    '        <i class="fa fa-times-circle"></i>' +
                    '        موجود نیست' +
                    '    </div>' +
                    '</div>';
                break;
            case 1:
                result = '<div class="row">' +
                    '    <div class="price-comingsoon">' +
                    '        <i class="fa fa-clock-o"></i>' +
                    '        به زودی' +
                    '    </div>' +
                    '</div>';
                break;
            case 2:
                result = '<div class="row">' +
                    '    <div class="price-contactus">' +
                    '        <i class="fa fa-phone"></i>' +
                    '        تماس بگیرید' +
                    '    </div>' +
                    '</div>';
                break;
            default:
                break;
        }
    }
    else if (minPrice) {
        if (minPrice.DiscountPercent > 0) {
            result = '<div class="original-price">' +
                     toPrice(minPrice.Price, "{0} <span class='price-unit'>{1}</span>") +
                     '</div>' +
                     '<div class="topay-price">' +
                     toPrice(minPrice.DiscountPrice, "{0} <span class='price-unit'>{1}</span>") +
                     '</div>';
        }
        else {
            result = '<div class="regular-price">' +
                     toPrice(minPrice.Price, "{0} <span class='price-unit'>{1}</span>") +
                     '</div>';
        }
    }

    return result;
}

function renderScore(sum, count, adminRate) {
    var result = '';

    var totalScore = calcRaty(sum, count, adminRate);
    var intValue = parseInt(totalScore);
    var decimalValue = totalScore - intValue;
    var counter = 0;

    result = "<a href='#' class='rate'>";
    for (counter = 0; counter < intValue; counter++) {
        result += "<img src='/assets/raty/images/star-on.png' />";
    }
    if (decimalValue != 0) {
        result += "<img src='/assets/raty/images/star-half.png' />";
        counter++;
    }
    for (var i = 0; i < 5 - counter; i++) {
        result += "<img src='/assets/raty/images/star-off.png' />";
    }

    return result += "</a>";

}

function calcRaty(sum, count, adminRate) {
    var result;

    if (count == 0 && adminRate == 0) {
        return 0;
    }

    var result = (sum + adminRate) / (count + 1);

    if (result > 5) {
        result = 5;
    }

    return result;
}

function getMarks(item) {
    var result = '';

    if (item.Marks) {

        result = "<div class='product_icon'>";
        for (var i = 0; i < item.Marks.length; i++) {

            var mark = item.Marks[i];
            result += "<div class='mark' style='background-color:" + mark.Color + "'><span>" + mark.Title + "</span></div>";
        }

        result += "</div>";
    }

    return result;
}