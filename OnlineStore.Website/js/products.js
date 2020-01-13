$('body').on('click', 'a.link-wishlist,a.btn-wishlist', function (e) {
    e.preventDefault();

    var $this = $(this),
        $productitem = $this.closest('.product-item,.product-view'),
        productname = $productitem.find('.product-title').text(),
        id = $productitem.data('productid');

    var $loading = $(staticTemplates.Loading);
    $productitem.append($loading);

    $.ajax({
        type: "POST",
        url: "/UserWishes/Add",
        data: {
            ProductID: id
        },
        success: function (response) {
            if (response.Success) {
                var result = response.Data;

                if (result.Login) {
                    if (result.Exists)
                        toastr.warning('شما \'' + productname + '\'  را به <a href="/My-Account/My-Wishes">آرزوهای من</a> اضافه کرده اید.', 'سبد خرید');
                    else
                        toastr.success('\'' + productname + '\' به <a href="/My-Account/My-Wishes">آرزوهای من</a> اضافه شد.', 'آرزوی من');
                }
                else {
                    toastr.warning('لطفا <a href="/ثبت-نام">ثبت نام</a> کنید یا <a href="/Login">وارد سایت</a> شوید.', 'آرزوی من');
                }
            }
            else {
                toastr.error(staticTexts.ResponseError, 'آرزوی من');
            }
        },
        error: function () {
            toastr.error(staticTexts.RequestError, 'آرزوی من');
        },
        complete: function () {
            $loading.remove();
        }
    });
});
