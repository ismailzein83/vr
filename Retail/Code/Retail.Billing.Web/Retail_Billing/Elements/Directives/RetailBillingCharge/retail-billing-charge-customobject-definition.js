(function (app) {

    'use strict';

    RetailbillingChargeCustomObjectDefinition.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function RetailbillingChargeCustomObjectDefinition(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RetailBEChargeCustomObjectDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingCharge/Templates/RetailBillingChargeCustomObjectDefinitionTemplate.html"
        };

        function RetailBEChargeCustomObjectDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordTypeSelectorAPI;
            var dataRecordTypeSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.normalColNum = ctrl.normalColNum;

                $scope.scopeModel.onDataRecordTypeSelectorReady = function (api) {
                    dataRecordTypeSelectorAPI = api;
                    dataRecordTypeSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var settings;

                    if (payload != undefined) {
                        settings = payload.settings;
                    }

                    function loadDataRecordTypeSelector() {
                        var loadDataRecordTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordTypeSelectorReadyPromiseDeferred.promise.then(function () {

                            var directivePayload;
                            if (settings != undefined) {
                                directivePayload = {
                                    selectedIds: settings.TargetRecordTypeId
                                };
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeSelectorAPI, directivePayload, loadDataRecordTypeSelectorPromiseDeferred);
                        });

                        return loadDataRecordTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({ promises: [loadDataRecordTypeSelector()] });
                };

                api.getData = function () {
                    var data = {
                        $type: "Retail.Billing.MainExtensions.RetailBillingCharge.RetailBillingChargeCustomObjectTypeSettings, Retail.Billing.MainExtensions",
                        TargetRecordTypeId: dataRecordTypeSelectorAPI.getSelectedIds()
                    };

                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('retailBillingChargeCustomobjectDefinition', RetailbillingChargeCustomObjectDefinition);
})(app);