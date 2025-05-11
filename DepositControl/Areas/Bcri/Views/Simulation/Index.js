$(document).ready(function () {


    function actions(cellvalue, options, rowObject) {
        var actionsRow = $("#simulationActionTemplate").clone();
        actionsRow.children()
            .attr("data-Id", rowObject.Id)
            .css("display", "inline-table");

        actionsRow.find('a[data-action=Join]').hide();
        actionsRow.find('a[data-action=Left]').hide();
        //if(rowObject.Status)
        if (rowObject.IsReady) {
            if (rowObject.YouIn) {
                actionsRow.find('a[data-action=Left]').show();
            } else {
                actionsRow.find('a[data-action=Join]').show();
            }
        }
        return actionsRow.html().replace(/\n/g, "").replace(/\r/g, "").replace(/\t/g, ""); // el enter activa un stylo en la jqgrid que queda feo
    }

    $('#New').click(function () {
        var send = false;
        //$form.find('input').val("");
        //$form.find('input[type=checkbox]').prop('checked', false);;
        //$form.clearValidation();
        //InitForm($form);
        var $form;
        BootstrapDialog.show({
            title: res.newNew + " " +  res.simulation,
            message: function (dialog) {
                var $message = $('#simulationCreateForm').clone().show(); //new panel
                InitForm($message);
                $form = $message.find('form');
                return $message;
            },
            buttons: [{
                label: res.create,
                action: function (dialogRef) {
                    //confirmar eliminacion, avisar usuarios activos en la misma
                    if (!$form.valid() || send)
                        return;
                    send = true;
                    $.ajax({
                        url: baseUrl + '/Simulation/Create',
                        method: "POST",
                        data: $form.serialize(),
                        success: function (resp) {
                            
                        },
                        error: function (a) {
                            send = false;
                            $("#msgResult").showAlert("<strong>Error!</strong> " + a.responseText, 'error', 15000);
                        }
                    });
                    setTimeout(function() {
                            $("#gridSimulation")
                                .setGridParam({ datatype: "json" })
                                .trigger("reloadGrid");
                            dialogRef.close();
                        },
                        1500);
                }
            },
            {
                label: res.close,
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });
    });


    var dicData = {};


    $("#gridSimulation").jqGrid({
        url: baseUrl + '/Simulation/GetAll',
        editurl: '',
        datatype: "json",
        colModel: [
            {
                label: "Id",
                name: "Id",
                key: true,
                hidden: true
            },
            {
                label: "YouIn",
                name: "YouIn",
                hidden: true
            },

            {
                label: res.name,
                name: "Name",
                width: 300,
                sorttype: "text",
                search: true,
                formatter: function (cellvalue, options, rowObject) {
                    return rowObject.YouIn ?  cellvalue + "<spam style='color: green;font-size: 12px;'>(You are in)</spam>" : cellvalue;
                }
            },
            {
                label: res.status,
                name: "Status",
                sorttype: "text",
                search: true
            },
            {
                label: res.creationDate,
                name: "CreationDate",
                sorttype: "text",
                search: true,
            },
            {
                label: res.owner,
                name: "Owner",
                sorttype: "text",
                search: true
            },
            {
                label: res.users,
                name: "Users",
                sorttype: "text",
                search: true,
            },
            {
                label: res.action,
                name: "Action",
                fixed: true,
                width: 180,
                search: false,
                title: "",
                formatter: actions,
                sortable: false
            }

            /*  Id,
                Name,
                CreationDate,
                Owner,
                Users,
                YouIn */
        ],

        sortname: "CreationDate",
        sortorder: "desc",
        loadonce: true,
        viewrecords: false,
        height: 310,
        width: 680,
        autowidth: false,
        rowNum: 10,
        pager: "#gridSimulationPager",
        loadComplete: function() {
            dicData = {},
                data = $("#gridSimulation").jqGrid('getGridParam', 'data');
            $.each(data, function (i, e) {
                dicData[e.Id] = e;
            });

        },
        gridComplete: function () {
            $(".processActionGrid a[data-action]").unbind("click"); // activo las acciones
            $(".processActionGrid a[data-action]").on("click", function(e) {
                e.preventDefault();
                var simulationId = $(this).parent("div[data-Id]").attr("data-Id");
                var action = $(this).attr("data-action");

				if (action == "Delete" && !confirm(res.simulationConfirmDelete))
					return;
				else if (action === "Export") {
					window.location.href = baseUrl + "/Simulation/Export/" + simulationId;
					return;
				}
                $.ajax({
                    method: "post",
                    url: baseUrl + "/Simulation/" + action + "/" + simulationId,
                    success: function(data) {
                        window.location.href = window.location.href;
                    },
                    error: function() {
                    }
                });
                if (action == "Delete") {
                    setTimeout(function() {
                            window.location.href = window.location.href;
                            $("#gridSimulation")
                                .setGridParam({ datatype: "json" })
                                .trigger("reloadGrid");
                        },
                        1500);
                }

            });
        },
    });
    $("#gridSimulation").navGrid("#gridSimulationPager",
        // the buttons to appear on the toolbar of the grid
        { edit: false, add: false, del: false, search: true, refresh: false, view: false, position: "left", cloneToTop: false },
        // options for the Edit Dialog
        {
            editCaption: "The Edit Dialog",
            recreateForm: true,
            checkOnUpdate: true,
            checkOnSubmit: true,
            closeAfterEdit: true,
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        // options for the Add Dialog
        {
            closeAfterAdd: true,
            recreateForm: true,
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        // options for the Delete Dailog
        {
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        });



        //RecargarGrilla
        setInterval(function () {
            if (Object.keys(dicData).length === 0) return;

            $.ajax({
                url: baseUrl + '/Simulation/GetAll',
                success: function(newData) { //trago la data de la grilla actualzada
                    var refresh = false;
                    $.each(newData, function (i, e) {
                        //compara cada elemento con el que esta actualmente en la grilla 
                        //dicData es un diccionario por id, se carga en el evento loadComplete de la grilla
                        if (!(dicData.hasOwnProperty(e.Id) && dicData[e.Id].Status === e.Status && dicData[e.Id].Users === e.Users)) {
                            refresh = true;
                        }
                    });
                    if(refresh)
                        $("#gridSimulation")
                            .setGridParam({ datatype: "json" })
                            .trigger("reloadGrid");
                }
            });
        }, 20000);
    
});