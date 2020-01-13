var $key = $('.search-box #Keyword'),
    $resultBox = $('.search-box .result-box'),
    $notFound = $('.search-box .not-found');

// جستجوی کلیدواژه
var getProductsTimer = undefined;
$key.bind('keyup paste', function () {
    if (getProductsTimer)
        clearTimeout(getProductsTimer);

    $resultBox.empty();

    if ($key.val().length > 2) {
        $resultBox.slideDown();
        $resultBox.html('<span class="not-found">در حال جستجو...</span>');

        getProductsTimer = setTimeout(function () {
            $.ajax({
                type: 'POST',
                url: '/Admin/Products/SearchKeywords',
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

                                var li = '<li><a class="keyword-item" href="#" data-id="' + item.ID + '">' + item.Title + '</a></li>';

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

// انتخاب کلیدواژه
$resultBox.on('click', '.keyword-item', function (e) {
    e.preventDefault();

    var $this = $(this),
        id = $this.data('id');

    $resultBox.hide();
    $key.val($this.text())
        .data('id', id);
});
