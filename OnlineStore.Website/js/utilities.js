function commaSeparate(input) {
    while (/(\d+)(\d{3})/.test(input.toString())) {
        input = input.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }

    return input;
}

function formatPrice(input, format) {
    if (input != 'null') {
        if (!isRial)
            input = (input / 10);

        if (format)
            return format.replace('{0}', commaSeparate(input));
        else
            return commaSeparate(input) + (isRial ? ' ریال' : ' تومان');
    }
    else {
        return 'نامعلوم';
    }
}

function toPrintFactor(id) {
    return '<a class="fa fa-print icon-print" title="پرینت" target="_blank" href="/My-Account/My-Orders/Factor/' + id + '"></a>';
}

function toOrderItemLink(id) {
    return '<a href="/Admin/OrderItems?id=' + id + '" target="_blank" class="icon-shopping-cart" ></a>';
}

function toProductCommentLink(id) {
    return '<a href="/Admin/ProductComments/index?ProductID=' + id + '" target="_blank" class="icon-comment-alt"></a>';
}

function toScoreCommentLink(id) {
    return '<a href="/Admin/ScoreComments/index?ProductID=' + id + '" target="_blank" class="icon-star"></a>';
}

function toRelProductLink(id) {
    return '<a href="/Admin/RelatedProducts/index?ProductID=' + id + '" target="_blank" class="icon-list"></a>';
}

function toProductGifts(id) {
    return '<a href="/Admin/ProductGifts/index?ProductID=' + id + '" target="_blank" class="icon-gift"></a>';
}

function toProductAccessoriesLink(id) {
    return '<a href="/Admin/ProductAccessories/index?ProductID=' + id + '" target="_blank" class="icon-wrench"></a>';
}

function toRelArticleLink(id) {
    return '<a href="/Admin/RelatedArticles/index?ArticleID=' + id + '" target="_blank" class="icon-list"></a>';
}

function toHomeBoxProductsLink(url) {
    return '<a href="' + url + '" target="_blank" class="icon-list"></a>';
}

function toProductLink(id) {
    return '<a href="/Products/grouptitle/perfix-' + id + '" target="_blank">لینک</a>';
}

function toProductQuestionLink(id) {
    return '<a href="/Admin/ProductQuestions/index?ProductID=' + id + '" target="_blank" class="icon-question-sign"></a>';
}

function toProductSuggestionLink(id) {
    return '<a href="/Admin/ProductSuggestions/index?ProductID=' + id + '" target="_blank" class="icon-share-alt"></a>';
}

function toArticleCommentLink(id) {
    return '<a href="/Admin/ArticleComments/index?ArticleID=' + id + '" target="_blank" class="icon-comment-alt"></a>';
}

function toArticleLink(id) {
    return '<a href="/Blog/A/B-' + id + '" target="_blank">لینک</a>';
}

function toKeywordProducts(id) {
    return '<a href="/Admin/ProductKeywords/index?KeywordID=' + id + '" target="_blank" class="icon-list"></a>';
}

function toUser(id) {
    return '<a href="/Admin/OSUsers/Edit/' + id + '" target="_blank" class="icon-user"></a>';
}

function fromPercent(input) {
    return '<span dir="ltr">' + input + '%</span>';
}

function toPersianDate(input) {
    if (input != 'null') {
        return '<span dir="ltr">' + moment(input).format('jYYYY/jMM/jDD HH:mm') + '</span>';
    }
}

function fromNow(input) {
    return '<span title="' + moment(input).format('jYYYY/jMM/jDD HH:mm') + '">' + moment(input).fromNow() + '</span>';
}

function toBooleanStatus(input) {
    if (input == 'true')
        return '<span class="label label-success">فعال</span>';
    else
        return '<span class="label label-danger">غیر فعال</span>';
}

function toBooleanYesNo(input) {
    if (input == 'true')
        return '<span class="label label-success">بلی</span>';
    else
        return '<span class="label label-danger">خیر</span>';
}

function toLink(input) {
    if (input == 'null') {
        return '';
    }
    else
        return '<a href="' + input + '" target="_blank">لینک</a>';
}

function toPackageLink(id) {
    return '<a href="/Packages/perfix-' + id + '" target="_blank">لینک</a>';
}

function fromSliderType(input) {
    switch (input) {
        case '0':
            return 'اسلایدر صفحه اصلی';
            break;
        case '1':
            return 'اسلایدر تخفیف ها';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromAttributeType(input) {
    switch (input) {
        case '0':
            return 'متن';
            break;
        case '1':
            return 'عدد';
            break;
        case '2':
            return 'تک انتخابی';
            break;
        case '3':
            return 'چند انتخابی';
            break;
        case '4':
            return 'بله / خیر';
            break;
        case '5':
            return 'متن چند خطه';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromProductDiscountStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromSpecialOrderStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-success">چک شده</span>';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromArticleStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
    }

}

function fromArticleCommentStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
    }
}

function fromScoreCommentStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
    }
}

function fromProductStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
    }
}

function fromQuestionStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">پاسخ داده نشده</span>';
            break;
        case '2':
            return '<span class="label label-success">پاسخ داده شده</span>';
            break;
    }
}

function fromEmailStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-success">دریافت شد</span>';
            break;
        case '1':
            return '<span class="label label-danger">ناموفق</span>';
            break;
    }

}

function fromPaymentMethodType(input) {
    switch (input) {
        case '0':
            return '<span class="label label-success">کارت شتاب</span>';
            break;
        case '1':
            return '<span class="label label-info">کارت به کارت</span>';
            break;
        case '2':
            return '<span class="label label-warning">پرداخت در محل</span>';
            break;
    }
}

function fromSendMethodType(input) {
    switch (input) {
        case '0':
            return 'پیک';
            break;
        case '1':
            return 'پست پیشتاز';
            break;
        case '2':
            return 'تیپاکس';
            break;
    }
}

function fromProductCommentStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">تایید شده</span>';
            break;
    }
}

function fromContactUsMessageStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-danger">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-warning">چک شده</span>';
            break;
        case '2':
            return '<span class="label label-success">پاسخ داده شده</span>';
            break;
    }
}

function fromCartStatus(input) {
    switch (input) {
        case '1':
            return '<span class="label label-warning">در حال خرید</span>';
            break;
        case '2':
            return '<span class="label label-success">پرداخت موفق</span>';
            break;
        case '3':
            return '<span class="label label-danger">پرداخت ناموفق</span>';
            break;
        case '4':
            return '<span class="label label-warning">در حال پرداخت</span>';
            break;
        case '5':
            return '<span class="label label-info">پرداخت در آینده</span>';
            break;
    }
}

function fromOrderSendStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-danger">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-warning">ارسال شد</span>';
            break;
        case '2':
            return '<span class="label label-success">تحویل داده شد</span>';
            break;
        case '3':
            return '<span class="label label-default">برگشت داده شد</span>';
            break;
        case '4':
            return '<span class="label label-info">بررسی شده</span>';
            break;

    }
}

function fromPriceType(input) {
    switch (input) {
        case '0':
            return 'خرید';
            break;
        case '1':
            return 'فروش';
            break;
    }
}

function fromMenuItemType(input) {
    switch (input) {
        case '0':
            return 'بدون لینک';
            break;
        case '1':
            return 'لینک';
            break;
        case '2':
            return 'صفحه داخلی';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromProductRequestStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-danger">رد شده</span>';
            break;
        case '2':
            return '<span class="label label-success">پاسخ داده شده</span>';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromEmailSendStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-warning">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-info">در حال ارسال</span>';
            break;
        case '2':
            return '<span class="label label-success">ارسال شده</span>';
            break;
        case '3':
            return '<span class="label label-danger">ناموفق</span>';
            break;
    }
}

function fromPriority(input) {
    switch (input) {
        case '0':
            return '<span class="label label-danger">کم</span>';
            break;
        case '1':
            return '<span class="label label-warning">متوسط</span>';
            break;
        case '2':
            return '<span class="label label-success">زیاد</span>';
            break;
    }
}

function fromWebsiteName(input) {
    switch (input) {
        case '0':
            return 'دیجی کالا';
            break;
        case '1':
            return 'بامیلو';
            break;
        default:
            return '<span class="label label-danger">نا معلوم</span>';
            break;
    }
}

function fromEmploymentStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-danger">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-warning">چک شده</span>';
            break;
    }
}

function fromColleagueStatus(input) {
    switch (input) {
        case '0':
            return '<span class="label label-danger">چک نشده</span>';
            break;
        case '1':
            return '<span class="label label-warning">چک شده</span>';
            break;
    }
}
