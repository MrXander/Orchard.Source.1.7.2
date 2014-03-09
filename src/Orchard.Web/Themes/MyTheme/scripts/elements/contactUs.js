contactUs = {
    initialize: function (url) {
        //send form
        $(".sendFeedback").click(function () {
            var form = $("form");
            form.validate(function (isValid) {
                if (isValid) {
                    basic.hideToggle("#form-ajax-loader");
                    basic.visibilityToggle("#footer .container-relative form");
                    var d = form.serialize();
                    $.post(
                            url,
                            d,
                            function (data, textStatus, jqXHR) {
                                contactUs.showMessage(data);
                                basic.hideToggle("#form-ajax-loader");
                                setTimeout(contactUs.showForm, 3000);
                            },
                            "json")
                        .fail(function () {
                            contactUs.showMessage("Не удалось отправить. Попробуйте еще раз");
                        });                    
                }
            });
            return false;
        });
    },

    showMessage: function (text) {
        $("#form-message").text(text);
        basic.hideToggle("#form-message");
    },

    showForm: function () {
        var container = $("#footer .container-relative");
        container.children().not("form").hide();
        basic.visibilityToggle("#footer .container-relative form");
    }
};

