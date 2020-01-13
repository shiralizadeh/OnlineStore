$(function () {
    var $searchBox = $('.main-header .search-box'),
        $key = $searchBox.find('.input-search'),
        $resultBox = $searchBox.find('.result-box'),
        $products = $searchBox.find('.result-box .products .result'),
        $blogs = $searchBox.find('.result-box .blogs .result'),
        $groups = $searchBox.find('.result-box .groups .result'),
        $notFound = $searchBox.find('.not-found'),
        $productGroup = $('.product-group'),
        $allGroup = $('.all-group');

    var resolution = $(window).width();

    if (resolution >= 690) {
        var getProductsTimer = undefined;
        $key.bind('paste keyup', function () {
            if (getProductsTimer)
                clearTimeout(getProductsTimer);

            if ($key.val().trim().length > 1) {
                getProductsTimer = setTimeout(function () {

                    var $loading = $(staticTemplates.Loading);
                    $searchBox.append($loading);

                    $.ajax({
                        type: 'POST',
                        url: '/Search/SimpleSearch',
                        data: {
                            Key: $key.val().trim()
                        },
                        success: function (response) {
                            if (response.Success) {
                                renderProducts(response.Data.Products);
                                renderBlogs(response.Data.Blogs);
                                renderGroups(response.Data.Groups);
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
                            $resultBox.slideDown();
                        }
                    });

                }, 800);
            }
            else {
                $resultBox.slideUp();
            }

        });
    }

    function renderProducts(products) {
        if (products.length == 0) {
            $products.html('<span class="not-found">موردی یافت نشد.</span>');
        }
        else {
            $products.empty();
            var $ul = $('<ul>');

            for (var i = 0; i < products.length; i++) {

                var item = products[i];

                var li = '<li><a href="' + item.Url + '" title="' + item.DisplayTitle + '"><img src="' + item.Image + '">'
                + '<div class="title-box"><p>' + item.Title_En + '</p><p>' + item.Title_Fa + '</p>'
                + '</div></a></li>';

                $ul.append(li);
            }

            $products.append($ul);
        }
    }

    function renderBlogs(blogs) {
        if (blogs.length == 0) {
            $blogs.html('<span class="not-found">موردی یافت نشد.</span>');
        }
        else {
            $blogs.empty();
            var $ul = $('<ul>');

            for (var i = 0; i < blogs.length; i++) {

                var item = blogs[i];

                var li = '<li><a href="' + blogUrl(item.ID, item.Title, item.GroupTitle) + '" title="' + item.Title + '"><img src="' + item.Image + '">'
                + '<div class="title-box"><p>' + item.Title + '</p>'
                + '</div></a></li>';

                $ul.append(li);
            }

            $blogs.append($ul);
        }
    }

    function renderGroups(groups) {
        if (groups.length == 0) {
            $groups.html('<span class="not-found">موردی یافت نشد.</span>');
        }
        else {
            $groups.empty();
            var $ul = $('<ul>');

            for (var i = 0; i < groups.length; i++) {

                var item = groups[i];

                var li = '<li><a href="' + productGroupUrl(item.TitleEn) + '" title="' + item.Title + '">' + item.Title + ' (' + item.TitleEn + ')</a></li>';

                $ul.append(li);
            }

            $groups.append($ul);
        }
    }

    $('body').on('click', function () {
        if ($resultBox.is(':visible')) {
            $resultBox.slideUp();
        }
    });

    $productGroup.on('click', 'a', function () {
        var $this = $(this);

        $allGroup.children('span:first').text($this.text());
        groupID = $this.data('id');
    });

    // Scroll
    $(".result-box .result").niceScroll({ cursorcolor: "#cfd8dc" });
});