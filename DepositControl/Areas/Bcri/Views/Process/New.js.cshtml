<script language="javascript" type="text/javascript">    
    $(document).ready(function () {
        $.validator.addMethod("greaterThan", function (value, element, params) {
            if (!/Invalid|NaN/.test($(element).datepicker("getDate"))) {
                return $(element).datepicker("getDate") >= $(params).datepicker("getDate");
            }
            return isNaN(value) && isNaN($(params).val())
                || (Number(value) >= Number($(params).val()));
        }, 'Must be greater than {0}.');
        $.validator.unobtrusive.adapters.addBool('greaterThan');
        var single = true;
        $('#NewRange').on('click', function () {
            single = false;
            showForm();
        });
        $('#NewSingle').on('click', function () {
            single = true;
            showForm();
        });
        function showForm() {
            var send = false;
            //$form.find('input').val("");
            //$form.find('input[type=checkbox]').prop('checked', false);;
            //$form.clearValidation();
            //InitForm($form);
            var $form;
            BootstrapDialog.show({
                title: res.newProcess,
                message: function (dialog) {
                    var $message = $('<div></div>');
                    //InitForm(f);
                    $message.load('@Url.Action("New")', function () {

                        $form = $('form', this);
                        $form.validate({});
                        InitForm(this);
                        if (single) {
                            $('#PeriodSingle', $form).show();
                            $('#PeriodMulti', $form).hide();
                        } else {
                            $('#PeriodSingle', $form).hide();
                            $('#PeriodMulti', $form).show();
                        }


                        $('#PeriodTo', $form).rules('add', {
                            greaterThan: '#PeriodFrom'
                        });

                    });
                    return $message;
                },
                buttons: [{
                    label: res.create,
                    action: function (dialogRef) {
                        if (!$form.valid() || send) return;
                        send = true;
                       
                            $.ajax({
                                url: '@Url.Action("AddMulti")',
                                method: "POST",
                                data: $form.serialize(),
                                success: function (resp) {
                                    $("#jqGridProcess")
                                        .setGridParam({ datatype: "json" })
                                        .trigger("reloadGrid");
                                    dialogRef.close();
                                },
                                error: function (a) {
                                    send = false;
                                    $("#msgResult").showAlert("<strong>Error!</strong> " + a.responseText, 'error', 1000 * 60 * 60 * 24);

                                },
                                beforeSend: function () {
                                    Bcri.Core.LoadingScreen.show();
                                },
                                complete: function () {
                                    Bcri.Core.LoadingScreen.hide();
                                }
                            });
                        }
                    },
                {
                    label: res.close,
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }]
            });
        }



    });
</script>
