﻿(function (app) {
    "use strict";
    ModuleTypeConditionDirective.inject = ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_ModuleTypeEnum"];
    function ModuleTypeConditionDirective(UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_ModuleTypeEnum) {

        return {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new ModuleTypeCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/InvoiceType/InvoiceGridActionFilterCondition/Templates/ModuleTypeConditionTemplate.html"

        };

        function ModuleTypeCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.moduleTypes = UtilsService.getArrayEnum(Retail_BE_ModuleTypeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceFilterConditionEntity;
                    if (payload != undefined && payload.invoiceFilterConditionEntity!=undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        $scope.scopeModel.selectedModuleType = UtilsService.getItemByVal($scope.scopeModel.moduleTypes, invoiceFilterConditionEntity.ModuleType, 'value');
                    }
                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.ModuleTypeCondition, Retail.BusinessEntity.Business",
                        ModuleType: $scope.scopeModel.selectedModuleType.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
    app.directive("retailInvoicetypeInvoicefilterconditionModuletype", ModuleTypeConditionDirective);
})(app);