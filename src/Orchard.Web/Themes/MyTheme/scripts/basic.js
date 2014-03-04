basic = {
    hideToggle : function(selector) {
        $(selector).toggleClass("hidden");
    },

    visibilityToggle: function (selector) {
        var elem = $(selector);
        if (elem.css('visibility') == 'hidden')
            elem.css('visibility', 'visible');
        else
            elem.css('visibility', 'hidden');
    }
};