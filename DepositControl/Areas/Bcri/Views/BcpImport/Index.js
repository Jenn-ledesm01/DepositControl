$(document).ready(function() {
    var $form = $("#bcpForm");
    function changeBcpType() {
        if ($('input[name=bcptype]:checked').val() == "Db") {
            $('#formDb').show();
            $('#formFile').hide();
        } else {
            $('#formFile').show();
            $('#formDb').hide();
        }
    }
    changeBcpType();
    $('input[name=bcptype]').on('change', changeBcpType);
    
    function changeFileType() {
        if ($('input[name=filetype]:checked').val() == "Fixed") {
            $('#Delimited').hide();
        } else {
            $('#Delimited').show();
        }
    }
    changeFileType();
    $('input[name=filetype]').on('change', changeFileType);


    
    $form.find("#bcpSave").on("click", function (e) {
        e.preventDefault();
        $.ajax({
            url: baseUrl + "/BcpImport/Save",
            method: "POST",
            data: $form.serialize(),
            success: function (resp) {
                if (resp.ok)
                    $("#bcpSaveAlert").showAlert("<strong>Success!</strong> your settings are saved correctly.");
                else
                    $("#bcpSaveAlert").showAlert("<strong>Error!</strong> " + resp.error, 'error');
            },
            error: function (a) {
                $("#bcpSaveAlert").showAlert("<strong>Error!</strong> " + a, 'error');
            }
        });
    });

});
