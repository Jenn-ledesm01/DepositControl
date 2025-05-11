$(document).ready(function () {
    var Common = $("#GridCustomViewJsData").data();
    var GridCodesDefault = {};
    var filtering = false;

    //$(document).on('click', '#dropDwnMenu', function (e) {   
    //    e.stopPropagation();
    //    e.preventDefault();
    //  //  $j("btn-group open").toggle();
    //    // $('#dropDwnMenu').toggle(); 

    //    $(this).on('click', '#panel1', function (e) {
    //        //e.stopPropagation();
    //        e.stopImmediatePropagation();
    //         $('#dropDwnMenu').hide();
    //         //if (!e.stopPropagation) {
    //         //    e.stopPropagation();
    //         //} else {
    //         //    e = window.event;
    //         //    e.cancelBubble = true;
    //         //}
    //         //e.cancelBubble = true;
    //    });
    //});

    // $('#dropDwnMenu').on('click', function (event) {
    //   $('#dropDwnMenu').parent().toggleClass('open');
    //});

    //$('#panel1').on('click',function () {
    //   $(this).parents('.dropdown-menu').find('button.dropdown-toggle').dropdown('toggle');
    //  $('#dropDwnMenu .dropdown-toggle').click()
    //});
    //$(this).ready(function () {
    //    $('#panel1').mouseleave(function () {

    //        $(".dropdown-toggle").dropdown('toggle');
    //    });
    //});

    //$('#panel1').on('click', function (e) {
    //    $(this).next('.dropdown-menu').toggle();
    //    e.stopPropagation();
    //});
    $(this).ready(function () {
        $("#panel1").click(function () {
            $("#dropDwnMenu").slideToggle("slow");
        });
    });
    CustomGridParseAll = PaseGrids

    PaseGrids();
    function PaseGrids() {
        $('.dx-datagrid').each(function () {
            var grid = $(this).parent();
            var gridCode = grid[0].id;
            if (grid.attr('data-hascustomview') == 'false')
                return;
            if (gridCode == "")
                return;
            if ($('#CustomView-' + gridCode).length > 0)
                return;

            GetStateDefault(gridCode);
            var GridInstance = $('#' + gridCode).dxDataGrid('instance');
            $('*[is-buttonfilter]').on('click', function () { filtering = true; });
            GridInstance.on("contentReady", function () {
                if (filtering) {
                    filtering = false;
                    Load(gridCode);
                }
            });


            GridCodesDefault[gridCode] = GridInstance.state();
            $('<div id="CustomView-' + gridCode + '"></div>').load(Common.index + '?gridCode=' + gridCode
                , function (responseTxt, statusTxt, xhr) {
                    var $this = $(this);
                    var ddlcustomviews = $this.find("#" + gridCode + "-select_CustomGridView");
                    ddlcustomviews.change(function () { Load(gridCode); });

                    $this.find("#" + gridCode + "-MenuRename").click(function () {
                        if ($("#" + gridCode + "-MenuRename").attr('class') != "disabled") {
                            Rename(gridCode);
                        }
                    });
                    $this.find("#" + gridCode + "-MenuSaveAs").click(function () { SaveAs(gridCode); });
                    $this.find("#" + gridCode + "-MenuSave").click(function () {
                        if ($("#" + gridCode + "-MenuSave").attr('class') != "disabled") {
                            id = $('#' + gridCode + '-select_CustomGridView :selected').val();
                            name = $('#' + gridCode + '-select_CustomGridView :selected').text();
                            Save(id, name, gridCode, false);
                        }
                    });
                    $this.find("#" + gridCode + "-MenuSetAsDefaultMe").click(function () {
                        if ($("#" + gridCode + "-MenuSetAsDefaultMe").attr('class') != "disabled") {
                            SetAsDefaultMe(gridCode);
                        }
                    });
                    $this.find("#" + gridCode + "-MenuSetAsDefaultForUsers").click(function () {
                        if ($("#" + gridCode + "-MenuSetAsDefaultForUsers").attr('class') != "disabled") {
                            SetAsDefaultForUsers(gridCode, 'default');
                        }
                    });
                    $this.find("#" + gridCode + "-MenuShare").click(function () {
                        if ($("#" + gridCode + "-MenuShare").attr('class') != "disabled") {
                            Share(gridCode);
                        }
                    });
                    $this.find('#' + gridCode + '-MenuDelete').on('click', function () {
                        if ($("#" + gridCode + "-MenuDelete").attr('class') != "disabled") {
                            Delete(gridCode);
                        }
                    });
                    InitForm(ddlcustomviews);
                    ddlcustomviews.change(function () {
                        InitForm(this);
                    });
                    grid.before($this);
                    //ddlcustomviews.val("0");
                    //if (ddlcustomviews.val() != "0")
                    //    Load(gridCode);
                    UpdatePopup(gridCode);
                });

        });
    }


    function Rename(gridCode) {
        BootstrapDialog.show({
            title: 'Rename',
            message: $('<textarea class="form-control" id="txtnameState" placeholder="Input name...">' + $("#" + gridCode + "-select_CustomGridView option:selected").text() + '</textarea><br><div id="MesgError"/>'),
            buttons: [
                {
                    label: 'Rename',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action:
                        function (dialogItself) {
                            var nombre = $("#txtnameState").val();
                            if (nombre == "") {
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name is empty.");
                            }
                            else if (!validateName(gridCode, nombre.toLowerCase())) {
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name already exists.");

                            }
                            else if (nombre.toLowerCase() == "(default)") {
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name is not valid.");

                            }
                            else {
                                $.ajax({
                                    url: Common.rename,
                                    global: false,
                                    type: "GET",
                                    data: {
                                        gridCustomViewId: $("#" + gridCode + "-select_CustomGridView").val(),
                                        Name: nombre
                                    },
                                    async: false,
                                    success: function (response) {
                                        if (response == "404")
                                            $("#ShowMessages").showAlert("<strong>Success!</strong>you haven't permission to rename");
                                        else {
                                            $("#ShowMessages").showAlert("<strong>Success!</strong>" + response + "");
                                            $("#" + gridCode + "-select_CustomGridView option:selected").text(nombre);
                                            SortItems(gridCode);
                                        }

                                    }, error: function (xhr, ajaxOptions, thrownError) {
                                        $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while renaming.");
                                    }
                                });
                                dialogItself.close();
                            }
                        }
                }
            ]
        });
        setTimeout(function () { $("#txtnameState").focus() }, 500);
    }
    function GetStateDefault(gridCode) {
        var dataGrid = $('#' + gridCode).dxDataGrid('instance');
        $.ajax({
            url: Common.load,
            global: false,
            type: "GET",
            data: {
                id: 0,
                gridCode: gridCode
            },
            dataType: "json",
            async: false,
            success: function (state) {
                if (state != "")
                    dataGrid.state($.parseJSON(state));
            }, error: function (xhr, ajaxOptions, thrownError) {
                $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while loading state.");
            }
        });
    }
    function Load(gridCode) {
        var dataGrid = $('#' + gridCode).dxDataGrid('instance');
        var id = $("#" + gridCode + "-select_CustomGridView").val();
        if (id != "0") {
            $.ajax({
                url: Common.load,
                global: false,
                type: "GET",
                data: {
                    id: $("#" + gridCode + "-select_CustomGridView").val(),
                    gridCode: ''
                },
                dataType: "json",
                async: false,
                success: function (dataJson) {
                    dataGrid.state($.parseJSON(dataJson));
                    UpdatePopup(gridCode);
                }, error: function (xhr, ajaxOptions, thrownError) {
                    $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while loading state.");
                }
            });
        } else {
            dataGrid.state(GridCodesDefault[gridCode]);
            UpdatePopup(gridCode);
        }

    }

    function SaveAs(gridCode) {
        BootstrapDialog.show({
            title: 'Save As',
            message: $('<textarea class="form-control" id="txtnameState" placeholder="Input name..."></textarea><br><div id="MesgError"/>'),
            buttons: [
                {
                    label: 'Save',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action:
                        function (dialogItself) {
                            var nombre = $("#txtnameState").val();
                            if (nombre == "") {
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name is empty.");
                            }
                            else if (nombre.toLowerCase() == "(default)") {
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name is not valid.");
                            }
                            else if (!validateName(gridCode, nombre.toLowerCase()))
                                $("#MesgError").showAlert("<strong>Failed!</strong> Name exist yet.");
                            else {
                                Save(0, $("#txtnameState").val(), gridCode, true);
                                dialogItself.close();
                            }
                        }
                }
            ]
        });
        setTimeout(function () { $("#txtnameState").focus() }, 500);
    }

    function Save(id, name, gridCode, IsNewItem) {
        var URLQueryString;
        var state = new FormData();
        if (name == "(default)")
            return;
        state.append('state', JSON.stringify($('#' + gridCode).dxDataGrid('instance').state()));
        URLQueryString = Common.save + '?Id=' + id;
        URLQueryString += '&name=' + name;
        URLQueryString += '&GridCode=' + gridCode;
        $.ajax({
            url: URLQueryString,
            global: false,
            processData: false,
            type: "POST",
            data: state,
            dataType: "text",
            async: false,
            success: function (new_item_id) {
                if (new_item_id == "-404") {
                    $("#ShowMessages").showAlert("<strong>Success!</strong>you haven't permission to rename");
                }
                else if (IsNewItem) {
                    AppendItem(gridCode, new_item_id, name);
                }

                $("#ShowMessages").showAlert("<strong>Success!</strong> The custom view was saved.");
                UpdatePopup(gridCode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while saving state.");
            }
        });

    }

    function validateName(gridCode, name) {
        isvalid = true
        $.each($("#" + gridCode + "-select_CustomGridView").prop('options'), function () {
            if (this.outerText.toLowerCase() == name.toLowerCase()
                && $("#" + gridCode + "-select_CustomGridView").val().toLowerCase() != this.outerText.toLowerCase())
                isvalid = false;
        });
        return isvalid;
    }
    function Delete(gridCode) {
        var ddlCustomViews = $("#" + gridCode + "-select_CustomGridView");
        if (ddlCustomViews.val() == "0")
            return;
        $.ajax({
            //url: '/GridCustomView/Delete',
            url: Common.delete,
            global: false,
            type: "GET",
            data: {
                id: ddlCustomViews.val()
            },
            dataType: "text",
            async: false,
            success: function (ReponseDelete) {
                if (ReponseDelete == "404")
                    $("#ShowMessages").showAlert("<strong>Success!</strong>you haven't permission to rename");
                else {
                    ddlCustomViews.find("option[value='" + ddlCustomViews.val() + "']").remove();
                    ddlCustomViews.val("0");
                    Load(gridCode);
                    $("#ShowMessages").showAlert("<strong>Success!</strong> The custom view was deleted.");
                }
            }, error: function (xhr, ajaxOptions, thrownError) {
                $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while deleting state.");
            }
        });
    }

    function UpdatePopup(gridCode) {
        var ownerName = "";
        var ddlCustomViews = $("#" + gridCode + "-select_CustomGridView");
        if (ddlCustomViews.val() == "0") {
            $("#" + gridCode + "-MenuOwner").find("a").html("Owner: Factory");
            $("#" + gridCode + "-MenuShare").addClass("disabled");
            $("#" + gridCode + "-MenuSetAsDefaultMe").addClass("disabled");
            $("#" + gridCode + "-MenuSetAsDefaultUsersAndProfiles").addClass("disabled");
            $("#" + gridCode + "-MenuRename").addClass("disabled");
            $("#" + gridCode + "-MenuSave").addClass("disabled");
            $("#" + gridCode + "-MenuDelete").addClass("disabled");
        }
        else {
            $("#" + gridCode + "-MenuShare").removeClass("disabled");
            $("#" + gridCode + "-MenuSetAsDefaultMe").removeClass("disabled");
            $("#" + gridCode + "-MenuSetAsDefaultUsersAndProfiles").removeClass("disabled");
            $.ajax({
                type: "POST",
                data: { gridCustomViewId: $("#" + gridCode + "-select_CustomGridView").val() },
                url: Common.getowner,
                success: function (msg) {
                    ownerName = msg;
                    $("#" + gridCode + "-MenuOwner").addClass("disabled");

                    if (ddlCustomViews.val() == "0" || ownerName != "") {
                        $("#" + gridCode + "-MenuRename").addClass("disabled");
                        $("#" + gridCode + "-MenuSave").addClass("disabled");
                        $("#" + gridCode + "-MenuDelete").addClass("disabled");
                    } else {
                        $("#" + gridCode + "-MenuRename").removeClass("disabled");
                        $("#" + gridCode + "-MenuSave").removeClass("disabled");
                        $("#" + gridCode + "-MenuDelete").removeClass("disabled");
                    }
                    if (ownerName == "")
                        ownerName = "Me";
                    $("#" + gridCode + "-MenuOwner").find("a").html("Owner: " + ownerName);
                }, error: function (xhr, ajaxOptions, thrownError) { }
            })
        };

    }

    function Share(gridCode) {
        var id = $("#" + gridCode + "-select_CustomGridView").val();
        BootstrapDialog.show({
            title: "Share for users",
            message: $('<div style="width: 100%; height: 200px; overflow-y: scroll;"></div>').load(Common.chooseprofiles + "?gridCustomViewId=" + id + "&entity=share"),
            buttons: [
                {
                    label: 'Apply',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action:

                        function (dialogItself) {
                            ProfilesChecked = [];
                            $(".ChkProfiles").each(function () {
                                if (this.checked)
                                    ProfilesChecked.push(this.name);
                            });
                            $.ajax({
                                type: "POST",
                                data: {
                                    ProfilesChecked: ProfilesChecked,
                                    gridCustomViewId: id
                                },
                                url: Common.share,
                                success: function (msg) {
                                    $("#ShowMessages").showAlert("<strong>Success!</strong> The view has been shared successfuly.");
                                    dialogItself.close();
                                }, error: function (xhr, ajaxOptions, thrownError) {
                                    $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while sharing view.");
                                    dialogItself.close();
                                }
                            });
                        }
                }
            ]
        });
    }


    function SetAsDefaultForUsers(gridCode) {
        var id = $("#" + gridCode + "-select_CustomGridView").val();
        BootstrapDialog.show({
            title: "Set As Default For Users",
            message: $('<div style="width: 100%; height: 200px; overflow-y: scroll;"></div>').load(Common.chooseprofiles + "?gridCustomViewId=" + id + "&entity=default"),
            buttons: [
                {
                    label: 'Apply',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action:

                        function (dialogItself) {
                            ProfilesChecked = [];
                            $(".ChkProfiles").each(function () {
                                if (this.checked)
                                    ProfilesChecked.push(this.name);
                            });
                            $.ajax({
                                type: "POST",
                                data: {
                                    ProfilesChecked: ProfilesChecked,
                                    gridCustomViewId: id
                                },
                                url: Common.setasdefaultforusers,
                                success: function (return_new_item) {
                                    $("#ShowMessages").showAlert("<strong>Success!</strong> The view has been shared successfuly.");
                                    dialogItself.close();
                                    if (return_new_item != "") {
                                        AppendItem(gridCode, return_new_item.split("!")[0], return_new_item.split("!")[1]);
                                    }
                                }, error: function (xhr, ajaxOptions, thrownError) {
                                    $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while sharing view.");
                                    dialogItself.close();
                                }
                            });
                        }
                }
            ]
        });
    }
    function AppendItem(gridCode, value, name) {
        $('#' + gridCode + '-select_CustomGridView').append('<option value="' + value + '" selected="selected">' + name + '</option>');
        SortItems(gridCode);
    }
    function SortItems(gridCode) {
        $('#' + gridCode + '-select_CustomGridView').html(
                $('#' + gridCode + '-select_CustomGridView option')
                .sort(function (x, y) { return $(x).text() < $(y).text() ? -1 : 1; })
                );
    }

    function SetAsDefaultMe(gridCode) {
        var ddlCustomViews = $("#" + gridCode + "-select_CustomGridView");
        if (ddlCustomViews.val() == "0")
            return;
        $.ajax({
            url: Common.setasdefaultme,
            global: false,
            type: "GET",
            data: {
                Index: ddlCustomViews.val()
            },
            dataType: "text",
            async: false,
            success: function (ReponseSetAsDefault) {
                $("#ShowMessages").showAlert("<strong>Success!</strong> The view now is default.");
                // respuesta
            }, error: function (xhr, ajaxOptions, thrownError) {
                $("#ShowMessages").showAlert("<strong>Fail!</strong> Error while setting as default.");
            }
        });
    }

});

