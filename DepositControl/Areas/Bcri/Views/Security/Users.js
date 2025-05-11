
$(document).ready(function () {
    var Common = $("#UsersJsData").data();   
    var gridTable = "#jqGridUsers";
    var gridPager = gridTable + "Pager";
    var EditOptions = {
        editCaption: res.editUser,
        recreateForm: true,
        checkOnUpdate: false,
        checkOnSubmit: false,
        closeAfterEdit: true,
        beforeSubmit: function (postdata) {//validacion de duplicado (solo anda con data local)
            grid = $(this);
            var id = postdata[grid.attr("Id") + "_id"];
            if (isDuplicateInGrid(postdata, "Email", this))
                return [false, res.existusersameemail];
            if (isDuplicateInGrid(postdata, "Name", this))
                return [false, res.existusersamename];
            if (!Common.password) {
                if (id === "_empty" && !postdata.Password)
                    return [false, res.usermusthavepassword];
            }
            if (postdata.Profiles == "") {
                return [false, res.mustselectoneprofileuser];
            }
            return [true];
        },
        afterSubmit: function () {
            $(gridTable).jqGrid("setGridParam", { datatype: "json" });
            $(gridTable).trigger("reloadGrid");
            return [true];
        },
        viewPagerButtons: false,
        errorTextFormat: function (data) {
            return "Error: " + data.responseText;
        }
    }
    

    $(gridTable).jqGrid({
        url: Common.dataurl,
        editurl: Common.editurl,
        datatype: "json",
        autowidth: true,
        colModel: [
            {
                label: "Id",
                name: "Id",
                key: true,
                hidden: true
            },
            {
                label: "Nombre",
                name: "Name",
                width: 100,
                edittype: "text",
                editable: true,
                editoptions: { maxlength: 100 },
                editrules: {
                    //custom rules
                    required: true
                }
            },
            {
                label: "Nombre completo",
                name: "FullName",
                width: 220,
                edittype: "text",
                editable: true,
                editoptions: { maxlength: 250 },
                editrules: { required: true }
            },
            {
                label: "Email",
                name: "Email",
                width: 220,
                formatter: "email",
                edittype: "text",
                editable: true,
                editoptions: { maxlength: 250 },
                editrules: {
					custom: true, email: true, required: true,
					custom_func: function (mail) {
						for (var letter in mail) {
							//if (mail[letter] === mail[letter].toUpperCase()) {
							//	return [false, "the email must not have any uppercase letters"];
							//}
						}
						return [true];
					}
                }
            },
            {
                label: "Estado",
                name: "State_Code",
                edittype: "select",
                editable: true,
                width: 70,
                fixed: true,
                editoptions: {
                    value: {
                        Enable: res.enable,
                        Disable: res.disable
                    }
                },
                editrules: { required: true }
            },
            {
                label: "Último inicio",
                name: "LastLogin",
                width: 120,
                fixed: true,
                editable: false,
                formatter: 'date',
                formatoptions: {
                    srcformat: 'd-m-Y',
                    newformat: 'd-m-Y'
                },
                index: "LastLogin",
                sorttype: "date",
                searchoptions: {
                    sopt: ['eq', 'ge', 'lt'],
                    dataInit: function (element) {
                        $(element).datepicker({
                            dateFormat: 'dd-mm-yy',
                            showOn: 'focus'
                        });
                    },
                }
            },
            {
                label: "Contraseña",
                name: "Password",
                fixed: !Common.password,
                hidden: !Common.password,
                edittype: "password",
                editrules: { edithidden: !Common.password, required: false },
                editoptions: { maxlength: 50 },
                editable: !Common.password,
                hidden: true
            },
            {
                label: "Perfiles",
                name: "Profiles",
                editable: true,
                search: false,
                edittype: "custom",
                editoptions: {
                    custom_element: function (value, editOptions) {
                        var elem = $("#profilesControl").clone().css("display", "block");

                        if (editOptions.rowId === "_empty")
                            return elem;

                        var idProfiles = $(gridTable).jqGrid("getRowData", editOptions.rowId).ProfilesIds.split(",");// "1,2"

                        for (var i = 0; i < idProfiles.length; i++) {
                            if (idProfiles[i] !== "") {
                                var id = idProfiles[i];

                                var check = elem.find("input[type=checkbox][value=" + id + "]").attr("checked", true);
                                updateCheck(check);
                            }
                        }
                        elem.find("input[type=checkbox]").on("change", function () {
                            updateCheck($(this));
                        });

                        function updateCheck(check) {
                            var li = check.closest("li.list-group-item");
                            var label = check.parent();
                            if (check.is(":checked")) {
                                li.css("background-color", " #337ab7");
                                label.css("color", "white");
                            }
                            else {
                                li.css("background-color", "");
                                label.css("color", "black");
                            }
                        }
                        return elem;
                    },
                    custom_value: function (elem) {
                        var checks = elem.find("input[type=checkbox]:checked");
                        var ids = [];
                        for (var i = 0; i < checks.length; i++) {
                            ids.push($(checks[i]).val());
                        }
                        return ids.join(",");
                    }
                }
            },
            {
                label: "",
                name: "ProfilesIds",
                editable: false,
                hidden: true,
                search: false
            },
            {
                label: "Editar",
                name: "edit",
                width: 50,
                hidden: !Common.accessedit,
                fixed: true,
                formatter: "actions",
                formatoptions: {
                    keys: true,
                    editOptions: EditOptions,
                    editformbutton: true,
                    delbutton: false
                },
                search: !Common.accessedit
            }
        ],
        sortname: "FullName",
        sortorder: "ASC",
        loadonce: true,
        viewrecords: true,
        pager: gridPager
        //gridComplete: function () {
        //    var allRowsInGrid = $(gridTable).jqGrid('getRowData');            
        //        for (i = 0; i < allRowsInGrid.length; i++) {
        //            if (allRowsInGrid[i].State_Code == res.deleted)
        //            {
        //                $("#jEditButton_" + allRowsInGrid[i].Id).hide();                        
        //            }                   
        //        }
        //}
    });

    $(gridTable).navGrid(gridPager,
        // the buttons to appear on the toolbar of the grid
        { edit: false, add: Common.accessnew, del: Common.accessdelete, search: true, refresh: true, view: false, position: "left", cloneToTop: false },
        // options for the Edit Dialog
        EditOptions,
        // options for the Add Dialog
        {
            addCaption: res.newUser,
            closeAfterAdd: true,
            recreateForm: true,
            beforeSubmit: EditOptions.beforeSubmit,
            afterSubmit: EditOptions.afterSubmit,
            errorTextFormat: function (data) {
                return res.error + ": " + data.responseText;
            }
        },
        // options for the Delete Dailog
        {
            afterSubmit: function () {
                $(gridTable).jqGrid("setGridParam", { datatype: "json" });
                $(gridTable).trigger("reloadGrid");
                return [true];
            },
            errorTextFormat: function (data) {
                return res.error + ": " + data.responseText;
            }
        });
});
