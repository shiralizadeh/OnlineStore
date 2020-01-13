function config(options) {
    $('.btn-jdelete').on('click', function (e) {
        e.preventDefault();

        if (!confirm('آیا مایل به حذف سطر مورد نظر هستید؟'))
            return;

        var $this = $(this),
            id = $this.closest('tr').data('id');

        $.ajax({
            url: options.DeleteUrl,
            method: 'POST',
            data: { ID: id },
            success: function (result) {
                if (result.Success)
                    $this.closest('tr').css({ 'text-decoration': 'line-through' }).animate({ opacity: '0.6' });
            },
            error: function () {
                $this.fadeIn().prev().fadeIn();
            },
            complete: function () {
            }
        });
    });
}