$(document).ready(function() {
    initCommets();
});

function initCommets(){
    var Common = $("#CommentsJsData").data();
    if (Common === undefined) return; //no inplementa comentarios

    function refresh() {
        $.ajax({
            type: 'GET',
            data: $('#chatContainer').data(),
            url: Common.refresh,
            success: function (data) {
                $('#chatContainer').html(data);
                $("em").html(function () {
                    var text = moment($(this).text(), "DD/MM/YYYY hh:mm:ss").calendar();
                    //devolver directamente la fecha en la que se escribio
                    //caso contrario
                    return text;
                });
                $('#chatContainer').scrollTop($('#chatContainer')[0].scrollHeight);
            }
        });
    }
    refresh();
    /**
     * @returns {a succes if the the message was send correctly} 
     */
    function sendMessage() {
        //Desabilitar boton 
        
        //desab cuadro de textOverflow
        if ($('#message').val() === "") {
            $('#message').focus();
            return;
        }


        $("#btn-chat").prop('disabled', true);  
        $("#message").prop('disabled', true);
        $.ajax({
            type: "POST",
            url: Common.dataurl,
            data: {
                message: $("#message").val(),
                processId: $('#chatContainer').data('processid')
            },
            success: function(data) {
                if (data.ok) {
                    $('#message').val('');
                    $("#btn-chat").prop('disabled', false);  
                    $("#message").prop('disabled', false);
                    refresh();
                }
                else if (data.error) {
                    $('#alterComments').showAlert(data.error, 'error');
                }
            }
        });
    }
    /**
     * Sends message to the DB if enter or click
     */
    $('#refresh').on('click', function(e) {
        e.preventDefault();
        refresh();
    });
    $("#btn-chat").click(function (e) {
        e.preventDefault();
        sendMessage(); 
    });
    $('#message').keydown(function (e) {
        if (e.keyCode == 13) {
            sendMessage();
            e.preventDefault();
        }
    }); 

};