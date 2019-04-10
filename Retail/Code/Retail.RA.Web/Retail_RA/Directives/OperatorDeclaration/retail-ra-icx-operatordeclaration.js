//(function (app) {

//    'use strict';

//    icxOperatorDeclaration.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService', 'Retail_Be_TrafficTypeEnum'];

//    function icxOperatorDeclaration(UtilsService, VRUIUtilsService, VRNotificationService, Retail_Be_TrafficTypeEnum) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new OperatorDeclaration($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "Ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/Retail_RA/Directives/OperatorDeclaration/Templates/IcxOperatorDeclarationTemplate.html"

//        };
//        function OperatorDeclaration($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var operatorSelectedPromiseDeferred;

//            var operatorSelectorAPI;
//            var operatorSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            var periodSelectorAPi;
//            var periodSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
//            var trafficType = Retail_Be_TrafficTypeEnum.Interconnect.value;
//            var operatorDeclarationEntity;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.selectedPeriods = [];
//                $scope.scopeModel.onOperatorSelectorReady = function (api) {
//                    operatorSelectorAPI = api;
//                    operatorSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onPeriodSelectorReady = function (api) {
//                    periodSelectorAPi = api;
//                    periodSelectorReadyPromiseDeferred.resolve();
//                };

//                $scope.scopeModel.onOperatorSelectionChanged = function () {
//                    var operatorId = operatorSelectorAPI.getSelectedIds();
//                    if (operatorId != undefined) {
//                        var setLoader = function (value) { $scope.isLoadingSelector = value };
//                        var payload = {
//                            operatorId: operatorId,
//                            trafficType: trafficType
//                        };
//                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, periodSelectorAPi, payload, setLoader, operatorSelectedPromiseDeferred);

//                    }
//                    else if (periodSelectorAPi != undefined && $scope.scopeModel.selectedPeriods != undefined)
//                        $scope.scopeModel.selectedPeriods.length = 0;
//                };

//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];
//                    if (payload != undefined) {
//                        operatorDeclarationEntity = payload.operatorDeclaration;
//                    }
//                    promises.push(loadOperatorPeriodSection());
//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    var data = {
//                        // $type: "Retail.RA.Business.IntlOperatorDeclarationServicesCustomObjectTypeSettings, Retail.RA.Business"
//                    };
//                    return data;
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function loadOperatorPeriodSection() {
//                var loadOperatorPromiseDeferred = UtilsService.createPromiseDeferred();

//                var promises = [];
//                promises.push(loadOperatorPromiseDeferred.promise);

//                var payload = {};
//                if (operatorDeclarationEntity != undefined && operatorDeclarationEntity.OperatorId != undefined) {
//                    payload.selectedIds = operatorDeclarationEntity.OperatorId;
//                    operatorSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
//                }

//                operatorSelectorReadyPromiseDeferred.promise.then(function () {
//                    VRUIUtilsService.callDirectiveLoad(operatorSelectorAPI, payload, loadOperatorPromiseDeferred);
//                });

//                if (operatorDeclarationEntity != undefined && operatorDeclarationEntity.OperatorId != undefined) {
//                    var loadPeriodPromiseDeferred = UtilsService.createPromiseDeferred();

//                    promises.push(loadPeriodPromiseDeferred.promise);

//                    UtilsService.waitMultiplePromises([periodSelectorReadyPromiseDeferred.promise, operatorSelectedPromiseDeferred.promise]).then(function () {
//                        var payload = {
//                            operatorId: operatorDeclarationEntity.OperatorId,
//                            trafficType: trafficType
//                        };
//                        VRUIUtilsService.callDirectiveLoad(periodSelectorAPi, payload, loadPeriodPromiseDeferred);

//                        operatorSelectedPromiseDeferred = undefined;
//                    });
//                }

//                return UtilsService.waitMultiplePromises(promises);
//            }
//        }
//    }

//    app.directive('retailRaIcxOperatordeclaration', icxOperatorDeclaration);

//})(app);
