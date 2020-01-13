// Loading
var staticTemplates = {
    Loading: '<div class="loading"><img src="/images/loading.gif" alt="لطفا صبر کنید" /></div>'
};

(function () {

    // pageLoaded
    if (window.pageLoaded) {
        window.pageLoaded();
    }

    // Defaults
    $.fn.selectpicker.defaults = { noneSelectedText: 'انتخاب کنید' };

    // sidebar dropdown menu
    jQuery('#sidebar .sub-menu > a').click(function () {
        var last = jQuery('.sub-menu.open', $('#sidebar'));
        last.removeClass("open");
        jQuery('.arrow', last).removeClass("open");
        jQuery('.sub', last).slideUp(200);
        var sub = jQuery(this).next();
        if (sub.is(":visible")) {
            jQuery('.arrow', jQuery(this)).removeClass("open");
            jQuery(this).parent().removeClass("open");
            sub.slideUp(200);
        } else {
            jQuery('.arrow', jQuery(this)).addClass("open");
            jQuery(this).parent().addClass("open");
            sub.slideDown(200);
        }
    });

    $(function () {
        function responsiveView() {
            var wSize = $(window).width();
            if (wSize <= 768) {
                $('#container').addClass('sidebar-close');
                $('#sidebar > ul').hide();
            }

            if (wSize > 768) {
                $('#container').removeClass('sidebar-close');
                $('#sidebar > ul').show();
            }
        }
        $(window).on('load', responsiveView);
        $(window).on('resize', responsiveView);
    });

    // sidebar toggle
    $('.icon-reorder').click(function () {
        if ($('#sidebar > ul').is(":visible") === true) {
            $('#main-content').css({
                'margin-right': '0px'
            });
            $('#sidebar').css({
                'margin-right': '-220px'
            });
            $('#sidebar > ul').hide();
            $("#container").addClass("sidebar-closed");
        } else {
            $('#main-content').css({
                'margin-right': '220px'
            });
            $('#sidebar > ul').show();
            $('#sidebar').css({
                'margin-right': '0'
            });
            $("#container").removeClass("sidebar-closed");
        }
    });

    // custom scrollbar
    $("#sidebar").niceScroll({ styler: "fb", cursorcolor: "#e8403f", cursorwidth: '3', cursorborderradius: '10px', background: '#404040', cursorborder: '' });
    //$("html").niceScroll({ styler: "fb", cursorcolor: "#e8403f", cursorwidth: '6', cursorborderradius: '10px', background: '#404040', cursorborder: '', zindex: '1000' });

    // tool tips
    $('.tooltips').tooltip();

    // popovers
    $('.popovers').popover();

    // validate
    $(".validate").validate({
        submitHandler: function (form) {
            var $form = $(form);
            if ($form.hasClass('ajax')) {
                var data = $form.serialize(),
                    url = $form.attr('action');

                if (window.ajaxSend) {
                    ajaxSend(data);
                }

                $form.find('.alert').fadeOut();
                $form.find('.loading').fadeIn();

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: data,
                    success: function (result) {
                        if (result.Success) {
                            $form.find('.alert-success').fadeIn();
                            $form[0].reset();

                            if (window.ajaxSuccess) {
                                ajaxSuccess(result);
                            }
                        }
                        else {
                            var $ul = $form.find('.alert-danger ul');
                            $ul.empty();
                            for (var i in result.Errors) {
                                var msg = result.Errors[i];

                                $ul.append('<li>' + msg + '</li>');
                            }
                            $ul.parent('.alert-danger').fadeIn();
                        }
                    },
                    error: function () {
                        alert('رخداد خطا');
                    },
                    complete: function () {
                        $form.find('.loading').fadeOut();
                    }
                });
            }
            else
                form.submit();
        }
    });

    // iCheck
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

    // PersianDateTimePicker
    $('.persiandate').MdPersianDateTimePicker({
        EnableTimePicker: false
    }).on('keydown', function (e) {
        if (e.keyCode != 9)
            e.preventDefault();
    });

    // Filter Box
    $(function () {
        var $filterBox = $('.filter-box'),
            $controlBox = $('.filter-box .control-box'),
            $icon = $('.filter-box i.open-icon');

        $filterBox.on('click', '.filter-header', function () {
            if ($controlBox.css('display') == 'block') {
                $icon.removeClass('icon-minus-sign').addClass('icon-plus-sign');
            }
            else {
                $icon.removeClass('icon-plus-sign').addClass('icon-minus-sign');
            }

            $controlBox.slideToggle();
        })
    });

    $(function () {
        var timer = setInterval(function () {
            $.ajax({
                url: '/Admin/Dashboard/IsLogin',
                success: function (result) {
                    if (result != 'True') {
                        clearInterval(timer);
                        if (confirm('شما از سیستم خارج شده اید.\nآیا مایل به ورود دوباره هستید؟'))
                            window.location = '/Admin/Dashboard';
                    }
                }
            });
        }, 10000);
    });

})();