@using Bcri.Core.Bussines
@model  ProcessConfig

@{
    var parametrics = Model.InputConfigs.Where(x => x.RepositoryConfig.Type.Code == "Parametric").Select(x => x.RepositoryConfig).ToList();
}
<div class="panel-body col-lg-12">
    <!-- Nav tabs -->
    <ul class="nav nav-pills">
        @foreach (RepositoryConfig parametric in parametrics)
            {
            <li class="@(parametrics.IndexOf(parametric) == 0 ? "active" : "")">
                <a href="#@parametric.Code-Output" data-toggle="tab">@parametric.Name</a>
            </li>
        }

    </ul>
    <!-- Tab panes -->
    <div class="tab-content">
        @foreach (RepositoryConfig parametric in parametrics)
            {
            <div class="panel-body tab-pane fade @(parametrics.IndexOf(parametric) == 0 ? "in active" : "")" id="@parametric.Code-Output">
                @*<h3>@output.Config.Name</h3>*@

                @Html.Partial("RepositoryDataGrid", parametric.Current)
            </div>
        }
    </div>
</div>
<!-- /.panel-body -->

