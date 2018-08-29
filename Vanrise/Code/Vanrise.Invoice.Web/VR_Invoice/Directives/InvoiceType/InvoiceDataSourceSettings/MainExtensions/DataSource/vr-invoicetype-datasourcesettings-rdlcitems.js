"use strict";

app.directive("vrInvoicetypeDatasourcesettingsRdlcitems", ["UtilsService", "VRNotificationService", "VRUIUtilsService",'VR_Invoice_ItemSetNameCompareOperatorEnum',
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_ItemSetNameCompareOperatorEnum) {

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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceDataSourceSettings/MainExtensions/DataSource/Templates/ItemsDataSourceSettings.html"

        };

        function RDLCItemsDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                ctrl.listItems = [];
                $scope.scopeModel.itemSetNamesCompareOperators = UtilsService.getArrayEnum(VR_Invoice_ItemSetNameCompareOperatorEnum);

                ctrl.addItem = function () {
                    if (ctrl.itemsetName != undefined) {
                        ctrl.listItems.push(ctrl.itemsetName);
                        ctrl.itemsetName = undefined;
                    }
                };
                ctrl.disableAddButton = true;
                ctrl.disableAddItem = function () {
                    if (ctrl.itemsetName == undefined || ctrl.itemsetName == "") {
                        ctrl.disableAddButton = true;
                        return null;
                    }
                    if (UtilsService.contains(ctrl.listItems, ctrl.itemsetName)) {
                        ctrl.disableAddButton = true;
                        return "Same name exists.";
                    }
                    ctrl.disableAddButton = false;
                    return null;
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.listItems.length = 0;
                    if (payload != undefined) {
                        if (payload.dataSourceEntity != undefined) {
                            $scope.scopeModel.selectedItemSetNameCompareOperator = UtilsService.getItemByVal($scope.scopeModel.itemSetNamesCompareOperators, payload.dataSourceEntity.CompareOperator, "value");
                            if (payload.dataSourceEntity.ItemSetNames != undefined)
                            {
                                for (var i = 0; i < payload.dataSourceEntity.ItemSetNames.length; i++) {
                                    var itemSetName = payload.dataSourceEntity.ItemSetNames[i];
                                    ctrl.listItems.push(itemSetName);
                                }
                            }
                            $scope.scopeModel.orderByField = payload.dataSourceEntity.OrderByField;
                            $scope.scopeModel.isDescending = payload.dataSourceEntity.IsDescending;

                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    return {
                        $type: "Vanrise.Invoice.MainExtensions.ItemsDataSourceSettings ,Vanrise.Invoice.MainExtensions",
                        ItemSetNames: ctrl.listItems,
                        CompareOperator: $scope.scopeModel.selectedItemSetNameCompareOperator != undefined ? $scope.scopeModel.selectedItemSetNameCompareOperator.value : undefined,
                        OrderByField: $scope.scopeModel.orderByField,
                        IsDescending: $scope.scopeModel.isDescending
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);