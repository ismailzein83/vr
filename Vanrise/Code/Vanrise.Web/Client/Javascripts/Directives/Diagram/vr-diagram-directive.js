(function (app) {

    "use strict";

    vrDiagram.ginject = ['UtilsService', 'MultiTranscludeService'];

    function vrDiagram(UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            scope: {
                onReady: "=",
                isrequired: "=",
                nodes: "=",
                links: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                ctrl.isValid = function () {
                    if (ctrl.model.nodeDataArray.length == 0 && ctrl.isrequired == true)
                        return "You Should add at least one node.";
                    return null;
                };

                /*start constant section*/
                var g = go.GraphObject.make;

                /*end constant section */
                var myDiagram = g(go.Diagram, $($element).find("#myDiagramId")[0], {
                    initialContentAlignment: go.Spot.LeftCenter,
                    contentAlignment: go.Spot.LeftCenter,
                    allowDrop: true,
                    allowMove: true,
                    layout: g(go.LayeredDigraphLayout, { layerSpacing: 100 }),
                    "animationManager.duration": 800,
                    "linkingTool.direction": go.LinkingTool.Either,
                    "ModelChanged": updateAngular,
                    "ChangedSelection": updateSelection,
                    "commandHandler.archetypeGroupData": { isGroup: true, category: "OfNodes" },
                    mouseDrop: function (e) { finishDrop(e, null); },
                    "undoManager.isEnabled": true,
                });
       
                myDiagram.addModelChangedListener(function (evt) {
                    if (!evt.isTransactionFinished) return;
                    var txn = evt.object;
                    if (txn === null) return;
                    var index;
                    var linkindex;
                    txn.changes.each(function (e) {
                        // ignore any kind of change other than adding/removing a node
                        if (e.modelChange !== "nodeDataArray" && e.modelChange !== "linkDataArray") {
                            return;
                        }
                        if (e.modelChange == "linkDataArray") {
                            if (e.change === go.ChangedEvent.Insert) {
                                linkindex = ctrl.model.linkDataArray.indexOf(e.newValue);
                            }
                        }
                        // record node insertions and removals
                        if (e.modelChange == "nodeDataArray") {
                            if (e.change === go.ChangedEvent.Insert) {
                                index = ctrl.model.nodeDataArray.indexOf(e.newValue);

                            }
                        }
                    });



                    if (index != undefined) {
                        myDiagram.animationManager.duration = 1;
                        var olddata = ctrl.model.nodeDataArray[index];
                        updateAngular(evt);
                    }

                    if (linkindex != undefined) {
                        myDiagram.model.setDataProperty(myDiagram.model.linkDataArray[linkindex], "category", "SimpleLink");
                        updateAngular(evt);
                    }
                });

                function finishDrop(e, grp) {
                    if (e.diagram.div.id == "palletId")
                        return;
                    //if (grp.data.Id == 7) {
                    //    e.diagram.currentTool.doCancel();
                    //    return;
                    //}

                    var ok = (grp !== null
                      ? grp.addMembers(grp.diagram.selection, true)
                      : e.diagram.commandHandler.addTopLevelParts(e.diagram.selection, true));
                    if (!ok) e.diagram.currentTool.doCancel();

                    myDiagram.layoutDiagram(true);
                }

                myDiagram.groupTemplateMap.add("", g(go.Group, "Vertical", {
                    computesBoundsAfterDrag: true,
                    mouseDrop: finishDrop,
                    handlesDragDropForMembers: true,
                    selectionObjectName: "DefaultGroup",
                    locationObjectName: "DefaultGroup",
                    resizable: true,
                    resizeObjectName: "DefaultGroup"
                },
                   g(go.Panel, "Auto", {
                       name: "DefaultGroup"
                   },
                        g(go.Shape, "RoundedRectangle",
                        {
                            strokeWidth: 2,
                            name: "SHAPE",
                            portId: "",
                            minSize: new go.Size(250, 75)
                        },
                        new go.Binding("figure", "shape"),
                        new go.Binding("fill", "color").makeTwoWay(),
                        new go.Binding("fromLinkable", "from")),
                        g(go.Placeholder, { padding: 5 }),
                        g(go.TextBlock,
                            {
                                alignment: go.Spot.Center,
                                font: "Bold 12pt Sans-Serif"
                            },
                            new go.Binding("text", "Name")
                        )
                    )
                  ));


                myDiagram.groupTemplateMap.add("SLOT", g(go.Group, "Auto",
                      {
                          background: "transparent",
                          computesBoundsAfterDrag: true,
                          mouseDrop: finishDrop,
                          selectionObjectName: "SlotGroup",
                          locationObjectName: "SlotGroup",
                          resizable: true,
                          resizeObjectName: "SlotGroup",
                          locationSpot: go.Spot.Center,
                          handlesDragDropForMembers: true,
                          padding: 5,
                          layout: g(go.GridLayout,
                         {
                             wrappingColumn: 2,
                             alignment: go.GridLayout.Position,
                             cellSize: new go.Size(1, 1)
                         })
                      },
                      g(go.Shape, "Rectangle",
                          { fill: null, stroke: "#333333", strokeWidth: 2 }
                     ),
                      g(go.Panel, "Vertical",
                       {
                           minSize: new go.Size(50, 100),
                           name: "SlotGroup"
                       }, g(go.Placeholder, { padding: 10, background: "transparent" }))

               ));


                myDiagram.groupTemplateMap.add("IMS", g("Group", "Auto",
                       {
                           background: "transparent",
                           computesBoundsAfterDrag: true,
                           mouseDrop: finishDrop,
                           selectionObjectName: "IMSGroup",
                           locationObjectName: "IMSGroup",
                           resizable: true,
                           groupable: false,
                           toLinkable: false,
                           fromLinkable: false,
                           resizeObjectName: "IMSGroup",
                           locationSpot: go.Spot.Center,
                           handlesDragDropForMembers: true,
                           layout: g(go.GridLayout,
                          {
                              wrappingColumn: 2, alignment: go.GridLayout.Position,
                              cellSize: new go.Size(1, 1), spacing: new go.Size(120, 10)
                          })
                       },
                       g(go.Shape, "Rectangle", { fill: null, stroke: "#33D3E5", strokeWidth: 2 }),
                       g(go.Panel, "Vertical",
                        {
                            minSize: new go.Size(250, 100),
                            name: "IMSGroup"
                        },
                         g(go.Panel, "Horizontal",
                           { stretch: go.GraphObject.Horizontal, background: "#33D3E5" },
                           g(go.TextBlock,
                             {
                                 alignment: go.Spot.Left,
                                 editable: true,
                                 margin: 5,
                                 font: "bold 18px sans-serif",
                                 opacity: 0.75,
                                 stroke: "#404040"
                             },
                             new go.Binding("text", "Name").makeTwoWay())
                         ),
                         g(go.Placeholder,
                           { padding: 10, alignment: go.Spot.TopLeft })

                           ),
                        g(go.Picture,
	                    {
	                        name: "Picture",
	                        desiredSize: new go.Size(50, 50),
	                        margin: new go.Margin(25, 8, 6, 10),
	                        imageAlignment: go.Spot.BottomCenter,
	                        source: "/Client/Images/mini-icons/DiagramSize/ims.png"
	                    })
                ));


                myDiagram.groupTemplateMap.add("OLT", g(go.Group, "Auto",
                  {
                      background: "transparent",
                      ungroupable: true,
                      computesBoundsAfterDrag: true,
                      locationSpot: go.Spot.Center,
                      mouseDrop: finishDrop,
                      selectionObjectName: "OLTGroup",
                      locationObjectName: "OLTGroup",
                      resizable: true,
                      resizeObjectName: "OLTGroup",
                      handlesDragDropForMembers: true,
                      layout: g(go.GridLayout,
                        {
                            wrappingColumn: 2, alignment: go.GridLayout.Position,
                            cellSize: new go.Size(1, 1), spacing: new go.Size(100, 10)
                        })
                  },
                  g(go.Shape, "Rectangle", { fill: null, stroke: "#FFDD33", strokeWidth: 2 }),
                  g(go.Panel, "Vertical",
                    {
                        minSize: new go.Size(250, 100),
                        //maxSize: new go.Size(280, 200),
                        name: "OLTGroup"
                    },
                    g(go.Panel, "Horizontal",
                      { stretch: go.GraphObject.Horizontal, background: "#FFDD33" },
                      g(go.TextBlock,
                        {
                            alignment: go.Spot.Left,
                            editable: true,
                            margin: 5,
                            font: "bold 16px sans-serif",
                            opacity: 0.75,
                            stroke: "#404040"
                        },
                        new go.Binding("text", "Name").makeTwoWay())
                    ),
                    g(go.Placeholder,
                      { padding: 10, alignment: go.Spot.TopLeft })

                  ),
                  g(go.Picture,
	                {
	                    name: "Picture",
	                    desiredSize: new go.Size(50, 50),
	                    margin: new go.Margin(25, 8, 6, 10),
	                    imageAlignment: go.Spot.BottomCenter,
	                    source: "/Client/Images/mini-icons/DiagramSize/olt.png",
	                })
                ));

                myDiagram.nodeTemplateMap.add("Port",
                 g(go.Node, "Auto",
                    {
                        mouseDrop: function (e, nod) { finishDrop(e, nod.containingGroup); }
                    },
                    g(go.Shape, "Circle",
                    {
                        fill: "#4d8a45",
                        stroke: "#4d8a45",
                        strokeWidth: 1,
                        toLinkable: true,
                        fromLinkable: true,
                        toLinkableSelfNode: true,
                        fromLinkableSelfNode: true,
                        portId: "",
                        minSize: new go.Size(15, 15),
                        maxSize: new go.Size(15, 15)
                    },
                    new go.Binding("fill", "color")),
                    g(go.Shape, "Circle",
                    {
                        fill: "#ffffff",
                        minSize: new go.Size(10, 10),
                        maxSize: new go.Size(10, 10)
                    }),
                    g(go.Shape, "Circle",
                    {
                        fill: "#333333",
                        minSize: new go.Size(2, 2),
                        maxSize: new go.Size(2, 2)
                    })
                ));



                myDiagram.nodeTemplateMap.add("PortRj",
                 g(go.Node, "Auto",
                    {
                        mouseDrop: function (e, nod) { finishDrop(e, nod.containingGroup); }
                    },
                    g(go.Shape, "Rectangle",
                    {
                        fill: "#FFFFFF",
                        stroke: "#4d8a45",
                        strokeWidth: 1,
                        toLinkable: true,
                        fromLinkable: true,
                        portId: "",
                        minSize: new go.Size(15, 15),
                        maxSize: new go.Size(15, 15)
                    },
                    new go.Binding("fill", "color")),
                    g(go.Shape, "RoundedRectangle",
                    {
                        fill: "#ffffff",
                        minSize: new go.Size(7, 7),
                        maxSize: new go.Size(10, 10)
                    })
                ));


                myDiagram.linkTemplateMap.add("", g(go.Link,
                    {
                        adjusting: go.Link.Stretch,
                        relinkableFrom: true,
                        relinkableTo: true,
                        toShortLength: 3
                    },
                    new go.Binding("points").makeTwoWay(),
                    g(go.Shape, { strokeWidth: 1.5 }),
                    g(go.TextBlock,
                    {
                        textAlign: "center",
                        font: "bold 14px sans-serif",
                        stroke: "#1967B3",
                        segmentIndex: 0,
                        segmentOffset: new go.Point(NaN, NaN),
                        segmentOrientation: go.Link.OrientUpright
                    },
                    new go.Binding("text", "text")),
                    g(go.TextBlock,
                    {
                        textAlign: "center",
                        font: "bold 14px sans-serif",
                        stroke: "#1967B3",
                        segmentIndex: -1,
                        segmentOffset: new go.Point(NaN, NaN),
                        segmentOrientation: go.Link.OrientUpright
                    },
                    new go.Binding("text", "totext")
                 )));

                myDiagram.linkTemplateMap.add("SimpleLink", g(go.Link,
                    {
                        adjusting: go.Link.Stretch,
                        relinkableFrom: true,
                        relinkableTo: true,
                        toShortLength: 3
                    },
                    new go.Binding("points").makeTwoWay(),
                    g(go.Shape, { strokeWidth: 1.5, stroke: "red" }),
                    g(go.Shape, { fromArrow: "BackwardV", stroke: "green", strokeWidth: 0.5 }),
                    g(go.Shape, { toArrow: "SidewaysV", stroke: "green", strokeWidth: 0.5 })
                ));


                myDiagram.addDiagramListener("ObjectSingleClicked", function (e) {
                    var part = e.subject.part;
                    if (part instanceof go.Node) {
                        console.log(part.data + "//  data clicked");
                    }
                    if (part instanceof go.Link) {
                        console.log(part.data + "//  link clicked");
                    }
                    $scope.$apply();
                });
                function updateAngular(e) {
                    setTimeout(function () {
                        if (e.isTransactionFinished) {
                            $scope.$apply();
                        }
                    });
                }

                function updateSelection(e) {
                    ctrl.selectednode = null;
                    var it = myDiagram.selection.iterator;
                    while (it.next()) {
                        var selnode = it.value;
                        if (selnode instanceof go.Node && selnode.data !== null) {
                            ctrl.selectednode = selnode.data;
                            break;
                        }
                    }
                    setTimeout(function () {
                        if (e.isTransactionFinished) {
                            $scope.$apply();
                        }
                    });
                }

                $scope.$watch("myDiagramCtrl.model", function (newmodel) {
                    var oldmodel = myDiagram.model;
                    if (oldmodel !== newmodel) {
                        myDiagram.removeDiagramListener("ChangedSelection", updateSelection);
                        myDiagram.model = newmodel;
                        myDiagram.addDiagramListener("ChangedSelection", updateSelection);
                    }
                });


                ctrl.model = new go.GraphLinksModel();
                ctrl.model.nodeGroupKeyProperty = "Parent";

                ctrl.model.nodeKeyProperty = "Id";
                ctrl.model.linkKeyProperty = "key";
                if (ctrl.nodes && ctrl.nodes.length > 0)
                    ctrl.model.nodeDataArray = ctrl.nodes;
                else
                    ctrl.model.nodeDataArray = [];
                if (ctrl.links && ctrl.links.length > 0)
                    ctrl.model.linkDataArray = ctrl.links;
                else
                    ctrl.model.linkDataArray = [];

                ctrl.selectednode = null;
                myDiagram.model = ctrl.model;
                myDiagram.layoutDiagram(true);

                var api = {};

                api.load = function (data) {
                    if (data != undefined) {
                        if (data.nodeDataArray != undefined && data.nodeDataArray.length > 0)
                            ctrl.model.nodeDataArray = data.nodeDataArray;
                        if (data.linkDataArray != undefined && data.linkDataArray.length > 0)
                            ctrl.model.linkDataArray = data.linkDataArray;
                    }
                };

                api.addNodeData = function (node) {
                    ctrl.model.addNodeData(node);
                };

                api.removeNodeData = function (node) {
                    var group = myDiagram.findNodeForKey(7);
                    myDiagram.select(group);
                    myDiagram.commandHandler.deleteSelection();

                };

                api.addLinkData = function (link) {                  
                    myDiagram.model.addLinkData(link);
                };

                api.getLinkData = function (key) {
                    var link = myDiagram.model.findLinkDataForKey(key);
                    return link;
                };

                api.setReadOnly = function (isreadOnly) {
                    myDiagram.isReadOnly = isreadOnly;
                };

                if (ctrl.onReady && typeof (ctrl.onReady) == 'function')
                    ctrl.onReady(api);

            },
            controllerAs: 'myDiagramCtrl',
            bindToController: true,
            template: function (element, attrs) {

                var template = '<vr-validator validate="myDiagramCtrl.isValid()"><div style="display: inline-block; vertical-align: top; width:100%">'
                                   + '<div id="myDiagramId"  style="display: inline-block;width:calc(100%);border: solid 1px black; height: 500px"></div>'
                               + '</div></vr-validator>';


                return template;
            }

        };

    }

    app.directive('vrDiagram', vrDiagram);

})(app);