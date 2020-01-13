(function ($) {
    $(function () {

        var $type = $(".menu-item-type"),
            $editor = $(".editor-type"),
            $link = $(".link-type");

        showEditor();

        $type.on("change", function () {
            showEditor();
            return;
        })

        function showEditor() {

            switch ($type.val()) {
                case '0':
                    $editor.hide();
                    $link.hide();
                    break;
                case '1':
                    $editor.hide();
                    $link.show();
                    break;
                case '2':
                    $editor.show();
                    $link.show();
                default: break;

            }
        };
    });

})(jQuery);
