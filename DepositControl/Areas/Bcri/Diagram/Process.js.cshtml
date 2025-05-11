<script type="text/javascript">
    'use strict';

    var processCode, diagram;

    function diagramFitToPage(id, preventScaling) {
        if (id && window) {
            if (ej.isMobile() && ej.isDevice()) {
                var diagram = $("#" + id).ejDiagram("instance");
                diagram.fitToPage("width", "content");
                if (!preventScaling) {
                    var viewPort = ej.datavisualization.Diagram.ScrollUtil._viewPort(diagram, true);
                    var bounds = diagram._getDigramBounds("content");
                    var scale = { x: viewPort.width / bounds.width, y: viewPort.height / bounds.height };
                    $("#" + id).ejDiagram({ height: $("#" + id).height() * Math.min(scale.x, scale.y) });
                    if (window.location.hostname) {
                        var iframe = top.document.getElementById('samplefile');
                        if (iframe) iframe.style.minHeight = $("#" + id).height() + "px";
                    }
                }
            }
        }
    }

    $(document).ready(function () {
        //Events

        processCode = $('#processCode').val();

        $('#processes').change(function () {
            var value = $(this).val();
            var sLink = '@Url.Action("Process", "Diagram")' + '/' + value;
            if (value !== '0') {
                window.location.href = sLink;
            }
        });

        //diagram = $("#myDiagramDiv").ejDiagram("instance");

        if (processCode !== '')
        {
            Bcri.Core.Ajax.send({
                url: '@Url.Action("GetInfo", "Diagram")',
                data: { entity: processCode },
                success: function (data) {
                    var connectors = [],
                        nodes = [];

                    // adds Input/Ouput parent nodes
                    nodes.push({
                        offsetX: 500,
                        offsetY: 90 + ((90 * Math.max(data.processInputConfig.length, data.processOutputConfig.length)) / 2),
                        name: 'processConfig',
                        width: 220,
                        height: 90,
                        type: ej.datavisualization.Diagram.Shapes.Html,
                        templateId: 'templateProcessConfig',
                        pCode: data.processConfig.code,
                        pUrl: '@Url.Action("Index", "Config")' + '/' + data.processConfig.code
                    });

                    for (var i = 0; i < data.processInputConfig.length; i++)
                    {
                        nodes.push({
                            offsetX: 200,
                            offsetY: 90 * (i + 1),
                            name: 'processInputConfig' + i,
                            width: 180,
                            height: 70,
                            type: ej.datavisualization.Diagram.Shapes.Html,
                            templateId: 'templateInputOutputConfig',
                            pCode: data.processInputConfig[i].code,
                        });

                        connectors.push({
                            segments: [{
                                type: "bezier"
                            }],
                            name: 'connectorProcessInputConfigProcess' + i,
                            sourceNode: 'processInputConfig' + i,
                            targetNode: 'processConfig'
                        });
                    }

                    for (var i = 0; i < data.processOutputConfig.length; i++) {
                        nodes.push({
                            offsetX: 800,
                            offsetY: 90 * (i + 1),
                            name: 'processOutputConfig' + i,
                            width: 180,
                            height: 70,
                            type: ej.datavisualization.Diagram.Shapes.Html,
                            templateId: 'templateInputOutputConfig',
                            pCode: data.processOutputConfig[i].code,
                        });

                        connectors.push({
                            segments: [{
                                type: "bezier"
                            }],
                            name: 'connectorProcessOutputConfigProcess' + i,
                            sourceNode: 'processConfig',
                            targetNode: 'processOutputConfig' + i
                        });
                    }

                    $("#myDiagramDiv").ejDiagram({
                        width: "100%",
                        height: "500px",
                        enableAutoScroll: true,
                        allowScrolling: true,
                        defaultSettings: {
                            connector: {
                                segments: [{ type: "orthogonal" }]
                            }
                        },
                        nodes: nodes,
                        connectors: connectors
                    });
                }
            });
        }
    });
</script>
