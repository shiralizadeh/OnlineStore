var $giftType = $('.gift-card-type'),
    $percent = $('.percent'),
    $price = $('.price'),
    $count = $('.count'),
    $minPrice = $('.min-price'),
    $iPrice = $price.find('input'),
    $iPercent = $percent.find('input'),
    $iCount = $count.find('input'),
    $iMinPrice = $minPrice.find('input');

$price.hide();
$count.hide();
$minPrice.hide();
$percent.show();

setGiftCardType();

$giftType.on('change', function () {
    $iPrice.val('');
    $iPercent.val('');
    $iCount.val('');
    $iMinPrice.val('');

    setGiftCardType();
});

$('.persiandate').MdPersianDateTimePicker({
    EnableTimePicker: true
}).on('keydown', function (e) {
    if (e.keyCode != 9)
        e.preventDefault();
});

function setGiftCardType() {
    switch ($giftType.val()) {
        case '0':
            $price.hide();
            $count.hide();
            $minPrice.hide();
            $percent.show();
            break;
        case '1':
            $price.show();
            $count.hide();
            $minPrice.hide();
            $percent.hide();
            break;
        case '2':
            $price.hide();
            $count.show();
            $minPrice.hide();
            $percent.show();
            break;
        case '3':
            $price.show();
            $count.show();
            $minPrice.hide();
            $percent.hide();
            break;
        case '4':
            $price.hide();
            $count.hide();
            $minPrice.show();
            $percent.show();
            break;
        case '5':
            $price.show();
            $count.hide();
            $minPrice.show();
            $percent.hide();
            break;
        default:
    }
}

