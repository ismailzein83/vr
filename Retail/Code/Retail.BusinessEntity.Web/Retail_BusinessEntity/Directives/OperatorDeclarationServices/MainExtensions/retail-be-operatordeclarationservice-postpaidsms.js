(function (app) {

    'use strict';

    OperatorDeclarationServicePostPaidSMS.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OperatorDeclarationServicePostPaidSMS(UtilsService, VRUIUtilsService) {
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
                var ctor = new OperatorDeclarationServicePostPaidSMSCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/MainExtensions/Templates/OperatorDeclarationServicePostPaidSMSTemplate.html"
        };

        function OperatorDeclarationServicePostPaidSMSCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
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
                    var postPaidSMSEntity;

                    if (payload != undefined) {

                        postPaidSMSEntity = payload.postPaidSMSEntity;

                        $scope.scopeModel.numberOfSMSs = postPaidSMSEntity.NumberOfSMSs;
                    }

                    var trafficTypeSelectorLoadPromise = loadTrafficTypeSelector();
                    var trafficDirectionSelectorLoadPromise = loadTrafficDirectionSelector();

                    promises.push(trafficTypeSelectorLoadPromise);
                    promises.push(trafficDirectionSelectorLoadPromise);

                    function loadTrafficTypeSelector() {
                        var trafficTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficTypeSelectorReadyDeferred.promise.then(function () {
                            var payload;
                            if (postPaidSMSEntity != undefined)
                                payload = {
                                    selectedIds: postPaidSMSEntity.TrafficType
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficTypeSelectorAPI, payload, trafficTypeSelectorLoadDeferred);
                        });
                        return trafficTypeSelectorLoadDeferred.promise;
                    }

                    function loadTrafficDirectionSelector() {
                        var trafficDirectionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        trafficDirectionSelectorReadyDeferred.promise.then(function () {
                            var payload;

                            if (postPaidSMSEntity != undefined)
                                payload = {
                                    selectedIds: postPaidSMSEntity.TrafficDirection
                                };
                            VRUIUtilsService.callDirectiveLoad(trafficDirectionSelectorAPI, payload, trafficDirectionSelectorLoadDeferred);
                        });
                        return trafficDirectionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices.PostpaidSMS,Retail.BusinessEntity.MainExtensions",
                        TrafficType: $scope.scopeModel.trafficType,

                        TrafficDirection: $scope.scopeModel.trafficDirection,

                        NumberOfSMSs: $scope.scopeModel.numberOfSMSs,
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationservicePostpaidsms', OperatorDeclarationServicePostPaidSMS);

})(app);