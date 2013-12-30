(function ($) {
    $.fn.contextmenu = function (settings) {
        settings = jQuery.extend({
            menuPanelSelector: undefined
        }, settings);

        function getPanel($element) {
            var $panel;
            if (settings.menuPanelSelector) {
                $panel = $element.siblings(settings.menuPanelSelector);
            }
            else {
                $panel = $element.next();
            }

            return $panel;
        }

        function open($element) {
            $panel = getPanel($element);
            var left = $element.offset().left - $panel.width() + $element.width();
            var top = $element.offset().top + $element.height();
            $panel.css({ 'left': left, 'top': top }).stop(true, true).delay(200).slideDown('fast');

            $panel.hover(function () {
                $panel.stop(true, true);
            }, function () {
                $panel.stop(true, true).delay(200).slideUp('fast');
            });
        }

        $(this).hover(function (event) {
            open($(this));
        }, function (event) {
            $panel = getPanel($(this));
            $panel.stop(true, true).delay(200).slideUp('fast');
        });

        $(this).click(function (event) {
            event.preventDefault();
        });
    }
})(jQuery)