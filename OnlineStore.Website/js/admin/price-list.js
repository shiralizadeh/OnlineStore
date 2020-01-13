var $priceListProducts = $('.price-list-box .column'),
    $priceListProductsUl = $('.price-list-box ul'),
    arrSections = [],
    arrProducts = [];

$priceListProductsUl.sortable({
    stop: function (event, ui) {
        var $this = $(this);
        var array = $this.sortable('toArray', { attribute: 'data-id' });

        var $section = $this.closest('.portlet');
        sortProducts(array, $section);
    }
});

$priceListProducts.sortable({
    stop: function (event, ui) {
        var $this = $(this);
        var array = [];

        $priceListProducts.each(function () {
            array.push($(this).sortable("toArray", { attribute: 'data-id' }));
        });

        sortSections(array, $this);
    },
    connectWith: $priceListProducts,
    placeholder: "portlet-placeholder ui-corner-all"
});

$priceListProductsUl.on('change ifChanged', 'input', function () {
    var $this = $(this);

    var newValue = $this.val();
    var $li = $this.closest('li');
    var id = $li.data('id');
    var fieldName = 0;

    if ($this.hasClass('title')) {
        fieldName = 0;
    }
    else if ($this.hasClass('sub-title')) {
        fieldName = 1;
    }
    else if ($this.hasClass('price')) {
        fieldName = 2;
    }
    else if ($this.hasClass('checkbox')) {
        fieldName = 3;
        newValue = $this.prop('checked');
    }

    saveChanges(id, newValue, fieldName, $li);
});

function sortProducts(array, section) {

    var $loading = $(staticTemplates.Loading);
    section.append($loading);

    $.ajax({
        url: '/Admin/PriceList/SortProducts',
        type: 'Post',
        data: {
            ProductItems: array
        },
        success: function (response) {
            if (!response.Success) {
                toastr.error('بروز خطا در مرتب سازی محصولات', 'خطا');
            }
        },
        error: function () {
            toastr.error('بروز خطا در مرتب سازی محصولات', 'خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });
}

function sortSections(array, section) {

    var $loading = $(staticTemplates.Loading);
    section.append($loading);

    $.ajax({
        url: '/Admin/PriceList/SortSections',
        type: 'Post',
        data: {
            Col1: array[0],
            Col2: array[1],
            Col3: array[2],
            Col4: array[3],
            Col5: array[4],
        },
        success: function (response) {
            if (!response.Success) {
                toastr.error('بروز خطا در مرتب سازی گروه ها', 'خطا');
            }
        },
        error: function () {
            toastr.error('بروز خطا در مرتب سازی گروه ها', 'خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });
}

function saveChanges(id, newValue, fieldName, li) {

    var $loading = $(staticTemplates.Loading);
    li.append($loading);

    $.ajax({
        url: '/Admin/PriceList/SaveChanges',
        type: 'Post',
        data: {
            ID: id,
            NewValue: newValue,
            PriceListFieldName: fieldName
        },
        success: function (response) {
            if (!response.Success) {
                toastr.error('بروز خطا در ویرایش مقادیر', 'خطا');
            }
        },
        error: function () {
            toastr.error('بروز خطا در ویرایش مقادیر', 'خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });
}



