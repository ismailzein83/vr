"use strict";

app.directive("vrAnalyticDaprofcalcGeneratealertprocessinput", ['UtilsService', 'VRUIUtilsService', 'VRValidationService', 'DAProfCalcChunkTimeEnum',
    function (UtilsService, VRUIUtilsService, VRValidationService, DAProfCalcChunkTimeEnum) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Analytic/Directives/DataAnalysis/ProfilingAndCalculation/ProcessInput/Templates/DAProfCalcGenerateAlertProcessInputTemplate.html'
        };

        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }

            $scope.scopeModel = {};
            var alertRuleTypeSelectorAPI;
            var alertRuleTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.onAlertRuleTypeSelectorReady = function (api) {
                alertRuleTypeSelectorAPI = api;
                alertRuleTypeSelectorReadyDeferred.resolve();
            };

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            function defineAPI() {

                var api = {};
                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.Analytic.BP.Arguments.DAProfCalcGenerateAlertInput, Vanrise.Analytic.BP.Arguments",
                            AlertRuleTypeId: alertRuleTypeSelectorAPI.getSelectedIds(),
                            FromTime: $scope.fromDate,
                            ToTime: new Date($scope.toDate.setDate($scope.toDate.getDate() + 1)),
                            ChunkTime: $scope.selectedChunkTime.value
                        }
                    };
                };

                api.load = function (payload) {
                    $scope.chunkTimes = UtilsService.getArrayEnum(DAProfCalcChunkTimeEnum);

                    var promises = [];

                    var alertRuleTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    alertRuleTypeSelectorReadyDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(alertRuleTypeSelectorAPI, undefined, alertRuleTypeSelectorLoadDeferred);
                    });

                    promises.push(alertRuleTypeSelectorLoadDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);