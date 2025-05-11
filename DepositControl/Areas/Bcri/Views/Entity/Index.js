$(document).ready(function () {

    $("#entityId").on('change', function (e) {
        $("#EntityContainer").show();
        $("#EntityContainer").load(baseUrl + "/Entity/EntityData", { EntityId: $(this).val() },
            function () {
                if ($("#entityId").val() !== "")
                    initPartial();
            }
        );
    });

    //agrego el primer campo que aparece por defecto vacio en caso de que no se haya elegido ninguna entidad
    if ($("#entityId>option:selected").length !== 0){
        $("#entityId").trigger("change");
    }
    $("#repositoryTypeId").html("<option value='' selected disabled></option>" + $("#repositoryTypeId").html());
});