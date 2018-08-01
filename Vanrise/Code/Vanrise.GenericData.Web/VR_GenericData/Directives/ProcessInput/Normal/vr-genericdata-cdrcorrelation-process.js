"use strict";

app.directive("vrGenericdataCdrcorrelationProcess", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CDRCorrelationProcessCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/ProcessInput/Normal/Templates/GenericDataCDRCorrelationProcessTemplate.html"
        };

        function CDRCorrelationProcessCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;



            var cdrCorrelationDefinitionSelectorAPI;
            var cdrCorrelationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.dateTimeMargin = "00:00:05";
                $scope.scopeModel.durationMargin = "00:00:05";
                $scope.scopeModel.batchIntervalTime = "01:00:00";

                $scope.scopeModel.onCDRCorrelationDefinitionSelectorReady = function (api) {
                    cdrCorrelationDefinitionSelectorAPI = api;
                    cdrCorrelationDefinitionSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadCDRCorrelationDefinitionSelectorPromise = loadCDRCorrelationDefinitionSelector();
                    promises.push(loadCDRCorrelationDefinitionSelectorPromise);

                    function loadCDRCorrelationDefinitionSelector() {

                        var cdrCorrelationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        cdrCorrelationDefinitionSelectorReadyDeferred.promise.then(function () {
                            VRUIUtilsService.callDirectiveLoad(cdrCorrelationDefinitionSelectorAPI, undefined, cdrCorrelationDefinitionSelectorLoadDeferred);
                        });

                        return cdrCorrelationDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        InputArguments: {
                            $type: "Vanrise.GenericData.BP.Arguments.CDRCorrelationProcessInput,Vanrise.GenericData.BP.Arguments",
                            CDRCorrelationDefinitionId: cdrCorrelationDefinitionSelectorAPI.getSelectedIds(),
                            DateTimeMargin: $scope.scopeModel.dateTimeMargin,
                            DurationMargin: $scope.scopeModel.durationMargin,
                            BatchIntervalTime: $scope.scopeModel.batchIntervalTime
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
