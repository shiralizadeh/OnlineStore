(function ($) {
    $(function () {

        var $type = $(".content-type"),
            $editor = $(".editor"),
            $simple = $(".simple");

        showEditor();

        $type.on("change", function () {
            showEditor();
            return;
        })

        function showEditor() {

            if ($type.val() == 1) {
                $editor.show();
                $simple.hide();
            }
            else {
                $editor.hide();
                $simple.show();
            }
        };
    });

})(jQuery);



