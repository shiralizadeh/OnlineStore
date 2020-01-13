var firstState = _.clone(productsListOptions),
    $productItemTemplate = $('#ProductItemTemplate'),
    $productslist = $('.products-list'),
    $pagingBox = $('.paging-box'),
    $paging = $('.paging'),
    $alertEmpty = $('.alert-empty');

$paging.on('click', 'a', function (e) {
    e.preventDefault();

    var $this = $(this),
        index = $this.data('index');

    productsListOptions.PageIndex = index;
    refresh(true);
});

function refresh(pushState) {
    loadProducts(pushState);
}

function loadProducts(pushState) {
    var $loading = $(staticTemplates.Loading);
    $productslist.append($loading);

    $('html, body').animate({ 'scrollTop': $('.page-title-box').position().top });

    $.ajax({
        url: '/Packages/',
        type: 'POST',
        data: productsListOptions,
        success: function (result) {
            if (result.Success) {
                var data = result.Data;
                var $ul = $productslist.find('ul');

                $ul.empty();

                var products = data.Packages;
                var totalPages = data.TotalPages;

                for (var i in products) {
                    var item = products[i],
                        tmp = $productItemTemplate.text();

                    var discountPercent = (item.NewPrice * 100) / item.OldPrice;

                    tmp = tmp.replace(/{{ID}}/g, item.ID);
                    tmp = tmp.replace(/{{Title}}/g, item.Title);
                    tmp = tmp.replace(/{{Url}}/g, item.Url);
                    tmp = tmp.replace(/{{ImageFile}}/g, item.ImageFile);
                    tmp = tmp.replace(/{{Price}}/g, renderPrice(item.OldPrice, item.NewPrice, discountPercent));
                    tmp = tmp.replace(/{{ToolTip}}/g, item.Title);
                    tmp = tmp.replace(/{{Score}}/g, renderScore(item.PackageScore));

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
                var originalUrl = '/Packages';
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

                var state = _.clone(productsListOptions);

                if (pushState) {
                    history.pushState(state, "", currentPageIndex + 1);
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

function renderPrice(price, discountPrice, discountPercent) {
    var result;

    if (discountPercent > 0) {
        result = '<div class="original-price">' +
                 toPrice(price, "{0} <span class='price-unit'>{1}</span>") +
                 '</div>' +
                 '<div class="topay-price">' +
                 toPrice(discountPrice, "{0} <span class='price-unit'>{1}</span>") +
                 '</div>';
    }
    else {
        result = '<div class="regular-price">' +
                 toPrice(price, "{0} <span class='price-unit'>{1}</span>") +
                 '</div>';
    }

    return result;
}

function renderScore(adminRate) {
    var result = '';

    var totalScore = adminRate;
    var intValue = parseInt(adminRate);
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
