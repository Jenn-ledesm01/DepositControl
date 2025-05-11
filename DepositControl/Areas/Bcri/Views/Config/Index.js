toCamelCase = function (str) {
     var camel = str
        .replace(/\s(.)/g, function ($1) { return $1.toUpperCase(); })
        .replace(/\s/g, '')
        .replace(/^(.)/, function ($1) { return $1.toLowerCase(); });
     return camel.charAt(0).toUpperCase() + camel.slice(1);
}

String.prototype.toCamelCase = function () {
    var camel = this.replace(/^([A-Z])|[\s-_]+(\w)/g, function (match, p1, p2, offset) {
        if (p2) return p2.toUpperCase();
        return p1.toLowerCase();
    });
    return (camel.charAt(0).toUpperCase() + camel.slice(1)).replace(/[^A-Za-z0-9]/,'');
};


$(function () {

    var Common = $("#configIndexjsData").data();

    if (!$('#Code').attr('disabled')) { //solo si no esta desabilitado
        $('#Name').on('keyup', function() {
            $('#Code').val($(this).val().toCamelCase());
        });
    }
    $("#executeOnTimePicker").datetimepicker({
        format: "HH:mm",
        useCurrent: false
    });

    function apliAutoCheck() {
        var disabled = !$("#AutoExecute").is(":checked");

        $(".formAuto").find("select, input").attr("disabled", disabled);
    }
    $("#AutoExecute").on("click", function () {
        apliAutoCheck();
    });
    apliAutoCheck();

    var $form = $("#configForm");
    $form.find("#configSave").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: Common.saveurl,
            method: "POST",
            data: $form.serialize(),
            success: function (resp) {
                if (resp.ok)
                    $("#configSaveAlert").showAlert(res.alertSuccessCofig);
            },
            error: function (a) {
            }
        });
    });

});