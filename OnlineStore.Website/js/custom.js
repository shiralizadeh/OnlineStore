jQuery(document).ready(function () {

    "use strict";

    /* Mega Menu */
    $('.mega-menu-title').on('click', function () {
        if ($('.mega-menu-category').is(':visible')) {
            console.log('1');
            $('.mega-menu-category').slideUp();
        } else {
            $('.mega-menu-category').slideDown();
            console.log('2');
        }
    });


    $('.mega-menu-category .nav > li').hover(function () {
        $(this).addClass("active");
        $(this).find('.popup').stop(true, true).fadeIn('fast');
    }, function () {
        $(this).removeClass("active");
        $(this).find('.popup').stop(true, true).fadeOut('fast');
    });


    $('.mega-menu-category .nav > li.view-more').on('click', function (e) {
        if ($('.mega-menu-category .nav > li.more-menu').is(':visible')) {
            $('.mega-menu-category .nav > li.more-menu').stop().slideUp();
            $(this).find('a').text('More category');
        } else {
            $('.mega-menu-category .nav > li.more-menu').stop().slideDown();
            $(this).find('a').text('Close menu');
        }
        e.preventDefault();
    });

    /* Brands Slider */
    $("#brands .owl").owlCarousel({
        autoPlay: false,
        items: 5,
        itemsDesktop: [1199, 4],
        itemsDesktopSmall: [991, 3],
        itemsTablet: [767, 2],
        itemsMobile: [480, 2],
        //slideSpeed: 1000,
        //paginationSpeed: 1000,
        //rewindSpeed: 1000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });


    /* timely-owl */
    $("#timely-owl .owl").owlCarousel({
        autoPlay: false,
        items: 1,
        itemsDesktop: [1199, 1],
        itemsDesktopSmall: [991, 1],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 1000,
        //paginationSpeed: 1000,
        //rewindSpeed: 1000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });
    /* special-offer slider */
    $("#special-offer .owl").owlCarousel({
        autoPlay: false,
        items: 1,
        itemsDesktop: [1199, 1],
        itemsDesktopSmall: [991, 1],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 3000,
        //paginationSpeed: 3000,
        //rewindSpeed: 3000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });
    /* latest-news slider */
    $("#latest-news .owl").owlCarousel({
        autoPlay: false,
        items: 1,
        itemsDesktop: [1199, 1],
        itemsDesktopSmall: [991, 1],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 1000,
        //paginationSpeed: 1000,
        //rewindSpeed: 1000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });
    /* clients-say slider */
    $("#clients-say .owl").owlCarousel({
        autoPlay: false,
        items: 1,
        itemsDesktop: [1199, 1],
        itemsDesktopSmall: [991, 1],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 3000,
        //paginationSpeed: 3000,
        //rewindSpeed: 3000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });
    /* featured-products slider */
    $("#featured-products .owl").owlCarousel({
        autoPlay: false,
        items: 4,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [991, 2],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 3000,
        //paginationSpeed: 3000,
        //rewindSpeed: 3000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });
    /* new-products slider */
    $("#new-products .owl").owlCarousel({
        autoPlay: false,
        items: 4,
        itemsDesktop: [1199, 3],
        itemsDesktopSmall: [991, 2],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 3000,
        //paginationSpeed: 3000,
        //rewindSpeed: 3000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });

    /* recent-post slider */
    $("#recent-post .owl").owlCarousel({
        autoPlay: false,
        items: 1,
        itemsDesktop: [1199, 1],
        itemsDesktopSmall: [991, 1],
        itemsTablet: [767, 2],
        itemsMobile: [480, 1],
        //slideSpeed: 3000,
        //paginationSpeed: 3000,
        //rewindSpeed: 3000,
        navigation: true,
        stopOnHover: true,
        pagination: false,
        scrollPerPage: true,
    });

    /* .thumbnail-container product image slider */
    if ($.fn.bxSlider)
        $('.thumbnail-container .bxslider').bxSlider({
            slideWidth: 94,
            slideMargin: 5,
            minSlides: 4,
            maxSlides: 4,
            pager: false,
            speed: 500,
            pause: 3000
        });


});