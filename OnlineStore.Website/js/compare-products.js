/// <reference path="jquery-1.11.3.min.js" />
$(function () {

    var $key = $('.search-box #Search'),
        $search = $('.search-box'),
        $resultBox = $('.search-box .result-box'),
        $notFound = $('.search-box .not-found'),
        $productCompare = $('.product-compare'),
        $productIDs = $('#ProductIDs'),
        $rows = $('.product-compare table tbody tr'),
        $headers = $('.product-compare table thead'),
        newProduct = -1,
        attrItems = [],
        products = $productIDs.val().split(',');

    renderSticker();
    makeRaty();
    // جستجوی محصول
    var getProductsTimer = undefined;
    $key.bind('keyup paste', function () {
        if (getProductsTimer)
            clearTimeout(getProductsTimer);

        if ($key.val().trim().length > 1) {

            getProductsTimer = setTimeout(function () {

                var $loading = $(staticTemplates.Loading);
                $search.append($loading);

                $.ajax({
                    type: 'POST',
                    url: '/CompareProducts/Search',
                    data: {
                        GroupID: mainGroupID,
                        Key: $key.val()
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

                                    var li = '<li><a href="#" data-id="' + item.ID + '" title="' + item.DisplayTitle + '"><img src="' + item.Image + '">'
                                            + '<div class="title-box"><p>' + item.Title_En + '</p><p>' + item.Title_Fa + '</p>'
                                            + '</div></a></li>';

                                    $ul.append(li);
                                }

                                $resultBox.append($ul);
                            }
                        }
                        else {
                            alert('خطا');
                        }
                    },
                    error: function () {
                        //alert('خطا');
                    },
                    complete: function () {
                        $loading.remove();
                        $resultBox.slideDown();
                    }
                });
            }, 800);
        }
        else {
            $resultBox.slideUp();
        }

    });

    // انتخاب محصول و اضافه به لیست مقایسه
    $resultBox.on('click', 'a', function (e) {
        e.preventDefault();

        var $this = $(this);

        $resultBox.hide();
        newProduct = $this.data('id');

        compare();

        var state = {};
        var url = 'http://online-store.com/CompareProducts/';
        url = url + products.join(',');

        history.pushState(state, '', url);

    });

    // حذف از لیست مقایسه
    $headers.on('click', '.fa-times-circle', function () {

        if (products.length <= 1) {
            toastr.warning('حذف محصول امکان پذیر نمی باشد!', 'مقایسه کالا');
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $productCompare.append($loading);

        var $this = $(this);
        var id = $this.closest('td').data('id');

        $rows.find('td[data-id="' + id + '"]').remove();
        $headers.find('td[data-id="' + id + '"]').remove();

        products = _.filter(products, function (item) {
            return item != id;
        });

        $loading.remove();
        renderSticker();

        var state = {};
        var url = 'http://online-store.com/CompareProducts/';
        url = url + products.join(',');
        history.pushState(state, '', url);

        $(window).resize();
    });


    function renderTotalScore() {
        $('.rate').raty({
            half: true,
            readOnly: true,
            score: function () {
                return $(this).attr('data-score');
            }
        });
    }

    function renderAttributes(attributes) {
        attrItems = [];
        for (var i = 0; i < attributes.length; i++) {
            var item = attributes;
            attrItems.push({
                Value: item[i].Value,
                ID: item[i].ID,
                Perfix: item[i].Perfix,
                Posfix: item[i].Posfix
            });
        }

        $rows.each(function () {
            var $this = $(this),
                id = $this.data('id'),
                str = '';

            if (typeof (id) != 'undefined') {

                var newItem = _.find(attrItems, function (item) {
                    return id == item.ID;
                });

                if (newItem.Value != null && newItem.Value != '') {
                    if (newItem.Perfix != null) {
                        str += newItem.Perfix + ' ';
                    }
                    str += newItem.Value;
                    if (newItem.Posfix != null) {
                        str += ' ' + newItem.Posfix;
                    }
                }

                $this.append('<td data-id="' + newProduct + '">' + str + '</td>');
            }

        });

    }

    function renderScoreAverage(scoresAverages) {
        var tUl = $('<ul />');
        var td = $('<td data-id="' + newProduct + '" />');

        for (var i = 0; i < scoresAverages.length; i++) {
            var item = scoresAverages;
            tUl.append('<li><span>' + item[i].Title + '</span>:<span>' + item[i].Average + '</span></li>');
        }

        $rows.filter(':nth-child(2)').append(td.append(tUl));
    }

    function compare() {

        var repeat = _.find(products, function (item) {
            return item == newProduct;
        });

        if (typeof (repeat) != 'undefined') {
            toastr.warning('محصول مورد نظر در لیست موجود است!', 'مقایسه کالا');
            $key.val('')
                .removeAttr('disabled');
            return;
        }

        if (products.length >= 5) {
            toastr.error('مقایسه ی بیش از 5 محصول امکان پذیر نمی باشد!', 'مقایسه کالا');
            $key.val('')
                .removeAttr('disabled');
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $productCompare.append($loading);

        products.push(newProduct);

        $.ajax({
            type: 'POST',
            url: '/CompareProducts/AddToCompare',
            data: {
                GroupID: mainGroupID,
                newProductID: newProduct
            },
            success: function (response) {
                if (response.Success) {
                    var dataItem = response.Data;

                    // Image
                    $headers.children(':nth-child(1)').append('<td data-id="' + newProduct + '"><a title="' + dataItem.ToolTip + '" href="' + dataItem.Url + '"><div class="product-detail"><img src="' + dataItem.Image + '" /><span class="title">' + dataItem.DisplayTitle + '</span><a class="fa fa-times-circle"></a></div></a></td>');

                    // Total Score
                    var ratyElement = '<a data-score="' + dataItem.Score + '" class="rate"></a><span class="preferentials">( از ' + dataItem.PreferentialsCount + ' رأی )</span>';
                    $rows.filter(':nth-child(1)').append('<td class="score" data-id="' + newProduct + '">' + ratyElement + '</td>');

                    // Price
                    var strPrice = renderPrice(dataItem);
                    $rows.filter(':nth-child(3)').append('<td data-id="' + newProduct + '">' + strPrice + '</td>');

                    // Attributes
                    if (dataItem.Attributes.length > 0) {
                        renderAttributes(dataItem.Attributes);
                    }

                    // Average Score
                    renderScoreAverage(dataItem.ScoresAverages);
                }
            },
            error: function () {
                alert('خطا');
            },
            complete: function () {
                $loading.remove();
                renderTotalScore();
                renderSticker();
                $key.val('');
                $(window).resize();
                makeRaty();
            }
        });
    }

    function renderSticker() {
        $headers.find('.ts-row-fixed').remove();
        $(document).tableSection();
    }

    function renderPrice(item) {
        var prices = item.Prices,
            minPrice = undefined,
            maxPrice = undefined;

        if (item.IsUnavailable) {
            result = "<div class='price-isunavailable'>" +
                     "<i class='fa fa-times-circle'></i> موجود نیست</div>";
        }
        else {
            if (prices != null && prices.length > 0) {
                minPrice = prices[0];
                maxPrice = prices[prices.length - 1];
            }

            if (minPrice) {
                if (minPrice.DiscountPercent > 0) {
                    result = "<div class='original-price'>" +
                                "<span>" + toPrice(minPrice.Price, "{0}{1}") + "</span>" +
                            "</div>" +
                            "<div class='topay-price'>" +
                                "<span>" + toPrice(minPrice.DiscountPrice, "{0}{1}") + "</span>" +
                            "</div>";
                }
                else {
                    result = "<div class='regular-price'>" +
                                "<span>" + toPrice(minPrice.Price, "{0}{1}") + "</span>" +
                            "</div>";
                }
            }
            else {
                result = "<div class='regular-price'>" +
                                "<span>" + toPrice(0, "{0}{1}") + "</span>" +
                "</div>";
            }
        }

        return result;

    }

    function makeRaty() {
        $('.score .rate').raty({
            half: true,
            readOnly: false,
            score: function () {
                return $(this).attr('data-score');
            },
            click: function (rate) {
                var $this = $(this);
                var $loading = $(staticTemplates.Loading);
                var $pane = $this.closest('.score');
                $pane.append($loading);

                var productID = $this.closest('td').data('id');

                $.ajax({
                    type: 'POST',
                    url: '/Products/InsertRate',
                    data: {
                        ProductID: productID,
                        Rate: rate
                    },
                    success: function (result) {
                        if (result.Success) {
                            toastr.success('با تشکر از امتیاز شما', 'امتیازدهی');
                        }
                        else if (result.Errors[0] == "Repeat") {
                            toastr.error('قبلا برای این محصول امتیاز ثبت کرده اید! ', 'امتیازدهی');
                        }
                    },
                    error: function (response) {
                        toastr.error(staticTexts.RequestError, 'امتیازدهی');
                    },
                    complete: function () {
                        $loading.remove();
                    }
                });
            }
        });
    }

});
