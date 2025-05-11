$(document).ready(function () {
    var Common = $("#ProfilesJsData").data();
    var CheckDefault = false;
    var treeData = [];
    console.log(Common);
    
    $.ajax({
        url: Common.accesstreedataurl,
        method: "get",
        dataType: "json",
        async: false,
        success: function (tree) {
            treeData = tree;
        }
    });
    var TreeManager = {
        checkChildNodes: function (cssSelector, node) {

            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode["nodeId"];
                    $(cssSelector).treeview("checkNode", nodeId);
                }
            }
        },
        uncheckChildNodes: function (cssSelector, node) {

            var children = node.nodes;
            if (children) {
                for (var i = 0; i < children.length; i++) {
                    var childNode = children[i];
                    var nodeId = childNode["nodeId"];
                    $(cssSelector).treeview("uncheckNode", nodeId);
                }
            }
        },
        uncheckParentNode: function (cssSelector, node) {

            var parent = $(cssSelector).treeview("getParent", node);
            var nodeId = parent["nodeId"];
            if (nodeId !== undefined) {
                $(cssSelector).treeview("uncheckNode", [nodeId, { silent: true }]);
                TreeManager.uncheckParentNode(cssSelector, parent);
            }
        }
    };

    function updateTreeChecks(treeData, access) {

        for (var i = 0; i < treeData.length; i++) {
            treeData[i].state = {
                checked: ($.inArray(treeData[i].Id, access) !== -1)
            }
            if (treeData[i].nodes) updateTreeChecks(treeData[i].nodes, access);
        }

    }


    function isAccessCheck(treeData) {

        $("#list-group").find('input[type=checkbox]').each(function () {
            $(this).change(function () {
                var colid = $(this).parents('tr:last').attr('id');
                if ($(this).is(':checked')) {
                    $("#list1").jqGrid('setSelection', colid);
                    $(this).prop('checked', true);
                }
                return true;
            });
        });
        return false;
    }
 
    

    var gridTable = "#jqGridProfiles";
    var gridPager = gridTable + "Pager";

    var EditOptions = {
        editCaption: res.editProfile,
        bSubmit: res.save,
        recreateForm: true,
        checkOnUpdate: false,
        checkOnSubmit: false,
        closeAfterEdit: true,
        beforeSubmit: function (postdata) { //validacion de duplicado (solo anda con data local)
            if (isDuplicateInGrid(postdata, "Name", this))
                return [false, res.profileNameDuplicate];

            if (postdata.Access == "") {
                return [false, res.mustselectoneaccessprofile];
            }

            Bcri.Core.LoadingScreen.show();

            return [true];


        },
        afterSubmit: function () {
            $(gridTable).jqGrid("setGridParam", { datatype: "json" });
            $(gridTable).trigger("reloadGrid");

            Bcri.Core.LoadingScreen.hide();

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
                editrules: { required: true }
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
                        Enable: "Habilitado",
                        Disable: "Deshabilitado"
                    }
                },
                editrules: { required: true }
            },
            {
                label: "Acceso",
                name: "Access",
                hidden: true,
                editable: true,
                search: false,
                edittype: "custom",
                editrules: { edithidden: true },
                editoptions: {
                    custom_element: function (value, editOptions) {
                        updateTreeChecks(treeData, value.split(","));
                        var elem = $("<div id=\"accessTree\"></div>");
                        elem.treeview({
                            data: treeData,
                            showIcon: true,
                            showCheckbox: true
                        });
                        function getCheck(node, elem) {
                            var nodoPadre = elem.treeview.length();
                            if (nodoPadre.Id != undefined) {
                                if (nodoPadre.Id) {
                                    elem.treeview(true).checkNode(nodoPadre, { silent: true });
                                    getParents(nodoPadre, elem);
                                }
                            }
                        };
                        function getParents(node, elem) {
                            var nodoPadre = elem.treeview("getParent", node);
                            if (nodoPadre.Id != undefined) {
                                if (nodoPadre.Id) {
                                    elem.treeview(true).checkNode(nodoPadre, { silent: true });
                                    getParents(nodoPadre, elem);
                                }
                            }
                        };
                        //elem.on('click', function () { alert("vvv"); });
                        elem.on('nodeChecked ', function (ev, node) {
                            for (var i in node.nodes) {
                                var child = node.nodes[i];
                                $(this).treeview(true).checkNode(child.nodeId);
                            };
                            getParents(node, elem);

                        })
                        elem.on('nodeUnchecked ', function (ev, node) {
                            var check = false;

                            for (var i in node.nodes) {
                                var child = node.nodes[i];
                                $(this).treeview(true).uncheckNode(child.nodeId);
                            };

                            //verifica que los demas 
                            var nodoPadre = elem.treeview("getParent", node);
                            if (nodoPadre.Id != undefined) {
                                for (var i in nodoPadre.nodes) {
                                    var child = nodoPadre.nodes[i];
                                    if (child.state.checked) {

                                        check = true;
                                        break;
                                    }
                                }

                               
                                

                                checkAbuelo = false;
                                    var nodoAbuelo = elem.treeview("getParent", nodoPadre);
                                    if (nodoAbuelo.Id != undefined) {
                                        for (var i in nodoAbuelo.nodes) {
                                            var child = nodoAbuelo.nodes[i];
                                            if (child.state.checked) {
                                                checkAbuelo = true;
                                                break;
                                            }
                                        }
                                        if (!checkAbuelo) {
                                            $(this).treeview(true).uncheckNode(nodoAbuelo, { silent: true });
                                        }
                                    }
                                if (!check && nodoAbuelo.Id == undefined) {
                                    $(this).treeview(true).uncheckNode(nodoPadre, { silent: true });                                    
                                }
                            }



                        });

                        elem.treeview("collapseAll", { silent: true });
                        return elem;
                    },
                    custom_value: function (elem) {
                        if (!elem.length) return;
                        //elem.treeview(true).methodName(args);
                        var checks = elem.treeview(true).getChecked();
                        var ids = [];
                        for (var i = 0; i < checks.length; i++) {
                            ids.push(checks[i].Id);
                        }
                        return ids.join(",");
                    }
                }
            },
            {
                label: "editar",
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
                search: !Common.accessedit,
            }
        ], 
        sortname: "Name",
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
            addCaption: res.newProfile,
            closeAfterAdd: true,
            recreateForm: true,
            beforeSubmit: EditOptions.beforeSubmit,
            afterSubmit: EditOptions.afterSubmit,
            beforeShowForm: function (formid) {
                $("#State_Code>option:eq(2)").remove();
                $("#State_Code>option:eq(1)").remove();
               
            },
            errorTextFormat: function (data) {
                return "Error: " + data.responseText;
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
                return "Error: " + data.responseText;
            }
        });

  
});
