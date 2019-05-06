(function (app) {

    'use strict';

    intlOperatorDeclaration.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService','Retail_Be_TrafficTypeEnum'];

    function intlOperatorDeclaration(UtilsService, VRUIUtilsService, VRNotificationService, Retail_Be_TrafficTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDeclaration($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OperatorDeclaration/Templates/IntlOperatorDeclarationTemplate.html"

        };
        function OperatorDeclaration($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var operatorSelectedPromiseDeferred;

            var selectedValues;

            var operatorId;
            var periodId;

            var operatorSelectorAPI;
            var operatorSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var periodSelectorAPi;
            var periodSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var trafficType = Retail_Be_TrafficTypeEnum.International.value;
            var operatorDeclarationEntity;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedPeriods = [];
                $scope.scopeModel.onOperatorSelectorReady = function (api) {
                    operatorSelectorAPI = api;
                    operatorSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onPeriodSelectorReady = function (api) {
                    periodSelectorAPi = api;
                    periodSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOperatorSelectionChanged = function () {
                    var operatorId = operatorSelectorAPI.getSelectedIds();
                    if (operatorId != undefined) {
                        var setLoader = function (value) { $scope.isLoadingSelector = value };
                        var payload= {
                            operatorId: operatorId,
                            trafficType: trafficType
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, periodSelectorAPi, payload, setLoader, operatorSelectedPromiseDeferred);

                    }
                    else if (periodSelectorAPi != undefined && $scope.scopeModel.selectedPeriods != undefined)
                        $scope.scopeModel.selectedPeriods.length = 0;
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        operatorDeclarationEntity = payload.operatorDeclaration;
                        selectedValues = payload.selectedValues;
                    }
                    if (selectedValues != undefined) {
                        operatorId = selectedValues.Operator;
                        periodId = selectedValues.Period;
                    }
                    promises.push(loadOperatorPeriodSection());
                    return UtilsService.waitMultiplePromises(promises);
                };

               
                api.setData = function (Object) {
                    Object.Operator = operatorSelectorAPI.getSelectedIds();
                    Object.Period = periodSelectorAPi.getSelectedIds();
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadOperatorPeriodSection() {
                var loadOperatorPromiseDeferred = UtilsService.createPromiseDeferred();
                    
                var promises = [];
                promises.push(loadOperatorPromiseDeferred.promise);

                var payload = {
                    businessEntityDefinitionId: '1A4A2877-D4C0-4B97-B4F0-2942BA342485'
                };
                if (operatorId != undefined) {
                    payload.selectedIds = operatorId;
                    operatorSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                }
               
                operatorSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(operatorSelectorAPI, payload, loadOperatorPromiseDeferred);
                    });

                if (operatorId != undefined) {
                    var loadPeriodPromiseDeferred = UtilsService.createPromiseDeferred();

                    promises.push(loadPeriodPromiseDeferred.promise);

                    UtilsService.waitMultiplePromises([periodSelectorReadyPromiseDeferred.promise, operatorSelectedPromiseDeferred.promise]).then(function () {
                        var payload = {
                            operatorId: operatorId,
                            trafficType: trafficType,
                            selectedIds: periodId
                        };
                        VRUIUtilsService.callDirectiveLoad(periodSelectorAPi, payload, loadPeriodPromiseDeferred);
                       
                        operatorSelectedPromiseDeferred = undefined;
                    });
                }

                return UtilsService.waitMultiplePromises(promises);
            }
        }
    }

    app.directive('retailRaIntlOperatordeclaration', intlOperatorDeclaration);

})(app);
