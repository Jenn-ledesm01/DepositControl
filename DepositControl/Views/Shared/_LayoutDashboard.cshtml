@using DNF.Security.Bussines
@model Bcri.Core.Bussines.ProcessConfig
@{
    ViewBag.Title = Model.Name;
    ViewBag.processCode = Model.Code;
    Layout = "_LayoutBcri.cshtml";
}


<div class="container-fluid">
    <div class="row">
        <h1 id="ProcessTitle" class="page-header">@Model.Type.Name - @Model.Name</h1>
    </div>
    <div class="panel-body">
        <!-- Nav tabs -->
        <ul id="ProcessDashboardTab" class="nav nav-tabs ">
            <li>
                <a href="@Url.Action("Processes","Process",new {processCode = Model.Code})" data-toggle="tab">Imports</a>
            </li>
            @*<li>de aca para abajo tiene que tener permiso para cada cosa
                    <a href="#parametrics" data-toggle="tab">Parametria</a>
                </li>*@
            @if (Current.User.HasAccess(Model.AccessCode + "Configuration"))
            {
                <li>
                    <a href="@Url.Action("index", "Config", new {processCode = Model.Code})" data-toggle="tab">Configuration</a>
                </li>
            }
            @*<li>
                <a href="#documentation" data-toggle="tab">Documentacion</a>
                </li>*@
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            @RenderBody()
        </div>
    </div>
</div>

@section scripts {
    @RenderSection("scripts", required:false)
    <script>
        $(function() {
            var url = window.location;
            $('#ProcessDashboardTab a').on('click', function(e) { //combierte los labs en links comunes
                e.preventDefault();
                window.location = $(this).attr('href');
            }).filter(function() { // si la url actual corresponde a algun tab lo pone como activo
                return this.href === url || url.href.indexOf(this.href) === 0;
            }).parent().addClass('active').unbind('click');
        });
    </script>
}
