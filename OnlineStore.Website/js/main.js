var staticTexts = {
    RequestError: 'رخداد خطا',
    ResponseError: 'رخداد خطا',
    MaxPriceFreeDelivery: 'رایگان'
};

var staticTemplates = {
    Loading: '<div class="loading"><img src="/images/loading.gif" alt="لطفا صبر کنید" /></div>'
};

var staticValues = {
    MaxPriceFreeDelivery: false
};

var $body = $('body');

function isFreeDelivery(price) {
    var hasFreeDeliveryProduct = false;
    _.find(cartItems, function (item) {
        if (item.IsFreeDelivery == true)
            hasFreeDeliveryProduct = true;
    });

    if ((staticValues.MaxPriceFreeDelivery && price >= 10000) || hasFreeDeliveryProduct)
        return true;
    else
        return false;
}

function toPrice(input, format) {
    if (input != 'null') {
        if (!isRial)
            input = (input / 10);

        if (format)
            return format.replace('{0}', commaSeparate(input))
                         .replace('{1}', (isRial ? ' ریال' : ' تومان'));
        else
            return commaSeparate(input) + (isRial ? ' ریال' : ' تومان');
    }
    else {
        return 'نامعلوم';
    }
}

function commaSeparate(input) {
    while (/(\d+)(\d{3})/.test(input.toString())) {
        input = input.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }

    return input;
}

function normalizeForUrl(str) {
    if (str != null) {
        str = str.replace(/ /g, '-');
        str = str.replace(',', '-');
        str = str.replace('،', '-');
        str = str.replace('/', '-');
        str = str.replace('\\', '-');
        str = str.replace('+', '-');

        while (str.indexOf("--") > 0) {
            str = str.replace("--", "-");
        }

        str = str.trim('-');
    }

    return str;
}

function blogUrl(id, title, group) {
    return "/Blog/" + normalizeForUrl(group) + "/" + normalizeForUrl(title) + "-" + id;
}

function productGroupUrl(title) {
    return "/Products/" + normalizeForUrl(title);
}

function getCompareUrl(productID) {
    var url = "/CompareProducts/" + productID;

    return url;
}

// validate
if ($.fn.validate) {
    $(".validate").validate({
        submitHandler: function (form) {
            var $form = $(form);
            if (!$form.hasClass('ajax'))
                form.submit();
        }
    });
}

function validateEmail(input) {
    var email = /^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;
    return email.test(input);
}

function validateMobile(input) {
    var mobile = /^([0|\+[0-9]{1,5})?([7-9][0-9]{9})$/;
    return mobile.test(input);
}

// PersianDateTimePicker
if ($.fn.MdPersianDateTimePicker) {
    $('.persiandate').MdPersianDateTimePicker({
        EnableTimePicker: false
    }).on('keydown', function (e) {
        if (e.keyCode != 9)
            e.preventDefault();
    });
}

// iCheck
if ($.fn.iCheck) {
    $('input[type="checkbox"]').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });

    $('input[type="radio"]').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });
}

toastr.options = {
    "positionClass": "toast-bottom-right",
};

// carousel btn
$('.btn-next').on('click', function (e) {
    e.preventDefault();
    var $this = $(this);

    $this.closest('.box-header').parent().find('.owl-carousel').trigger('prev.owl.carousel');
});

$('.btn-prev').on('click', function (e) {
    e.preventDefault();
    var $this = $(this);

    $this.closest('.box-header').parent().find('.owl-carousel').trigger('next.owl.carousel');
});

$('.simple-carousel').owlCarousel({
    slideSpeed: 200,
    singleItem: true,
    autoplay: false,
    autoplayHoverPause: false,
    items: 1
});

// mini menu
$('.mini-menu').on('click', function (e) {
    e.preventDefault();

    $(this).closest('.top-menu').toggleClass('open');
});

$(window).on('resize', function (e) {
    var width = $(window).width();

    if (width >= 1024)
        $('.top-menu').removeClass('open');
});

$('.tooltipster').tooltipster({
    theme: 'tooltipster-dd'
});

$('.tooltipster-right').tooltipster({
    theme: 'tooltipster-dd',
    position: 'right'
});

$('.tooltipster-top').tooltipster({
    theme: 'tooltipster-dd',
    position: 'top'
});

$('.tooltipster-left').tooltipster({
    theme: 'tooltipster-dd',
    position: 'left'
});

$(function () {
    // MegaMenuSlider
    $('#MegaMenuSlider').owlCarousel({
        paginationSpeed: 500,
        autoplay: true,
        loop: true,
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        smartSpeed: 300,
        items: 1
    });

    // Basket
    var $basketContent = $('.basket-content');

    $('.right-menu ul.root > li').on('mouseenter', function (e) {
        e.preventDefault();
        var $this = $(this);

        var width = $(window).width();
        if (width >= 690)
            $this.children('.submenu').stop(true, false).fadeIn();

    }).on('mouseleave', function (e) {
        e.preventDefault();
        var $this = $(this);

        var width = $(window).width();
        if (width >= 690)
            $this.children('.submenu').hide();

    }).find('> a').on('click', function (e) {
        var $this = $(this);

        if ($this.next().length > 0)
            e.preventDefault();

        var width = $(window).width();
        if (width >= 690) {
            $this.closest('.right-menu').find('.open').removeClass('open');
            return;
        }

        if ($this.parent().hasClass('open'))
            return;

        $this.closest('.right-menu').find('.open')
             .find('.submenu').slideUp({
                 complete: function () {
                     $(this).parent().removeClass('open');
                 }
             });

        $this.parent().addClass('open')
             .find('.submenu').slideDown();
    });

    var $miniCart = $('.mini-cart');
    $('.btn-cart').on('click', function (e) {
        e.preventDefault();

        if ($miniCart.hasClass('open')) {
            $miniCart.removeClass('open');
            $basketContent.stop(true, false).slideUp(300);
        }
        else {
            $miniCart.addClass('open');
            $basketContent.stop(true, false).slideDown(300);
        }
    });

    $('.support, .account').on('mouseenter', function (e) {
        e.preventDefault();
        var $this = $(this);

        $this.find('.content-box').stop(true, false).slideDown(300);
    }).on('mouseleave', function (e) {
        var $this = $(this);

        $this.find('.content-box').stop(true, false).slideUp(300);
    }).children('a').on('click', function (e) {
        e.preventDefault();
    });

    $body.on('click', function (e) {
        var $this = $(e.target);

        if ($this.hasClass('.mini-cart') || $this.closest('.mini-cart').length == 0) {
            $miniCart.removeClass('open');
            $basketContent.stop(true, false).slideUp(300);
        }
    });

    var $megamenubg = $('.mega-menu-bg'),
        $megamenubox = $('.mega-menu-box'),
        $megamenuhandle = $('.mega-menu-handle'),

        $megamenu = $('.mega-menu'),
        $mainlevels = $megamenu.find('.mainlevels'),
        $submenus = $megamenu.find('.submenus');

    $megamenuhandle.on('click', function (e) {
        e.preventDefault();

        $megamenuhandle.toggleClass('open');
        $body.toggleClass('megamenu-open');

        $megamenubox.toggle();
        $megamenubg.toggle();

        if ($megamenuhandle.hasClass('open')) {
            //$('html, body').animate({ 'scroll-top': $('.page-title-box').position().top - 10 });
        }
    });

    $megamenubg.add($megamenubox).on('click', function (e) {
        if (e.target == $megamenubg[0] || e.target == $megamenubox[0]) {
            $megamenuhandle.removeClass('open');
            $body.removeClass('megamenu-open');

            $megamenubox.hide();
            $megamenubg.hide();
        }
    });

    $mainlevels.on('mouseenter focus', 'li', function (e) {
        e.preventDefault();

        var $this = $(this),
            id = $this.data('id');

        $this.addClass('active').siblings('li').removeClass('active');
        $submenus.find('[data-id="' + id + '"]').addClass('active').siblings('.menu-submenu').removeClass('active');
    }).on('click', 'li', function (e) {
        e.preventDefault();

        if ($(window).width() < 960) {
            var $this = $(this),
                id = $this.data('id');

            var $li = $submenus.find('[data-id="' + id + '"]');

            $('html, body').animate({ 'scroll-top': $li.position().top - 10 });
        }
    });

    var $gototop = $('.goto-top');
    $(window).on('scroll', function (e) {
        var top = $(window).scrollTop();

        if (top > 300) {
            if (!$gototop.prop('visible'))
                $gototop.prop('visible', true).animate({ right: '20px', opacity: 0.8 }, 100);
        }
        else {
            if ($gototop.prop('visible'))
                $gototop.prop('visible', false).animate({ right: '-60px', opacity: 0 }, 100);
        }
    });

    $gototop.on('click', function () {
        $('html, body').animate({ 'scroll-top': 0 });
    });
});