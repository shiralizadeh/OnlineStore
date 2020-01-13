var $key = $('.search-box #Search'),
    $resultBox = $('.search-box .result-box'),
    $url = $('.search-box #Url'),
    $notFound = $('.search-box .not-found');

// جستجوی مقاله
var getArticlesTimer = undefined;
$key.bind('keyup paste', function () {
    if (getArticlesTimer)
        clearTimeout(getArticlesTimer);

    $resultBox.empty();

    if ($key.val().length > 2) {
        $resultBox.slideDown();
        $resultBox.html('<span class="not-found">در حال جستجو...</span>');

        getArticlesTimer = setTimeout(function () {
            $.ajax({
                type: 'POST',
                url: $url.val(),
                data: {
                    Key: $key.val(),
                },
                success: function (response) {
                    if (response.Success) {
                        if (response.Data.length == 0) {
                            $resultBox.html('<span class="not-found">موردی یافت نشد.</span>');
                        }
                        else {
                            $resultBox.empty();
                            var $ul = $('<ul>');

                            for (var i = 0; i < response.Data.length; i++) {
                                var item = response.Data[i];

                                var li = '<li><a href="#" data-id="' + item.ID + '" title="' + item.Title + '"><img src="' + item.Image + '"><span>' + item.Title + '</span></a></li>';

                                $ul.append(li);
                            }

                            $resultBox.append($ul);
                        }
                    }
                    else {
                        alert('خطا');
                    }
                },
                error: function () {
                    //alert('خطا');
                },
            });
        }, 800);
    }
    else {
        $resultBox.slideUp();
    }

});

// انتخاب مطلب مرتبط
$resultBox.on('click', 'a', function (e) {
    e.preventDefault();

    var $this = $(this);

    $resultBox.hide();
    $key.val($this.attr('title'))
        .data('id', $this.data('id'))
        .attr('disabled', 'disabled');
});
