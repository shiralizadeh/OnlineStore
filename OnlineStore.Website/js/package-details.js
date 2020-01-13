$(function () {
    var $tabs = $('.product-tabs'),
        $packageDetails = $('.package-details'),
        $controls = $('.product-controls'),

        $boxHeader = $('.product-tabs .box-header'),
        $boxContent = $('.product-tabs .box-content'),
        $tabBoxes = $('#TabBoxes'),

        $rate = $('.product-details .rate-comment .rate'),
        productID = $('.product-details').data('productid'),

        productname = $('.product-title h1').text();

    makeRaty();

    // Product Gallery
    $(".cloudzoom").CloudZoom({
        zoomPosition: 12
    });

    $(".cloudzoom-gallery").CloudZoom();

    $('#ProductGallery').Thumbelina({
        $bwdBut: $('#ProductGallery .left'),
        $fwdBut: $('#ProductGallery .right')
    });

    // Tabs
    $boxContent.children('div').each(function () {
        var $this = $(this);
        var tagH2 = $this.find('h2'),
            $copyH2 = tagH2.clone();

        $copyH2.find('span').remove();

        $boxHeader.append('<a href="#' + $this.attr("ID") + '">' + $copyH2.text() + '</a>');

        tagH2.addClass('hide');
    });

    $boxHeader.append('<div class="clear"></div>');

    $boxHeader.children('a:first').addClass('selected');
    $boxContent.children('div:first').show()

    $boxHeader.on("click", 'a', function (e) {
        e.preventDefault();

        $boxContent.children('div').hide();
        $boxHeader.find('a').removeClass('selected');

        var $this = $(this);
        $this.addClass('selected');
        var tabID = $(this).attr('href');

        $('html,body').animate({
            scrollTop: $tabs.offset().top
        }, 600);

        $(tabID).show();
        $(window).resize();
    });

    // Sticky
    $tabBoxes.sticky({ topSpacing: 0 });
    $tabBoxes.on('sticky-start', function () {
        var elem = $('<div class="hidden-title">').append($('.product-features h1').children().clone());
        $(this).children('.clear').before(elem);
    });
    $tabBoxes.on('sticky-end', function () {
        $(this).children('div.hidden-title').remove();
    });

    function makeRaty() {
        $rate.raty({
            half: true,
            readOnly: true,
            score: function () {
                return $(this).attr('data-score');
            },
        });
    }

    $('.package-details .btn-add-cart').on('click', function (e) {
        e.preventDefault();

        var $this = $(this);

        var $loading = $(staticTemplates.Loading);
        $controls.append($loading);

        var packageid = $packageDetails.data('packageid');

        $.ajax({
            type: "POST",
            url: "/Cart/Add",
            data: {
                PackageID: packageid
            },
            success: function (response) {
                if (response.Success) {
                    var result = response.Data;

                    if (result.Exists)
                        toastr.warning('شما \'' + productname + '\'  را به <a href="/Cart">سبد خرید</a> اضافه کرده اید.', 'سبد خرید');
                    else
                        toastr.success('\'' + productname + '\' به <a href="/Cart">سبد خرید</a> اضافه شد.', 'سبد خرید');

                    renderMiniCart(result);
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'سبد خرید');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'سبد خرید');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });
});
