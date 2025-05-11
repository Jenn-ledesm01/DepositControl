$(document).ready(function() {

        //Global values
        var page = 1;
        var Common = $("#DocumentSearchIndexJsData").data();
        if (Common === undefined) return;
         
            Dropzone.options.dropzone = {
                maxFiles: 20,
                init: function() {
                    this.on("maxfilesexceeded", function(data) {
                        var res = eval('(' + data.xhr.responseText + ')');

                    }); 
                    this.on("complete", function(data) {
                        if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                        location.reload();
                        }
                        //var res = eval('(' + data.xhr.responseText + ')');
                        var res = JSON.parse(data.xhr.responseText);
                         
                    });

                    this.on("addedfile", function(file) {

                        // Create the remove button
                        var removeButton = Dropzone.createElement("<button>Remove file</button>");

                        // Capture the Dropzone instance as closure.
                        var _this = this;

                        // Listen to the click event
                        removeButton.addEventListener("click", function(e) {
                            // Make sure the button click doesn't submit the form:
                            e.preventDefault();
                            e.stopPropagation();
                            // Remove the file preview.
                            _this.removeFile(file);
                            // If you want to the delete the file on the server as well,
                            // you can do the AJAX request here.
                        });
                    });
                }
            }; 
//Si el file no esta visible
    $('#myModal').on('hidden.bs.modal', function(e) {
        Dropzone.forElement(".dropzone").removeAllFiles(true);
    });


    $(function initInfiniteScroll() {
        $(document).bind('scroll', function() {
            if ((page != -1) && $(window).scrollTop() >= ($(document).height() - $(window).height() - 20)) {
                page++;
                NextPage();
            }
        });
    });


    function NextPage() { 
        var $form = $("#FilterForm");
        $form.find('#Page').val(page);
        $.ajax({    
            url: Common.getpage,
            data: $form.serialize(),
            type: "POST",
            success: function(html) {  
                var documents = $('<div />').append(html).find('.Document').html();
                if (page == 1) {
                    $(".chat").HTML(documents);
                } 
                else {
                    $(".chat").append(documents);
                }
                if (html.length < 10) //si es menor a 10 caracteres esta vacio
                    page = -1; // si ya no tengo elementos pongo el page en -1 para que no siga pidiendo 
            },
            error: function(xhr, status, error) {
                alert(xhr.responseText);
            }
        });
    };

});