"use strict";
app.directive("retailBeRecurringchargetypeSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService","Retail_BE_RecurringChargeTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_RecurringChargeTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RecurringChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Extensions/RecurringChargeTypeSettings/Templates/RecurringChargeTypeSettingsTemplate.html"

        };

        function RecurringChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.recurringChargeTypes = UtilsService.getArrayEnum(Retail_BE_RecurringChargeTypeEnum);

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];

                    if (payload != undefined && payload.settings != undefined) {
                        $scope.scopeModel.selectedRecurringChargeType = UtilsService.getItemByVal($scope.scopeModel.recurringChargeTypes, payload.settings.Type, 'value');
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.RecurringChargeTypeSettings, Retail.BusinessEntity.Business",
                        Type: $scope.scopeModel.selectedRecurringChargeType != undefined ? $scope.scopeModel.selectedRecurringChargeType.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);