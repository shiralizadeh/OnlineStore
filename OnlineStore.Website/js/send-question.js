$(function () {
    var $sendQuestion = $('#SendQuestion'),
        $id = $('.product-details').data('productid'),
        $userName = $("#Questioner"),
        $question = $("#Question"),
        $btn = $("#SendQuestion .btn-submit"),
        productname = $('.product-title h1').text();

    $btn.on("click", function (e) {
        e.preventDefault();

        if (!$sendQuestion.valid()) {
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $sendQuestion.append($loading);

        $.ajax({
            type: "POST",
            url: "/Products/SendQuestion",
            data: {
                ID: $id,
                UserName: $userName.val(),
                Question: $question.val(),
            },
            success: function (response) {
                if (response.Success) {
                    toastr.success('پرسش شما برای محصول ' + productname + ' با موفقیت ارسال شد.', 'پرسش برای محصول ' + productname);
                    clearForm();
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'پرسش برای محصول ' + productname);
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'پرسش برای محصول ' + productname);
            },
            complete: function () {
                $loading.remove();
            }
        });
    });

    function clearForm() {
        $userName.val("");
        $question.val("");

    }
});