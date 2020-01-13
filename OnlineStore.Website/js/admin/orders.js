$('.jconfigGrid').on('click', '.icon-archive', function (e) {
    e.preventDefault();

    var $this = $(this);
    var id = $this.data('id');

    $.ajax({
        url: '/Admin/Orders/Archive',
        type: 'Post',
        data: {
            ID: id
        },
        success: function (response) {
            if (response.Success) {
                $this.closest('tr').fadeOut();
            }
        },
        error: function () {
            toastr.error('بروز خطا', 'خطا');
        }
    });
});

$('.print-data').on('click', function () {
    var ids = [];
    $('.select-item:checked').each(function () {
        var $this = $(this),
            id = $this.closest('tr').data('id');

        ids.push(id);
    });

    if (ids.length == 0) {
        toastr.error('لطفا سفارشات مورد نظر خود را انتخاب نمایید.', 'بروز خطا');
        return;
    }

    window.open('/Admin/PostalInformation/Index/' + ids);
});