//---------------- On Ready ------------------------

$(function () {
    jQuery.extend(jQuery.validator.messages, {
        required: "Este campo &eacute; requerido.",
        remote: "Por favor, corrija este campo.",
        email: "Por favor, forne&ccedil;a um endere&ccedil;o eletr&ocirc;nico v&aacute;lido.",
        url: "Por favor, forne&ccedil;a uma URL v&aacute;lida.",
        date: "Por favor, forne&ccedil;a uma data v&aacute;lida.",
        dateISO: "Por favor, forne&ccedil;a uma data v&aacute;lida (ISO).",
        number: "Por favor, forne&ccedil;a um n&uacute;mero v&aacute;lido.",
        digits: "Por favor, forne&ccedil;a somente d&iacute;gitos.",
        creditcard: "Por favor, forne&ccedil;a um cart&atilde;o de cr&eacute;dito v&aacute;lido.",
        equalTo: "Por favor, forne&ccedil;a o mesmo valor novamente.",
        accept: "Por favor, forne&ccedil;a um valor com uma extens&atilde;o v&aacute;lida.",
        maxlength: jQuery.validator.format("Por favor, forne&ccedil;a n&atilde;o mais que {0} caracteres."),
        minlength: jQuery.validator.format("Por favor, forne&ccedil;a ao menos {0} caracteres."),
        rangelength: jQuery.validator.format("Por favor, forne&ccedil;a um valor entre {0} e {1} caracteres de comprimento."),
        range: jQuery.validator.format("Por favor, forne&ccedil;a um valor entre {0} e {1}."),
        max: jQuery.validator.format("Por favor, forne&ccedil;a um valor menor ou igual a {0}."),
        min: jQuery.validator.format("Por favor, forne&ccedil;a um valor maior ou igual a {0}.")
    });
    // Sort the date of the member
    SortWhosOnline();

    // Makes specific tables responsive
    ResponsiveTable();

    // Mobile Nav
    MobileNav();

    // Subscription Methods
    emailsubscription();
    emailunsubscription();

    // Setup the upload field styles
    SetUpUploadStyle();
});


var SetUpUploadStyle = function () {
    $(document).on("change", ".btn-file :file", function () {
        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger("fileselect", [numFiles, label]);
    });

    $(".btn-file :file").on("fileselect", function (event, numFiles, label) {
        var input = $(this).parents(".input-group").find(":text"),
            log = numFiles > 1 ? numFiles + " files selected" : label;

        if (input.length) {
            input.val(log);
        } else {
            if (log) alert(log);
        }
    });
};


var MobileNav = function() {
    var mobilenavbutton = $(".showmobilenavbar");
    if (mobilenavbutton.length > 0) {
        mobilenavbutton.click(function (e) {
            e.preventDefault();
            $(".mobilenavbar-inner ul.nav").slideToggle();
        });
    }
};


/*------------------ General Ajax Form Class ----------------------*/
var submitAjaxForm = function () {
    var submitFormElement = $(".ajaxform form");
    submitFormData(submitFormElement);
};

/*------------------ GENERAL SUBMIT FORMS METHOD -------------------*/

var submitFormData = function (formElement) {
    formElement.submit(function (e) {
        e.preventDefault();
        $(this).validate();
        if ($(this).valid()) {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                dataType: "json",
                cache: false,
                success: function (result) {
                    if (result.Success) {
                        closeSlideOutPanel();
                        ShowUserMessage(result.ReturnMessage);
                    } else {
                        ShowUserMessage(result.ReturnMessage);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        }
        return false;
    });
};

/*------------ SUBSCRIPTION METHODS --------------------*/

var emailsubscription = function () {
    var esub = $(".emailsubscription");
    if (esub.length > 0) {
        esub.click(function (e) {
            e.preventDefault();
            var entityId = $(this).data("id");
            var subscriptionType = $(this).data("type");

            $(this).hide();

            var subscribeEmailViewModel = new Object();
            subscribeEmailViewModel.Id = entityId;
            subscribeEmailViewModel.SubscriptionType = subscriptionType;

            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(subscribeEmailViewModel);

            $.ajax({
                url: app_base + "Email/Subscribe",
                type: "POST",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $(".emailunsubscription").fadeIn();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        }); 
    }
};

var emailunsubscription = function () {
    var eunsub = $(".emailunsubscription");
    if (eunsub.length > 0) {
        eunsub.click(function (e) {
            e.preventDefault();
            var thisLink = $(this);
            var entityId = $(this).data("id");
            var subscriptionType = $(this).data("type");          

            thisLink.hide();

            var unSubscribeEmailViewModel = new Object();
            unSubscribeEmailViewModel.Id = entityId;
            unSubscribeEmailViewModel.SubscriptionType = subscriptionType;

            // Ajax call to post the view model to the controller
            var strung = JSON.stringify(unSubscribeEmailViewModel);

            $.ajax({
                url: app_base + "Email/UnSubscribe",
                type: "POST",
                cache: false,
                data: strung,
                contentType: "application/json; charset=utf-8",
                success: function (data) {

                    // We might be on the following page, so hide the items we need
                    var categoryRow = thisLink.closest(".categoryrow");
                    if (categoryRow) {
                        categoryRow.fadeOut("fast");
                    }
                    var topicrow = thisLink.closest(".topicrow");
                    if (topicrow) {
                        topicrow.fadeOut("fast");
                    }

                    var emailsubscription = $(".emailsubscription");
                    if (emailsubscription) {
                        emailsubscription.fadeIn();
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUserMessage("Error: " + xhr.status + " " + thrownError);
                }
            });
        });
    }
};

// Responsive Table

var ResponsiveTable = function() {
    var adaptiveTable = $(".table-adaptive");
    if (adaptiveTable.length > 0) {
        var headertext = [],
        headers = document.querySelectorAll(".table-adaptive th"),
        tablerows = document.querySelectorAll(".table-adaptive th"),
        tablebody = document.querySelector(".table-adaptive tbody");

        for (var i = 0; i < headers.length; i++) {
            var current = headers[i];
            headertext.push(current.textContent.replace(/\r?\n|\r/, ""));
        }
        if (tablebody.rows != null) {
            for (var i = 0, row; row = tablebody.rows[i]; i++) {
                for (var j = 0, col; col = row.cells[j]; j++) {
                    col.setAttribute("data-th", headertext[j]);
                }
            }
        }
    }
}

// Whos online

var SortWhosOnline = function () {
    $.getJSON(app_base + "Members/LastActiveCheck");
}


