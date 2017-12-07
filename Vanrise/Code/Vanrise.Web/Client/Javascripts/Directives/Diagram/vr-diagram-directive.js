(function (app) {

    "use strict";

    vrDiagram.ginject = ['UtilsService', 'MultiTranscludeService'];

    function vrDiagram(UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            scope: {

            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;


            },
            link: function (scope, element, attrs, ctrl) {

                ctrl.isValid = function () {
                    if (ctrl.model.nodeDataArray.length == 0)
                        return "You Should add at least one node.";
                    return null;
                };


                var g = go.GraphObject.make;  // for conciseness in defining templates
                //var baseele = element[0];

                var myDiagram = g(go.Diagram, $(element).find("#myDiagramId")[0], {// must name or refer to the DIV HTML element
                    initialContentAlignment: go.Spot.Center,
                    allowDrop: true,
                    allowMove: true,
                    initialAutoScale: go.Diagram.Uniform,
                    "animationManager.duration": 800,
                    // "linkingTool.direction": go.LinkingTool.ForwardsOnly,
                    "ModelChanged": updateAngular,
                    "ChangedSelection": updateSelection,
                    //  layout: g(go.LayeredDigraphLayout, { isInitial: false, isOngoing: false, layerSpacing: 50 }),
                    "undoManager.isEnabled": true,
                    isReadOnly:true
                });

                myDiagram.addModelChangedListener(function (evt) {
                    // ignore unimportant Transaction events
                    if (!evt.isTransactionFinished) return;
                    var txn = evt.object;  // a Transaction
                    if (txn === null) return;
                    // iterate over all of the actual ChangedEvents of the Transaction
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
                        // console.log(ctrl.model.nodeDataArray)

                        myDiagram.model.setDataProperty(myDiagram.model.nodeDataArray[index], "Name", ctrl.model.nodeDataArray[index].Name + " Ins " + (index + 1));
                        myDiagram.model.setKeyForNodeData(myDiagram.model.nodeDataArray[index], ctrl.model.nodeDataArray[index].id + "-" + (index + 1));


                        myDiagram.model.setDataProperty(myDiagram.model.nodeDataArray[index], "from", true);
                        var olddata = ctrl.model.nodeDataArray[index];


                        // myDiagram.model.addNodeData({ id: 1, from: true, loc: olddata.loc });

                        // myDiagram.model.removeNodeData(ctrl.model.nodeDataArray[index]);

                        // //setTimeout(function () {
                        //     myDiagram.animationManager.duration = 800;

                        //// }, 1)
                        //console.log(ctrl.model.nodeDataArray)
                        //myDiagram.model.nodeDataArray[index] = {                            
                        //    id: 1,
                        //    from:true
                        //};
                        //console.log(ctrl.model.nodeDataArray)

                        //ctrl.model.nodeDataArray[index] = {
                        //    id: 1,
                        //    from:true
                        //};

                        updateAngular(evt);
                    }

                    if (linkindex != undefined) {
                        myDiagram.model.setDataProperty(myDiagram.model.linkDataArray[linkindex], "text", " Tras " + (linkindex + 1));
                        updateAngular(evt);
                    }
                });

                var yellowgrad = g(go.Brush, "Linear", { 0: "rgb(254, 201, 0)", 1: "rgb(254, 162, 0)" });
                var greengrad = g(go.Brush, "Linear", { 0: "#98FB98", 1: "#9ACD32" });
                var bluegrad = g(go.Brush, "Linear", { 0: "#B0E0E6", 1: "#87CEEB" });
                var redgrad = g(go.Brush, "Linear", { 0: "#C45245", 1: "#871E1B" });
                var whitegrad = g(go.Brush, "Linear", { 0: "#F0F8FF", 1: "#E6E6FA" });

                var bigfont = "bold 13pt Helvetica, Arial, sans-serif";
                var smallfont = "bold 11pt Helvetica, Arial, sans-serif";

                // Common text styling
                function textStyle() {
                    return {
                        margin: 6,
                        wrap: go.TextBlock.WrapFit,
                        textAlign: "left",
                        editable: true,
                        font: smallfont
                    };
                }


                // when the document is modified, add a "*" to the title and enable the "Save" button




                myDiagram.nodeTemplateMap.add("",
                    g(go.Node, "Auto",
                    { locationSpot: go.Spot.Center },
                    new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),
                    g(go.Shape, "Rectangle",
                        { strokeWidth: 2, name: "SHAPE", toLinkable: true, portId: "", minSize: new go.Size(250, 75), maxSize: new go.Size(280, 200) },
                        new go.Binding("figure", "shape"),
                        new go.Binding("fill", "color").makeTwoWay(),
                        new go.Binding("fromLinkable", "from")

                     ),
                      g(go.Panel, "Horizontal",
                            g(go.Picture,
                            {
                                name: "Picture",
                                desiredSize: new go.Size(15, 15),
                                margin: new go.Margin(6, 8, 6, 10),
                            },
                            new go.Binding("source", "icon")),
                            g(go.TextBlock, "page", textStyle(),
                            {
                                stroke: "whitesmoke",
                                wrap: go.TextBlock.WrapFit, minSize: new go.Size(100, 20)
                            },
                            new go.Binding("text", "Name").makeTwoWay())
                       )
                    ));
                myDiagram.linkTemplate =
                  g(go.Link,  // the whole link panel
                    {
                        curve: go.Link.Bezier, adjusting: go.Link.Stretch,
                        reshapable: true, relinkableFrom: true, relinkableTo: true,
                        toShortLength: 3
                    },
                    new go.Binding("points").makeTwoWay(),
                    new go.Binding("curviness"),
                    g(go.Shape,  // the link shape
                      { strokeWidth: 1.5 }),
                    g(go.Shape,  // the arrowhead
                      { toArrow: "standard", stroke: null }),
                    g(go.Panel, "Auto",
                      g(go.Shape,  // the label background, which becomes transparent around the edges
                        {
                            fill: g(go.Brush, "Radial",
                                    { 0: "rgb(240, 240, 240)", 0.3: "rgb(240, 240, 240)", 1: "rgba(240, 240, 240, 0)" }),
                            stroke: null
                        }),
                      g(go.TextBlock, "transition",  // the label text
                        {
                            textAlign: "center",
                            font: "9pt helvetica, arial, sans-serif",
                            margin: 4,
                            editable: true  // enable in-place editing
                        },
                        // editing the text automatically updates the model data
                        new go.Binding("text").makeTwoWay())
                    ));




                var palette = g(go.Palette, $(element).find("#palletId")[0],  // create a new Palette in the HTML DIV element "palette"
                    {
                        // share the template map with the Palette
                        nodeTemplateMap: myDiagram.nodeTemplateMap,
                        initialContentAlignment: go.Spot.Center,
                        autoScale: go.Diagram.Uniform  // everything always fits in viewport
                    });

                palette.model.nodeDataArray = [
                    { id: 1, category: "Step", shape: "Rectangle", icon: "Client/Images/inherited.png", color: "#00adef", Name: "Relation DS", title: { value: "title 12", isrequired: true } },
                    { id: 2, category: "Step", shape: "Pentagon", icon: "/Client/Images/support.png", color: "#C45245", Name: "Relation DS", title: { value: "title 12", isrequired: true } },
                    { id: 3, category: "Start", shape: "Ellipse", icon: "/Client/Images/support.png", color: "#00adef", Name: "Relation DS", title: { value: "title 12", isrequired: true } },
                    { id: 4, category: "Start", shape: "Database", icon: "/Client/Images/support.png", color: "#C45245", Name: "Relation DS", title: { value: "title 12", isrequired: true } }
                ];
                myDiagram.addDiagramListener("ObjectSingleClicked", function (e) {
                    var part = e.subject.part;

                    if (part instanceof go.Node) {
                        console.log(part.data + "//  data clicked");

                    }
                    if (part instanceof go.Link) {
                        console.log(part.data + "//  link clicked");
                    }

                    scope.$apply();
                });
                function updateAngular(e) {
                    setTimeout(function () {
                        if (e.isTransactionFinished) {
                            scope.$apply();
                        }
                    });
                }

                function updateSelection(e) {
                    myDiagram.model.selectedNodeData = null;
                    var it = myDiagram.selection.iterator;
                    while (it.next()) {
                        var selnode = it.value;
                        // ignore a selected link or a deleted node
                        if (selnode instanceof go.Node && selnode.data !== null) {
                            myDiagram.model.selectedNodeData = selnode.data;
                            break;
                        }
                    }
                    scope.$apply();
                }

                // notice when the value of "model" changes: update the Diagram.model
                scope.$watch("myDiagramCtrl.model", function (newmodel) {
                    var oldmodel = myDiagram.model;
                    if (oldmodel !== newmodel) {
                        myDiagram.removeDiagramListener("ChangedSelection", updateSelection);
                        myDiagram.model = newmodel;
                        myDiagram.addDiagramListener("ChangedSelection", updateSelection);
                    }
                });

                scope.updateValue = function () {
                    myDiagram.model.setDataProperty(ctrl.model.nodeDataArray[0], "from", !ctrl.model.nodeDataArray[0].from);
                    var node = myDiagram.findNodeForData(ctrl.model.nodeDataArray[0]);
                    if (ctrl.model.nodeDataArray[0].from == false) {
                        var it = node.findLinksOutOf();
                        while (it.next()) {

                            var item = it.value;
                            myDiagram.model.removeLinkData(item.data);
                        }
                    }
                    myDiagram.model.setDataProperty(ctrl.model.nodeDataArray[0], "nbIns", 4);

                };

                ctrl.model = new go.GraphLinksModel([]);
                myDiagram.model = ctrl.model;
                myDiagram.layoutDiagram(true);

            },
            controllerAs: 'myDiagramCtrl',
            bindToController: true,
            template: function (element, attrs) {

                var template = '<vr-validator validate="myDiagramCtrl.isValid()"><div style="display: inline-block; vertical-align: top; width:100%">'
                                   + '<div id="palletId"  style="display: inline-block;width:300px;border: solid 1px black; height: 720px"></div>'
                                   + '<div id="myDiagramId"  style="display: inline-block;width:calc(100% - 300px);border: solid 1px black; height: 720px"></div>'
                               + '</div></vr-validator>{{myDiagramCtrl.model}}';


                return template;
            }

        };

    }

    app.directive('vrDiagram', vrDiagram);

})(app);