var Common = null;

String.prototype.toCamelCase = function () {
    var camel = this.replace(/^([A-Z])|[\s-_]+(\w)/g, function (match, p1, p2, offset) {
        if (p2) return p2.toUpperCase();
        return p1.toLowerCase();
    });
    return (camel.charAt(0).toUpperCase() + camel.slice(1)).replace(/[^A-Za-z0-9]/, '');
};


$(document).ready(function () {
    Common = $("#ProcessJsData").data();
    //Events
    $("#selectProcess").change(function () {
        var value = $(this).val();
        var sLink = Common.loaddataurl + "/" + value;
        if (value !== "0") {
            window.location.href = sLink;
        }
    });


    $('#newProcessConfig').on('click', function (event) {
        event.preventDefault();
        var eform  = $('#formNew').clone();
        BootstrapDialog.show({
            title: res.createProcessConfig,
            message: function() {
                
                eform.show();
                InitForm(eform);
                $('#Name', eform).on('keyup', function () {
                    $('#Code', eform).val($(this).val().toCamelCase());
                });

                return eform;
            } ,
            buttons: [{
                label: res.save,
                action: function (dialogItself) {
                    var form = $('form', eform);
                    if (!form.valid()) return;
                    $.ajax({
                        url: baseUrl + '/Config/CreateConfig',
                        data: form.serialize(),
                        type: "GET",
                        success: function(processCode) {
                            window.location = baseUrl + '/Config/' + processCode;
                            dialogItself.close();
                        },
                        error: function(resp) {
                            $('#configSaveAlert', eform).showAlert(resp.responseText, res.error);
                        }

                    });

                    
                }
            }, {
                label: res.close,
                action: function (dialogItself) {
                    dialogItself.close();
                }
            }]
        });
        
    });
});

