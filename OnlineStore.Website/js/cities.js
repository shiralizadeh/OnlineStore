$(function () {
    var $stateCityBox = $('.state-city'),
        $state = $('.state'),
        $city = $('.city');

    $state.on('change', function () {
        $this = $(this);

        var $loading = $(staticTemplates.Loading);
        $city.after($loading);

        $city.find('option').remove();

        $.ajax({
            type: 'POST',
            url: '/Cities/Get',
            data: {
                ID: $this.val(),
            },
            success: function (result) {
                if (result.Success) {
                    for (var i = 0; i < result.Data.length; i++) {
                        $city.append('<option value=' + result.Data[i].ID + '>' + result.Data[i].Title + '</option>')
                    }

                    if (initCityID && initCityID >= -1) {
                        $city.val(initCityID);
                        initCityID = -1;
                    }
                }
                else {
                    alert('Error');
                }
            },
            error: function (result) {
                alert('Error');
            },
            complete: function () {
                $loading.remove();
            }
        });
    });

    if (initStateID) {
        $state.val(initStateID).change();
    }
});