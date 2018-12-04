(function (app) {

    'use strict';

    OperatorDeclarationServiceSMS.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OperatorDeclarationServiceSMS(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDeclarationServiceSMSCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/MainExtensions/Templates/OperatorDeclarationServiceSMSTemplate.html"
        };

        function OperatorDeclarationServiceSMSCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var trafficDirectionSelectorAPI;
            var trafficDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTrafficDirectionSelectorReady = function (api) {
                    trafficDirectionSelectorAPI = api;
                    trafficDirectionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var SMSEntity;

                    if (payload != undefined) {

                        SMSEntity = payload.settings;
                        $scope.scopeModel.amount = SMSEntity.Amount;
                        $scope.scopeModel.numberOfSMSs = SMSEntity.NumberOfSMSs;
                    }

                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    promises.push(trafficDirectionSelectorLoadPromise);

                    function loadTrafficDirectionSelector() {
                        var trafficDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficDirectionSelectorReadyDeferred.promise.then(function () {
                            var payload;

                            if (SMSEntity != undefined)
                                payload = {
                                    selectedIds: SMSEntity.TrafficDirection
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficDirectionSelectorAPI, payload, trafficDirectionSelectorLoadDeferred);
                        });
                        return trafficDirectionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices.SMS,Retail.BusinessEntity.MainExtensions",
                        TrafficDirection: trafficDirectionSelectorAPI.getSelectedIds(),
                        NumberOfSMSs: $scope.scopeModel.numberOfSMSs,
                        Amount: $scope.scopeModel.amount
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationserviceSms', OperatorDeclarationServiceSMS);

})(app);