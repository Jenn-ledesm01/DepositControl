$(document).ready(function () {
    function checkBcp() {
        if ($('#chkBcp').prop('checked')) {
            $('#divBcp').show();
        } else {
            $('#divBcp').hide();
        }
    }
    checkBcp();
    $('#chkBcp').on('change', checkBcp);

    
    function checkSp() {
        if ($('#chkSp').prop('checked')) {
            $('#divSp').show();
        } else {
            $('#divSp').hide();
        }
    }
    checkSp();
    $('#chkSp').on('change', checkSp);

});