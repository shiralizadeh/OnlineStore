$(function () {
    var $id = $("#ProductID"),
        $userName = $("#UserName"),
        $question = $("#Question"),
        $btn = $(".btn-send"),
        $success = $(".messages-body .alert-success"),
        $danger = $(".messages-body .alert-danger"),
        $loading = $(".loading");

    $loading.fadeOut();

    $btn.on("click", function () {
        $loading.fadeIn();

        $.ajax({
            type: "POST",
            url: "/User/ProductQuestions/SendQuestion",
            data: {
                ID: $id.val(),
                UserName: $userName.val(),
                Question: $question.val(),
            },
            success: function (response) {
                if (response.Success) {
                    $success.removeClass("hide").fadeIn().children(".message-pane").text("درخواست شما با موفقیت ارسال شد.");
                    clearForm();
                }
                else {
                    $danger.removeClass("hide").fadeIn();
                    for (var i in response.Errors) {
                        alert(response.Errors);
                        $success.children("ul").append("<li>" + response.error + "</li>");
                    }
                }
            },
            error: function (response) {
                $danger.removeClass("hide").fadeIn().append("<li>عملیات با خطا مواجه شد.</li>");
            },
            complete: function () {
                $loading.fadeOut();
            }
        });
    });

    function clearForm() {
        $userName.val("");
        $question.val("");

    }
});