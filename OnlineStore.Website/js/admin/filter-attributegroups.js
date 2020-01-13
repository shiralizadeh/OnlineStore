var $jsonGroups = $("#hfJSONGroups"),
    $attrGroups = $("#AttrGroups");

function treeChecked() {
    $attrGroups.empty();
    $.ajax({
        type: 'POST',
        url: '/Admin/AttrGroups/FilterByGroups',
        data: {
            Groups: eval($jsonGroups.val())
        },
        success: function (response) {
            if (response.Success) {
                $attrGroups.append("<option value='-1'>گروه ویژگی</option>")
                if (response.Data.length > 0) {
                    for (var i = 0; i < response.Data.length; i++) {
                        var item = response.Data[i];
                        $attrGroups.append("<option value='" + item.ID + "'>" + item.Title + "</option>")
                    }
                }
            }
            else {
                alert('خطا');
            }
        },
        error: function () {
            alert('خطا');
        },
    });

}

