"use strict";

app.directive("vrInvoicetypeOpenrdlcreportDatasourcesettingsRdlcitems", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RDLCItemsDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/RDLCReport/RDLCReportDataSourceSettings/MainExtensions/Templates/RDLCItemsDataSourceSettings.html"

        };

        function RDLCItemsDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.listItems = [];
                ctrl.addItem = function()
                {
                    if(ctrl.itemsetName != undefined)
                    {
                        ctrl.listItems.push(ctrl.itemsetName);
                    }
                }
                ctrl.disableAddButton = true;
                ctrl.disableAddItem = function()
                {
                    if (ctrl.itemsetName == undefined || ctrl.itemsetName == "")
                    {
                        ctrl.disableAddButton = true;
                        return null;
                    }
                    if (UtilsService.contains(ctrl.listItems, ctrl.itemsetName))
                    {
                        ctrl.disableAddButton = true;
                        return "Same name exist.";
                    }
                    ctrl.disableAddButton = false;
                    return null;
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.listItems.length = 0;
                    if (payload != undefined) {
                        if(payload.ItemSetNames != undefined)
                        {
                            for(var i=0;i<payload.ItemSetNames.length;i++)
                            {
                                var itemSetName = payload.ItemSetNames[i];
                                ctrl.listItems.push(itemSetName);
                            }
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    console.log(ctrl.listItems);
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.RDLCItemsDataSourceSettings ,Vanrise.Invoice.MainExtensions",
                        ItemSetNames : ctrl.listItems
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);