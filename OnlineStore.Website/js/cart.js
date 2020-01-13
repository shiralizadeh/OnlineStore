var miniCartTemplate = $('#MiniCartTemplate').text();

$('body').on('click', '.btn-minicart-remove', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var $this = $(this),
        $cartItem = $this.closest('.cart-item'),
        productname = $cartItem.find('h2').text(),
        productID = $cartItem.data('productid'),
        productVarientID = $cartItem.data('productvarientid'),
        packageID = $cartItem.data('packageid');

    if (productVarientID == '{{ProductVarientID}}')
        productVarientID = undefined;

    if (productID == '{{ProductID}}')
        productID = undefined;

    if (packageID == '{{PackageID}}')
        packageID = undefined;

    var $loading = $(staticTemplates.Loading);
    $cartItem.append($loading);

    $.ajax({
        type: "POST",
        url: "/Cart/Remove",
        data: {
            ProductVarientID: productVarientID,
            ProductID: productID,
            PackageID: packageID,
        },
        success: function (response) {
            if (response.Success) {
                var result = response.Data;

                if (result.Exists)
                    toastr.success('\'' + productname + '\' از <a href="/Cart">سبد خرید</a> حذف شد.', 'سبد خرید');
                else
                    toastr.warning('شما \'' + productname + '\'  را از <a href="/Cart">سبد خرید</a> حذف کرده اید.', 'سبد خرید');

                if (productID)
                    $('tr[data-productid="' + productID + '"]').remove();

                if (productVarientID)
                    $('tr[data-productvarientid="' + productVarientID + '"]').remove();

                if (productVarientID)
                    $('tr[data-packageid="' + productVarientID + '"]').remove();

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

$('body').on('click', '.btn-cart-remove', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var $this = $(this),
        $tr = $this.closest('tr'),
        productID = $tr.data('productid'),
        productVarientID = $tr.data('productvarientid'),
        productname = $tr.find('.product-url').text();

    var $loading = $(staticTemplates.Loading);
    $tr.append($loading);

    $.ajax({
        type: "POST",
        url: "/Cart/Remove",
        data: {
            ProductID: productID,
            ProductVarientID: productVarientID
        },
        success: function (response) {
            if (response.Success) {
                var result = response.Data;

                if (result.Exists)
                    toastr.success('\'' + productname + '\' از <a href="/Cart">سبد خرید</a> حذف شد.', 'سبد خرید');
                else
                    toastr.warning('شما \'' + productname + '\'  را از <a href="/Cart">سبد خرید</a> حذف کرده اید.', 'سبد خرید');

                if (productID)
                    $('tr[data-productid="' + productID + '"]').remove();

                if (productVarientID)
                    $('tr[data-productvarientid="' + productVarientID + '"]').remove();

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

$('.products-box, .products-list-box, .similar-products-box').on('click', '.btn-add-cart', function (e) {
    e.preventDefault();

    var $this = $(this),
        $li = $this.closest('li'),
        productname = $li.find('h2').text(),
        productid = $li.data('productid'),
        productvarientid = $li.data('productvarientid'),
        data = {};

    if ($li.find('.price-isunavailable').length > 0) {
        toastr.warning('\'' + productname + '\' موجود نمی باشد.', 'سبد خرید');
        return;
    }

    if ($li.find('.price-comingsoon').length > 0) {
        toastr.info('\'' + productname + '\' به زودی ارائه می شود.', 'سبد خرید');
        return;
    }

    if ($li.find('.price-contactus').length > 0) {
        toastr.info('برای اطلاع از موجودی \'' + productname + '\' تماس بگیرید.', 'سبد خرید');
        return;
    }

    if (productvarientid == -1) {
        toastr.error('اطلاعات محصول ناقص است.', 'سبد خرید');
        return;
    }
    else if (productvarientid == '') {
        data.ProductID = productid;
    }
    else {
        data.ProductVarientID = productvarientid;
    }

    var $loading = $(staticTemplates.Loading);
    $li.append($loading);

    $.ajax({
        type: "POST",
        url: "/Cart/Add",
        data: data,
        success: function (response) {
            if (response.Success) {
                var result = response.Data;

                if (!result.CanAdd) {
                    toastr.error('\'' + productname + '\' موجود نمی باشد.', 'سبد خرید');
                    return;
                }

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

var $miniCart = $('.mini-cart'),
    $cartItems = $('.cart-items'),
    $cartCount = $('.basket-count-box span'),
    $cartEmpty = $('.cart-empty'),
    $cartBox = $('.cart-box'),
    $stepsBox = $('.steps-box'),
    $cartAlert = $('.cart-alert-empty'),
    $totalPrice = $('.mini-cart .total-price .price, .cart-box .total-price'),
    $totalProfit = $('.mini-cart .total-profit .price, .cart-box .total-profit'),
    $totalTopay = $('.mini-cart .total-topay .price, .cart-box .total-topay, .mini-cart .totalprice'),
    $sendtype = $('.sendtype'),
    $paymenttype = $('.paymenttype'),
    $cart = $('.cart');

function renderMiniCart(result) {
    cartItems = result.CartItems;

    $cartItems.empty();
    $cartCount.text(result.CartItems.length);

    if (result.CartItems.length == 0) {
        // Mini Cart
        $cartEmpty.removeClass('hide');
        $totalTopay.eq(1).html('<span class="cart-empty">تجربه خرید مطمئن...</span>');

        // Cart
        $cartBox.addClass('hide');
        $stepsBox.addClass('hide');
        $cartAlert.removeClass('hide');
    }
    else {
        // Mini Cart
        $cartEmpty.addClass('hide');

        // Cart
        $cartAlert.addClass('hide');
    }


    for (var i in result.CartItems) {
        var item = result.CartItems[i];

        var template = miniCartTemplate;

        for (var i in item) {
            var regex = new RegExp('{{' + i + '}}', 'g');
            template = template.replace(regex, item[i]);
        }

        template = $(template);
        if (item.DiscountPrice == 0) {
            template.find('.price-box').html(
                '<div class="regular-price">' + toPrice(item.Price, '{0} <span>{1}</span>') + '</div>'
            );
        }
        else {
            template.find('.price-box').html(
                '<div class="original-price">' + toPrice(item.Price, '{0} <span>{1}</span>') + '</div>' +
                '<div class="discount-price">' + toPrice(item.DiscountPrice, '{0} <span>{1}</span>') + '</div>');
        }

        $cartItems.append(template);
    }

    $totalPrice.html(toPrice(result.Total, '{0} <span class="price-unit">{1}</span>'));
    $totalTopay.html(toPrice(result.TotalDiscount, '{0} <span class="price-unit">{1}</span>'));
    $totalTopay.attr('data-price', result.TotalDiscount);

    var profit = (result.Total - result.TotalDiscount);
    if (profit == 0)
        $('.total-profit-row').addClass('hide');
    else {
        $('.total-profit-row').removeClass('hide');
        $totalProfit.html(toPrice(profit, '{0} <span class="price-unit">{1}</span>'));
    }

    copyCart();
}

function copyCart() {
    var $copyCart = $($('.step-content-1').html());

    $copyCart.find('.btn-quantity-down, .btn-quantity-up, .btn-cart-remove').remove();
    $copyCart.find('th:last').html('');

    var totaltopay = $copyCart.find('.total-topay').data('price');

    var delivary = -1;
    var sendtype = $sendtype.find(':checked').val();

    switch (sendtype) {
        case '0':
            delivary = 50000;
            break;
        case '1':
            delivary = 100000;
            break;
        case '2':
            delivary = 100000;
            break;
        default:
            break;
    }

    if (isFreeDelivery(totaltopay)) {
        delivary = 0;
    }

    $copyCart.find('.delivary-price-row').removeClass('hide');

    if (delivary > 0) {
        $copyCart.find('.delivary-price').html(toPrice(delivary, '{0} <span class="price-unit">{1}</span>'));
        $copyCart.find('.total-topay').html(toPrice(totaltopay + delivary, '{0} <span class="price-unit">{1}</span>'));
    }
    else {
        if (sendtype != '0' && isFreeDelivery(totaltopay)) {
            $copyCart.find('.delivary-price').html(staticTexts.MaxPriceFreeDelivery);
        }
        else {
            $copyCart.find('.delivary-price').html('رایگان');
        }
    }

    $('.step-content-4').html('').append($copyCart);
}

$('.mini-cart .dropdown-toggle').on('click', function (e) {
    var $this = $(this);

    if ($this.find('.cart-count').text() == 0) {
        e.preventDefault();
        e.stopPropagation();

        toastr.warning('سبد خرید شما خالی است.', 'سبد خرید');
    }
});

$('body').on('click', '.btn-quantity-down, .btn-quantity-up', function (e) {
    e.preventDefault();

    var $this = $(this),
        $tr = $this.closest('tr'),
        $quantity = $tr.find('.quantity'),
        quantity = $quantity.val(),
        productid = $tr.data('productid'),
        productvarientid = $tr.data('productvarientid'),
        prevQuantity = -1;

    quantity = parseInt(quantity);
    if (quantity > 0) {
        prevQuantity = quantity;

        if ($this.hasClass('btn-quantity-up'))
            quantity++;
        else
            if (quantity > 1) quantity--;
    }
    else {
        quantity = 1;
    }

    $quantity.val(quantity);

    var $loading = $(staticTemplates.Loading);
    $tr.append($loading);

    $.ajax({
        type: "POST",
        url: "/Cart/Changed",
        data: {
            productID: productid,
            productVarientID: productvarientid,
            Quantity: quantity
        },
        success: function (response) {
            if (response.Success) {
                var result = response.Data;

                var productByPID = _.find(result.CartItems, function (item) { return item.ProductID == productid });
                var productByVID = _.find(result.CartItems, function (item) { return item.ProductVarientID == productvarientid });

                var product;
                if (productByPID)
                    product = productByPID;

                if (productByVID)
                    product = productByVID;

                var $priceitem = $tr.find('.price-item');
                var $totalitem = $tr.find('.total-item');

                var priceTemplate = '{0} <span class="price-unit">{1}</span>';
                if (product.DiscountPrice > 0) {
                    $priceitem.find('.original-price').html(toPrice(product.DiscountPrice, priceTemplate));
                    $priceitem.find('.discount-price').html(toPrice(product.Price, priceTemplate));

                    $totalitem.find('.original-price').html(toPrice(product.Quantity * product.DiscountPrice, priceTemplate));
                    $totalitem.find('.discount-price').html(toPrice(product.Quantity * product.Price, priceTemplate));
                }
                else {
                    $priceitem.find('.regular-price').html(toPrice(product.Price, priceTemplate));

                    $totalitem.find('.regular-price').html(toPrice(product.Quantity * product.Price, priceTemplate));
                }

                renderMiniCart(result);
            }
            else {
                $quantity.val(prevQuantity);
                toastr.error(staticTexts.ResponseError, 'سبد خرید');
            }
        },
        error: function () {
            $quantity.val(prevQuantity);
            toastr.error(staticTexts.RequestError, 'سبد خرید');
        },
        complete: function () {
            $loading.remove();
        }
    });
});