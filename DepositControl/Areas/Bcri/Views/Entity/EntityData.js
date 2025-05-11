function getFromController(method, data, fnSuccess) {//Se escribe el metodo aca para disponibilizarlo en ambos archivos (Index.js, Entity.js)
    if ($("#entityId>option:selected").text()) {
        $.get(baseUrl + "/Entity/" + method, data, fnSuccess)
            .fail(function (err) {
                $("#successFail").showAlert("<strong>Error!</strong> " + err.responseText, 'error');
            });
    }
}

function postSuccessMsg(msg) {
    $("#successFail").html(msg);
    setTimeout(function () { $('.alert').alert("close"); }, 2500);
}

function initPartial() {
    $("#saveEntity").click(function () {
        if (!$("#entityExists").prop("checked"))
            return;

        var entity = {
            Id: $("#entityId>option:selected").val(),
            LargeData: $("#LargeData").prop("checked"),
            IsLoggeable: $("#IsLoggeable").prop("checked"),
            IsTranslatable: $("#IsTranslatable").prop("checked")
        }
        $.post(baseUrl + "/Entity/SaveEntity", { entity: entity, repositoryName: $("#entityName").val(), repositoryType: $("#repositoryTypeId").val() }, function (resp) {
            $("#successFail").html(resp);
            setTimeout(function () { $('.alert').alert("close"); }, 2500);
        });
    });

    $("#generateRepo").click(function () {
        getFromController("GenerateRepository", { entity: $("#entityId>option:selected").text() }, postSuccessMsg);
        $("#entityId").trigger("change");
    });

    $("#generateEntity").click(function () {
        getFromController("GenerateEntity", { entity: $("#entityId>option:selected").text() }, function (msg) {
            postSuccessMsg(msg);
            window.location.href = baseUrl + "Entity?Name=" + $("#entityId>option:selected").text();
        });
        $("#entityId").trigger("change");
    });

    $("#EntityGrid").jqGrid({
        editurl: baseUrl + "/Grid/Edit",
        colModel: [
            {
                name: "Id",
                hidden: true,
                editable: false
            },
            {
                name: "Entity_Id",
                hidden: true,
                editable: false
            },
            {
                label: "Field Name",
                name: "Name",
                editable: false
            },
            {
                name: "Description",
                editable: true
            },
            {
                label: "Data Type",
                name: "DataType",
                editable: false
            },
            {
                label: "Reference Entity",
                name: "RefEntity",
                editable: false
			},
			{
				label: "Order",
				name: "order",
				editable: false,
				formatter: function () { return $('<spam><i class="fas fa-grip-vertical"></i></spam>') }
			}
        ],
        pager: "#EntityGridPager",
        datatype: "clientside",
        sortable: true,
        cellEdit: true,
        cellsubmit: 'remote',
        cellurl: 'Entity/ModifyStructDescription',
        beforeSubmitCell: function (rowid, cellname, value, iRow, iCol) {
            debugger;

            var data = $(this).getRowData(rowid);

            return {
                Id: data.Id,
                Entity_Id: data.Entity_Id,
                Name: data.Name,
                Description: value
            }
        }
    });
	$("#EntityGrid").jqGrid('sortableRows');

    $("#EntityGrid").navGrid("#EntityGridPager",
        {
            edit: false,
            add: false,
            del: false,
            search: false,
            refresh: false,
            view: false,
            position: "left",
            cloneToTop: false
        },
        {
            editCaption: "Edit",
            recreateForm: true,
            closeAfterEdit: true,
            viewPagerButtons: false,
            beforeSubmit: function (postdata, $form) {
            },
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        {
            closeAfterAdd: true,
            recreateForm: true,
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        },
        {
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
            }
        }
    );

    getFromController("getStructs", { EntityId: $("#entityId>option:selected").val() }, function (data) {
        $('#EntityGrid').jqGrid('setGridParam', { data: data }).trigger('reloadGrid');
    });
}