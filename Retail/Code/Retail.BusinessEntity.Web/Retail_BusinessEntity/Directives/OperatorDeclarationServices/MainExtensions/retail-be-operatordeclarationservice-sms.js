﻿(function (app) {

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
                    var SMSEntity;

                    if (payload != undefined) {

                        SMSEntity = payload.settings;
                        $scope.scopeModel.amount = SMSEntity.Amount;
                        $scope.scopeModel.numberOfSMSs = SMSEntity.NumberOfSMSs;
                    }

                    var trafficTypeSelectorLoadPromise = loadTrafficTypeSelector();
                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    promises.push(trafficTypeSelectorLoadPromise);
                    promises.push(trafficDirectionSelectorLoadPromise);

                    function loadTrafficTypeSelector() {
                        var trafficTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficTypeSelectorReadyDeferred.promise.then(function () {
                            var payload;
                            if (SMSEntity != undefined)
                                payload = {
                                    selectedIds: SMSEntity.TrafficType
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficTypeSelectorAPI, payload, trafficTypeSelectorLoadDeferred);
                        });
                        return trafficTypeSelectorLoadDeferred.promise;
                    }

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
                        TrafficType: trafficTypeSelectorAPI.getSelectedIds(),
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