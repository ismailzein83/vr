(function (app) {

    'use strict';

    OperatorDeclarationServicePostPaidCDR.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OperatorDeclarationServicePostPaidCDR(UtilsService, VRUIUtilsService) {
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
                var ctor = new OperatorDeclarationServicePostPaidCDRCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/MainExtensions/Templates/OperatorDeclarationServicePostPaidCDRTemplate.html"
        };

        function OperatorDeclarationServicePostPaidCDRCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var trafficTypeSelectorAPI;
            var trafficTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var trafficDirectionSelectorAPI;
            var trafficDirectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTrafficTypeSelectorReady = function (api) {
                    trafficTypeSelectorAPI = api;
                    trafficTypeSelectorReadyDeferred.resolve();
                };

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
                    var postPaidCDREntity;

                    if (payload != undefined) {

                        postPaidCDREntity = payload.settings;

                        $scope.scopeModel.successfulCalls = postPaidCDREntity.SuccessfulCalls;
                        $scope.scopeModel.totalDuration = postPaidCDREntity.TotalDuration;
                        $scope.scopeModel.totalChargedDuration = postPaidCDREntity.TotalChargedDuration;
                    }

                    function loadTrafficTypeSelector() {
                        var trafficTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficTypeSelectorReadyDeferred.promise.then(function () {
                            var payload;
                            if (postPaidCDREntity != undefined)
                                payload = {
                                    selectedIds: postPaidCDREntity.TrafficType
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficTypeSelectorAPI, payload, trafficTypeSelectorLoadDeferred);
                        });
                        return trafficTypeSelectorLoadDeferred.promise;
                    }

                    function loadTrafficDirectionSelector() {

                        var trafficDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficDirectionSelectorReadyDeferred.promise.then(function () {
                            var payload;

                            if (postPaidCDREntity != undefined)
                                payload = {
                                    selectedIds: postPaidCDREntity.TrafficDirection
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficDirectionSelectorAPI, payload, trafficDirectionSelectorLoadDeferred);
                        });
                        return trafficDirectionSelectorLoadDeferred.promise;
                    }

                    var trafficTypeSelectorLoadPromise = loadTrafficTypeSelector();
                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    promises.push(trafficTypeSelectorLoadPromise);
                    promises.push(trafficDirectionSelectorLoadPromise);

                    

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices.PostpaidCDR,Retail.BusinessEntity.MainExtensions",
                        TrafficType: trafficTypeSelectorAPI.getSelectedIds(),
                        TrafficDirection: trafficDirectionSelectorAPI.getSelectedIds(),
                        SuccessfulCalls: $scope.scopeModel.successfulCalls,
                        TotalDuration: $scope.scopeModel.totalDuration,
                        TotalChargedDuration: $scope.scopeModel.totalChargedDuration,
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationservicePostpaidcdr', OperatorDeclarationServicePostPaidCDR);

})(app);