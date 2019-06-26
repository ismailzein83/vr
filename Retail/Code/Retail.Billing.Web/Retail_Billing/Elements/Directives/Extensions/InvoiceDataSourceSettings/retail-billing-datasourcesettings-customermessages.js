"use strict";

app.directive("retailBillingDatasourcesettingsCustomermessages", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_Billing_CustomerMessagesTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_Billing_CustomerMessagesTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerMessagesDataSourceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/Extensions/InvoiceDataSourceSettings/Templates/CustomerMessagesTemplate.html"
        };

        function CustomerMessagesDataSourceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var selectedTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.customerMessagesTypes = UtilsService.getArrayEnum(Retail_Billing_CustomerMessagesTypeEnum);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined && payload.dataSourceEntity != undefined) {
                        $scope.scopeModel.type = UtilsService.getItemByVal($scope.scopeModel.customerMessagesTypes, payload.dataSourceEntity.Type, 'value');;
                    };

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Billing.Business.CustomerMessagesDataSourceSettings, Retail.Billing.Business",
                        Type: $scope.scopeModel.type.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);