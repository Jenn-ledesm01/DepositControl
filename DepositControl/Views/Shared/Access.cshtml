@using DNF.Security.Bussines
@model List<Access>
@{
    if (!Model.Any()) { return; }

    var levelClass = Model[0].TreeLevel.ToString();
    switch (Model[0].TreeLevel)
    {
        case 1:
            levelClass = "second";
            break;
        case 2:
            levelClass = "third";
            break;
    }
}
@foreach (var access in Model)
{
    <li>
        <a href="@(string.IsNullOrEmpty(access.Url) ? "#" : Url.Content(access.Url) )" 
           data-navigation="@Url.Action("Navigation","Security", new {access.Id})"  data-accessId="@access.Id">
            @if (!string.IsNullOrEmpty(access.Icon))
            {
                <i class="@access.Icon fa-fw"></i>// lista de iconos completa aca: http://ironsummitmedia.github.io/startbootstrap-sb-admin-2/pages/icons.html
            }
            <span>@access.Name</span>
            @if (access.Accesss.Any())
            {
                <span class="fa arrow"></span>
            }
        </a>
        @if (access.Accesss.Any())
        {
            <ul class="nav nav-@levelClass-level  collapse" >
                @Html.Partial("Access", access.Accesss)
            </ul>
        }
    </li>
}


