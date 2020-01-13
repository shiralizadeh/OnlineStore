/// <reference path="../jquery-1.11.3.min.js" />
var $taskDateForm = $('#TaskDateForm'),
    $taskDate = $('#TaskDate');

$('.my-tasks').on('click', '.icon-check', function () {
    var $this = $(this);
    var tr = $this.closest('tr');
    var rowID = tr.data('id');

    $.ajax({
        url: '/Dashboard/HideTask',
        type: 'Post',
        data: {
            ID: rowID
        },
        success: function (response) {
            if (response.Success) {
                tr.fadeOut();
            }
        },
        error: function () {
            toastr.error('عملیات با خطا مواجه شد', 'خطا');
        }
    });
});

$('.btn-settaskdate').magnificPopup({
    type: 'inline',
    midClick: true,
    removalDelay: 300,
    mainClass: 'mfp-fade',
});


$('.btn-settaskdate').on('click', function () {

    var $loading = $(staticTemplates.Loading);
    $taskDateForm.append($loading);

    var $id = $(this).closest('tr').data('id');

    $.ajax({
        url: '/Admin/Dashboard/GetTask',
        type: 'Post',
        data: {
            ID: $id
        },
        success: function (response) {
            if (response.Success) {
                var item = response.Data;
                $taskDate.val(item.PersianUserTaskDate);
                $taskDateForm.data('id', item.ID);
            }
        },
        complete: function () {
            $loading.remove();
        }
    });
});

$taskDateForm.on('click', '.btn-success', function () {

    if ($taskDate.val() == '') {
        toastr.error('لطفا تاریخ مورد نظر خود را وارد نمایید.', 'خطا');
    }
    var $loading = $(staticTemplates.Loading);
    $taskDateForm.append($loading);

    var $id = $taskDateForm.data('id');

    $.ajax({
        url: '/Admin/Dashboard/SetTask',
        type: 'Post',
        data: {
            ID: $id,
            UserTaskDate: $taskDate.val()
        },
        success: function (response) {
            if (response.Success) {
                toastr.success('ویرایش با موفقیت انجام شد.', 'تایید');
                $('.mfp-close').click();
            }
        },
        error: function () {
            toastr.error('بروز خطا.', 'خطا');
        },
        complete: function () {
            $loading.remove();
        }
    });
});