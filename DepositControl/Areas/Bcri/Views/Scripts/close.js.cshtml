<script type="text/javascript">

    var validNavigation = false;

    function endSession() {

        $.ajax({
            url: baseUrl + '/Home/closeLogOut',
            type: 'POST',
            data: {},
            dataType: 'json',
            error: function (ex) { alert(ex.responseText) },
            success: function (data) {
            }
        });

    }
    function wireUpEvents() {
        window.onbeforeunload = function () {
            if (!validNavigation) {
                endSession();
            }
			validNavigation = false;
        }

        $("#selectProcess").on("change", function (e) {
            validNavigation = true;
        });

        $('body').on('click', '[data-action]', function () {
            validNavigation = true;
        });

        $("button[title='Descargar PDF']").click(function () {
            validNavigation = true;
        });

        $("button[title='Descargar TXT']").click(function () {
            validNavigation = true;
        });

        $("button[title='Descargar Excel']").click(function () {
            validNavigation = true;
        });
		$(document).on('click', '.ui-pg-button', function() {
			validNavigation = true;
		});

        //  F5
        $(document).bind('keydown', function (e) {
            if (e.keyCode == 116) {
                validNavigation = true;
            }
        });

        //  all links in the page
        $("a").bind("click", function () {
            validNavigation = true;
        });
		 $("a[data-toggle='tab']").bind("click", function () {
            validNavigation = false;
        });
		//submit for all forms in the page
        $("form").bind("submit", function () {
            validNavigation = true;
        });

        // click for all inputs in the page
        $("input[type=submit]").bind("click", function () {
            validNavigation = true;
        });
    }


    $(document).ready(function () {
        if (!Bcri.Settings.allowUrlNavigation) {
            wireUpEvents();
        }
    });

</script>



