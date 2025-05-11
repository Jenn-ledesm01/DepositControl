@using DNF.Structure.Bussines;
@using Bcri.Core.Bussines;
@model Entity
@using Type = DNF.Type.Bussines.Type;
@{
    bool IsRepository = (Model?.Structs?.Any(x => x.Name == "Repository_Id") ?? false) && ((ViewBag.repoCfgEntity?.Count ?? 0) > 0);
    bool EntityExists = (Model?.Structs?.Count ?? 0) > 0;
}
<style type="text/css">
    h3 {
        font-size: 1.5em;
    }

	.fa-grip-vertical {
		cursor: grab;
	}
	.fa-grip-vertical:active {
		cursor: grabbing;
	}

</style>

<h3>@Res.res.entityData</h3>
<div class="row">
    @if (IsRepository)
    {
        <div class="form-group col-lg-3">
            <label for="entityName">@Res.res.displayedEntityName</label>
            <input class="form-control" id="entityName" type="text" placeholder="@Res.res.entityNameToShow" value="@ViewBag.repoCfgEntity[0].Name"><br />
            <label for="repositoryTypeId">@Res.res.repositoryType</label>
            @Html.DropDownList("repositoryTypeId",
                                 new Type("RepositoryConfig").AllTypes.Select(type =>
                                     new SelectListItem
                                     {
                                         Value = type.Id.ToString(),
                                         Text = type.Code,
                                         Selected = type.Id == ViewBag.repoCfgEntity[0].Type.Id
                                     }),
                                 new
                                 {
                                     @id = "repositoryTypeId",
                                     @class = "form-control select2"
                                 })
        </div>
    }
    @if (EntityExists)
    {
        <div class="form-group col-lg-3">
            <label style="display:block">
                @Html.CheckBoxFor(x => x.LargeData) @Res.res.bigData
            </label>
            <label style="display:block">
                <input type="checkbox" name="isRepository" id="isRepository" value="on" disabled @(IsRepository ? "checked" : "")> @Res.res.isRepository
            </label>
        </div>
        <div class="form-group col-lg-3">
            <label style="display:block">
                @Html.CheckBoxFor(x => x.IsLoggeable) @Res.res.isLoggeable
            </label>
            <label style="display:block">
                <input type="checkbox" name="entityExists" id="entityExists" value="on" disabled @(EntityExists ? "checked" : "")> @Res.res.entityExists
            </label>
        </div>
        <div class="form-group col-lg-3">
            <label style="display:block">
                @Html.CheckBoxFor(x => x.IsTranslatable) @Res.res.isTranslatable
            </label>
            <label style="display:block">
                <input type="checkbox" name="isMultiCompany" id="isMultiCompany" value="on" disabled @((Model.Structs?.Any(x => x.Name == "Company_Id") ?? false) && ((ViewBag.repoCfgEntity)?.Any() ?? false) ? "checked" : "")> @Res.res.isMuliCompany
            </label>
        </div>
    }
</div>
<div class="row">
    @if (EntityExists)
    {
        <div class="col-lg-3" style="margin-top: 2px;">
            <button id="saveEntity" class="btn btn-primary">
                <span class="fa fa-save" aria-hidden="true"></span> @Res.res.save
            </button>
        </div>

        <div class="col-lg-3" style="margin-top: 2px;">
            <a href="@Url.Action("GenerateEntity", "Entity", new { download = true, entity = Model.Name })" target="_blank">
                <div id="downloadEntityCS" class="btn btn-primary">
                    <span class="fa fa-download" aria-hidden="true"></span> @Res.res.downloadEntityCS
                </div>
            </a>
        </div>
    }
    @if (!IsRepository && EntityExists)
    {
        <div class="col-lg-3" style="margin-top: 2px;">
            <button id="generateRepo" class="btn btn-primary">
                <span class="fa fa-repeat" aria-hidden="true"></span> @Res.res.generateRepository
            </button>
        </div>
    }
    <div class="col-lg-3" style="margin-top: 2px;">
        <button id="generateEntity" class="btn btn-primary">
            <span class="fa fa-repeat" aria-hidden="true"></span> @Res.res.generateEntity
        </button>
    </div>
</div>
@if (EntityExists)
{
    <h3>@Res.res.entityStructure</h3>
    <div class="row" style="margin-top: 25px">
        <div id="GridContainer">
            <div class="col-lg-9">
                <table id="EntityGrid"></table>
                <div id="EntityGridPager"></div>
            </div>
        </div>
    </div>
}
