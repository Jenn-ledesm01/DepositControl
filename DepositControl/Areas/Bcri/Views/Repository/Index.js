$(function () {
    var Common = $("#RepositoryJsData").data();
    $('#RepositoryConfigId').on('change', function(e) {
        $('#RepositoryGridContainer').load(Common.gridurl, { repositoryConfigId: $(this).val() }, function() {
            initRepositoryGrid('#RepositoryGridContainer');
        });
    });
});