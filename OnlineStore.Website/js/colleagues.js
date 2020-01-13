/// <reference path="jquery-1.11.3.min.js" />
$(function () {
    var $sendRequest = $('#ColleagueForm'),
        $firstName = $("#ColleagueForm #FirstName"),
        $lastName = $("#ColleagueForm #LastName"),
        $email = $("#ColleagueForm #Email"),
        $phone = $("#ColleagueForm #Phone"),
        $mobile = $("#ColleagueForm #Mobile"),
        $companyName = $("#ColleagueForm #CompanyName"),
        $companyAddress = $("#ColleagueForm #CompanyAddress"),
        $cooperationDescription = $("#ColleagueForm #CooperationDescription"),
        $text = $("#ColleagueForm #Text"),
        $btn = $("#ColleagueForm .btn-submit"),
        $messageBox = $('.message-box');

    $btn.on("click", function () {
        if (!$sendRequest.valid()) {
            return;
        }

        var $loading = $(staticTemplates.Loading);
        $sendRequest.append($loading);

        var data = {
            FirstName: $firstName.val(),
            LastName: $lastName.val(),
            CompanyName: $companyName.val(),
            Email: $email.val(),
            Phone: $phone.val(),
            Mobile: $mobile.val(),
            CompanyAddress: $companyAddress.val(),
            CooperationDescription: $cooperationDescription.val(),
            Text: $text.val()
        };

        $.ajax({
            type: "POST",
            url: "/Colleagues/SendRequest",
            data: data,
            success: function (response) {
                if (response.Success) {
                    $sendRequest.hide();
                    $messageBox.removeClass('hide');
                }
                else {
                    toastr.error(staticTexts.ResponseError, 'همکاری با ما');
                }
            },
            error: function () {
                toastr.error(staticTexts.RequestError, 'همکاری با ما');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });
});