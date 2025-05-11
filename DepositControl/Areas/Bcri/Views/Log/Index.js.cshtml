<script type="text/javascript">
	'use strict';

	var logActionUrl = '',
		inpActions;

	$(document).ready(function ()
	{
		$('#types').on('change', function (e) {
			Bcri.Core.Ajax.send({
				url: '@Url.Action("GetLogAccion", "Log")',
				data: { typeCode: $(this).val() },
				success: function (data) {
					inpActions.empty();
					for (var i = 0; i < data.length; i++)
					{
						inpActions.append($('<option></option>').prop('value', data[i].id).text(data[i].value));
					}
				}
			});
		}).trigger('change');

		inpActions = $('#actions')
			.on('change', function (e) {
				$('#jqGridProcess')
					.setGridParam({ url: '@Url.Action("GetDynamicDataSet", "Log")?actionId=' + inpActions.val() })
					.trigger("reloadGrid");
			});

		$("#jqGridProcess").jqGrid({
			//url: logActionUrl + '?typeCode=' + $('#types').val(),
			datatype: "json",
			colModel: [
				{ label: 'Id', name: 'dataId', hidden: true },
				{
					label: 'Date', name: 'date', sorttype: 'date',
					formatter: function (cellValue) {
						return moment(cellValue).format("MMM/DD/YYYY").replace(".", "");
					}
				},
				{ label: 'UserName', name: 'user.name' },
				{ label: 'Action To', name: 'actionTo' },
				{ label: 'Data', name: 'data' },
				{ label: 'Old Value', name: 'oldValue' },
				{ label: 'Value', name: 'value' },
			],
			grouping: true,
			groupingView: {
				groupField: ['dataId'],
				groupDataSorted: true
			},
			sortname: "date",
			sortordersortorder: "desc",
			//loadonce: true,
			viewrecords: false,
			height: 200,
			rowNum: 10,
			pager: '#jqGridProcessPager'
		});

		$("#jqGridProcess").navGrid("#jqGridProcessPager",
			// the buttons to appear on the toolbar of the grid
			{ edit: false, add: false, del: false, search: true, refresh: true, view: false, position: "left", cloneToTop: false },
			// options for the Edit Dialog
			{
        		editCaption: "The Edit Dialog",
        		recreateForm: true,
        		checkOnUpdate: true,
        		checkOnSubmit: true,
        		closeAfterEdit: true,
        		errorTextFormat: function (data) {
        			return "Error: " + data.responseText;
        		}
			},
			// options for the Add Dialog
			{
        		closeAfterAdd: true,
        		recreateForm: true,
        		errorTextFormat: function (data) {
        			return "Error: " + data.responseText;
        		},
				/*
        		beforeSubmit: function (postdata, $form) {
        			var dateInputs = $form.find("input[type=date]");
        			for (var i = 0, length = dateInputs.length; i < length; i++) {
        				var input = $(dateInputs[i]);
        				postdata[input.attr("name")] = input.val();
        			}
        			return [true, ""];
        		},
        		afterSubmit: function (response, postdata) {
        			$("#jqGridProcess")
						.setGridParam({ datatype: "json" })
						.setGridParam({ url: Common.dataurl })
						.trigger("reloadGrid");

        			//document.location.reload(true);
        			return [true, ""];
        		}
				*/
			},
			// options for the Delete Dailog
			{
        		errorTextFormat: function (data) {
        			return "Error: " + data.responseText;
        		}
			}
		);
	});
</script>
