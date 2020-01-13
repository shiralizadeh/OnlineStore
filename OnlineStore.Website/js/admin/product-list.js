var $btn = $('#btnReference'),
    $user = $('#Username'),
    $loading = $('.loading'),
    $successPanel = $('.messages-body .alert-success'),
    $errorPanel = $('.messages-body .alert-danger'),
    $jsonGroups = $("#hfJSONGroups"),
    $producers = $("#Producer"),
    $producersList = $('.producers-list');

$btn.on('click', function () {
    if ($user.val() == -1) {
        alert('لطفا نویسنده مورد نظر را انتخاب نمایید.');
        return;
    }

    var ids = getSelectedProducts();

    if (!ids.length) {
        alert('لطفا محصولات مورد نظر را انتخاب نمایید.');
        return;
    }

    $loading.fadeIn();
    $.ajax({
        type: 'Post',
        url: '/Admin/Products/ReferenceProduct',
        data: {
            ProductIDs: ids,
            UserID: $user.val()
        },
        success: function (result) {
            if (result.Success) {
                $successPanel.fadeIn().removeClass('hide').children('span').text('ارجاع به ' + $("#Username option:selected").text() + ' با موفقیت انجام شد.');
                $user.val('-1');
                $('.select-item:checked').prop('checked', false);
            }
        },
        error: function (result) {
            errorList = $errorPanel.removeClass("hide").fadeIn().children('ul');
            errorList.empty();
            for (var i in result.Errors) {
                errorList.append("<li>" + result.Errors[i] + "</li>");
            }
        },
        complete: function () {
            $loading.fadeOut();
        }
    });
});

function getSelectedProducts() {
    var ids = [];

    $('.select-item:checked').each(function () {
        var $this = $(this),
            id = $this.closest('tr').data('id');

        ids.push(id);
    });

    return ids;
}

function treeChecked() {
    var $loading = $(staticTemplates.Loading);
    $producersList.append($loading);

    $producers.empty();
    $.ajax({
        type: 'POST',
        url: '/Admin/Products/FilterProducers',
        data: {
            Groups: eval($jsonGroups.val())
        },
        success: function (response) {
            if (response.Success) {
                if (response.Data.length > 0) {
                    $producers.append("<option value='-1'>تولید کننده</option>")

                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data[i];
                        $producers.append("<option value='" + item.ID + "'>" + item.Title + "</option>")
                    }
                }
            }
            else {
                alert('خطا');
            }
        },
        error: function () {
            alert('خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });

}