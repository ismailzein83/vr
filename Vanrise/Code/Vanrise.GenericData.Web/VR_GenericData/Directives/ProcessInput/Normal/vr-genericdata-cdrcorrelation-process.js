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
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/ProcessInput/Normal/Templates/GenericDataCDRCorrelationProcessTemplate.html"
        };

        function CDRCorrelationProcessCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var cdrCorrelationDefinitionSelectorAPI;
            var cdrCorrelationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

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
                            DateTimeMargin: $scope.scopeModel.dateTimeMargin,
                            DurationMargin: $scope.scopeModel.durationMargin,
                            CDRCorrelationDefinitionId: cdrCorrelationDefinitionSelectorAPI.getSelectedIds()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;
    }]);
