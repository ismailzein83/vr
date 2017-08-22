"use strict";

app.directive("whsBeFinancialaccountdefinitionsettingsSupplierpostpaid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierPostpaid($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/FinancialAccount/MainExtensions/SupplierPostpaid/Templates/SupplierPostpaidDefinitionSettings.html"

        };

        function SupplierPostpaid($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
           
            function initializeController() {
                $scope.scopeModel = {};

             
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettingsEntity;
                    if (payload != undefined) {
                        extendedSettingsEntity = payload.extendedSettingsEntity;
                    }
                    var promises = [];
                  
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.FinancialAccountTypes.SupplierPostpaid.SupplierPostpaidDefinitionSettings ,TOne.WhS.BusinessEntity.MainExtensions",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);