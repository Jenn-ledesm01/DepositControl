/// <reference path="AccessManagement.cshtml" />


function generateNodes(Access) {
    var nodes = [];
    $.each(Access, function (index, value) {
        nodes.push({
            data: value,
            text: value.text,
            icon: value.icon,
            children: value.nodes && Array.isArray(value.nodes) ?
                generateNodes(value.nodes) :
                undefined
        });
    });
    return nodes;
}


function disableNode(jstree, node_id) {
    var node = jstree.get_node(node_id);
    jstree.disable_node(node);

    //child disable
    node.children.forEach(function (child_id) {
        disableNode(jstree, child_id);
    });
}

function enableNode(jstree, node_id) {
    var node = jstree.get_node(node_id);
    jstree.enable_node(node);

    //child disable
    node.children.forEach(function (child_id) {
        enableNode(jstree, child_id);
    });
}

function forAllNodes(idJsTree, functionToApply) {
    $("#" + idJsTree + ' >ul > li').each(function () {
        functionToApply($("#" + idJsTree).jstree(), this.id);
    });
}

$(function () {

    var nodesMovedToSave = [];

    var treeViewElement = $("#tree-access");
    var accessConfigElemnt = $("#access-config");
    var urlsElemnt = $("#AccessManagementJsData");

    var urlAccessTreeData = $(urlsElemnt).attr("url-AccessTreeData");
    var urlAccessData = $(urlsElemnt).attr("url-AccessData");
    var urlAccessTreeDataSave = $(urlsElemnt).attr("url-AccessTreeDataSave");
    var urlAccessDataDelete = $(urlsElemnt).attr("url-AccessDataDelete");

    //Botonera
    var btntreeAccessAdd = $("#tree-access-add");
    var btntreeAccessReload = $("#tree-access-reload");
    var btntreeAccessSave = $("#tree-access-save");
    var btntreeAccessDel = $("#tree-access-del");
    var btntreeAccessDelConfirm = $("#tree-access-del-confirm")
    var btntreeAccessCancel = $("#tree-access-cancel")

    //Funciones de ayuda
    var showConfirmMove = function () {
        $("#botoneraJstree").hide();
        $("#confirm-moving").show();
    };
    var showBotonera = function () {
        $("#botoneraJstree").show();
        $("#confirm-moving").hide();
    };
    var lockFormAccessData = function () {
        if (document.getElementById("access-save") !== null) {
            $("#access-save").addClass("disabled");
            $("#access-cancel").addClass("disabled");
            
            $(accessConfigElemnt).find("fieldset").attr("disabled", "disabled");
        }
    };
    var unlockFormAccessData = function () {
        if (document.getElementById("access-save") !== null) {
            $("#access-save").removeClass("disabled")
            $("#access-cancel").removeClass("disabled");;
            $(accessConfigElemnt).find("fieldset").removeAttr("disabled");
        }
    };
    var PrintSuccess = function (msg) {
        $("#alerts-AccessManagement").showAlert(msg, "success", 2000);
    };
    var PrintError = function (msg) {
        $("#alerts-AccessManagement").showAlert(msg, "error", 10000);
    };


    /*
        Creo el nodo y desactivo todos 
        para obligar a guardar al nuevo 
        nodo y luego asignar el padre o la
        estructura que corresponde al guardar
        el tree
    */

    //create_node ([par, node, pos, callback, is_loaded])
    $(btntreeAccessAdd).on("click",
        function (e) {
            var jstree = $(treeViewElement).jstree();

            var parentNode = null;
            var id_node_selected = jstree.get_selected()[0];
            if (id_node_selected) {
                var selected_node = jstree.get_node(id_node_selected);
                parentNode = selected_node.id;
            }
            //Creo el nodo
            var new_node = jstree.create_node(
                parentNode,
                {
                    text: res.newNew,
                    data: {
                        Id: 0
                    }
                }
            );


            jstree.deselect_all();
            //select_node(obj[, supress_event, prevent_open])
            jstree.select_node(new_node);

            //Deasctivo el jstree
            forAllNodes(jstree.element.attr("Id"), disableNode);

            //Desactivo la botonera
            $("#botoneraJstree .btn").each(function (index, element) {
                // element == this
                $(element).addClass("disabled");
            });

        });


    //Crea el arbol y configura sus eventos
    var CreateJsTree = function () {

        nodesMovedToSave = [];
        showBotonera();

        //Limpio el contenedor
		$(treeViewElement).empty();
        var whitelist = ["id"];
        $(treeViewElement).each(function () {
            var attributes = this.attributes;
            var i = attributes.length;
            while (i--) {
                var attr = attributes[i];
                if ($.inArray(attr.name, whitelist) == -1)
                    this.removeAttributeNode(attr);
            }
        });

        $.get(
            urlAccessTreeData,
            function (data, textStatus, jqXHR) {
                $(treeViewElement).jstree({
                    plugins: ["dnd"],
                    core: {
                        multiple: false,
                        check_callback: true,
                        data: generateNodes(data),
                        themes: {
                            name: 'proton',
                            responsive: true
                        }
                    }
                });

                //Evento del treeview
                $(treeViewElement).on("select_node.jstree",
                    //Cargo los datos del nodo seleccionado
                    function (e, data) {
                        $(accessConfigElemnt).load(
                            urlAccessData + "/" + data.node.data.Id,
                            function () {
                                var jstree = $(treeViewElement).jstree();
                                var selectedNode_Id = jstree.get_selected()[0];
                                var selectedNode = jstree.get_node(selectedNode_Id);

                                var isNew = selectedNode.data.Id == 0;
                                if (isNew) {
                                    var parent_id = selectedNode.parent;
                                    if (parent_id == null || parent_id == "#") {
                                        //Es un root
                                        $("#form-access-data #Parent_Id").val(null)
                                    } else {
                                        var parent = jstree.get_node(parent_id).data.Id; //El verdadero Id Padre
                                        $("#form-access-data #Parent_Id").val(parent);
                                    }

                                    //Evento cancelar que refrezca la grilla y cancela
                                    $("#access-cancel").on('click', function () {
                                        CreateJsTree();
                                        $(accessConfigElemnt).empty();
                                        
                                        //activo la botonera
                                        $("#botoneraJstree .btn").each(function (index, element) {
                                            // element == this
                                            $(element).removeClass("disabled");
                                        });
                                    });
                                }

                                //Calbacks de la ventana para cuando guarde con exito o no
                                if (callback_saveSuccess) {
									callback_saveSuccess = function (newAcceess) {

										//Activo todos los nodos
										forAllNodes(treeViewElement.attr("id"), enableNode);

										//Activo la botonera
										$("#botoneraJstree .btn").each(function (index, element) {
											// element == this
											$(element).removeClass("disabled");
										});

										//Recargo la grilla para que tenga los nuevos valores
										//CreateJsTree();
									};
                                }
                            }
                        );
                    });

                $(treeViewElement).on("move_node.jstree", function (e, obj) {
                    showConfirmMove();
                    lockFormAccessData();
                    
                    nodesMovedToSave.push(obj.node);
                });
            }
        );
    }

    CreateJsTree();

    $(btntreeAccessReload).on("click", CreateJsTree);

    $(btntreeAccessSave).on("click",
        function () {            
            var jstree = $(treeViewElement).jstree();            
            var nodes = nodesMovedToSave;
            var accesses = [];

			var hash = {}; //elimina duplicados
			nodes = nodes.filter(function (current) {
				var exists = !hash[current.id] || false;
				hash[current.id] = true;
				return exists;
			});

            $.each(nodes, function (index, node) {

				var parent_Id = null;
				var parentNode = jstree.get_node(node.parent);

				parent_Id = parentNode.data && { Id: parentNode.data.Id };
				
				accesses.push(...parentNode.children.map(function (node, i) {
					node = jstree.get_node(node);
					return {
						Id: node.data.Id,
						Parent: parent_Id,
						Posicion: i
					};
				}));				
            });


            Bcri.Core.LoadingScreen.show();

            $.ajax({
                type: "POST",
                url: urlAccessTreeDataSave,
                data: {
                    accesses: accesses
                },
                dataType: "json",
                success: function (response) {
                    if (response.anyError) {
                        PrintError(response.mensageResponse);
                    } else {
                        PrintSuccess(response.mensageResponse);

                        showBotonera();
                        unlockFormAccessData();
                        nodesMovedToSave = [];

                    }
                },
                error: function (e) {
                    PrintError(e);
                },
                complete: Bcri.Core.LoadingScreen.hide
            });

        }
    );


    $(btntreeAccessDel).on("click", function (e) {
        var jstree = $(treeViewElement).jstree();

        var selected_id = jstree.get_selected()[0];
        if (!(selected_id)) {
            PrintError(res.pleaseSelectANode);

            $("#modalConfirmDelete #tree-access-del-confirm").hide();
            $("#modalConfirmDelete .withSelectedNode").hide();
            $("#modalConfirmDelete .withNoSelectedNode").show();
        } else {
            $("#modalConfirmDelete #tree-access-del-confirm").show();
            $("#modalConfirmDelete .withSelectedNode").show();
            $("#modalConfirmDelete .withNoSelectedNode").hide();


            $("#modalConfirmDelete .selectedNode").html(
                jstree.get_node(selected_id).data.Name
            );
        }
    })

    $(btntreeAccessDelConfirm).on("click", function (e) {
        var jstree = $(treeViewElement).jstree();

        var selected_id = jstree.get_selected()[0];
        $.ajax({
            type: "POST",
            url: urlAccessDataDelete,
            data: {
                access: jstree.get_node(selected_id).data
            },
            dataType: "json",
            success: function (response) {
                if (!response.anyError) {
                    PrintSuccess(response.mensageResponse);

                    jstree.delete_node(selected_id);

					$(accessConfigElemnt).empty();

                } else {
                    PrintError(response.mensageResponse);
                }
            }
        });


    });

	$(btntreeAccessCancel).on('click', function (e) {
		showBotonera();
		unlockFormAccessData();
		CreateJsTree();
	});
});

