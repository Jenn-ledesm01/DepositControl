$(document).ready(function() {
    var $form = $('#spProcessForm');

    $("#spRefresh").on('click', function(e) {
        e.preventDefault();
        var formData = new FormData($form[0]);
        $.ajax({
            url: baseUrl + '/SpProcess/ReCreate',
            method: "POST",
            data: formData,
            contentType: false, 
            processData: false,
            success: function (data) {
                $('#Content').val(data);
            },
            error: function (error) {
                $('#spAlert').showAlert("Unspected Error: " + error.responseText, 'error');
            }
        });  
    });

    $("#spSave").on('click', function (e) {
        e.preventDefault();
        var formData = new FormData($form[0]);
        $.ajax({
            url: baseUrl + '/SpProcess/Save',
            method: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                $('#spAlert').showAlert("Save success");
            },
            error: function (error) {
                $('#spAlert').showAlert("Unspected Error: " + error.responseText, 'error');
            }
        });
    });

    $("#spApply").on('click', function (e) {
        e.preventDefault();
        var formData = new FormData($form[0]);
        $.ajax({
            url: baseUrl + '/SpProcess/Apply',
            method: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                $('#spAlert').showAlert("Apply success");
            },
            error: function (error) {
                $('#spAlert').showAlert("Unspected Error: " + error.responseText, 'error');
            }
        });
    });


});
