"use strict";
app.directive("vrGenericdataGenericbusinessentitySelectorfilter", ["UtilsService", "VRUIUtilsService", 'VR_GenericData_GenericBusinessEntityAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_GenericBusinessEntityAPIService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntitySelectorFilter.html"
        };

        function GenericBusinessEntitySelectorFilterCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorConditionDirectiveAPI;
            var selectorConditionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onSelectorConditionDirectiveReady = function (api) {
                    selectorConditionDirectiveAPI = api;
                    selectorConditionDirectiveReadyDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var beDefinitionId;

                    if (payload != undefined) {
                        beDefinitionId = payload.beDefinitionId;

                        if (payload.beRuntimeSelectorFilter != undefined)
                            $scope.scopeModel.IsNotApplicableInSearch = payload.beRuntimeSelectorFilter.NotApplicableInSearch;
                    }

                    var loadSelectorConditionDirectivePromise = loadSelectorConditionDirective();
                    promises.push(loadSelectorConditionDirectivePromise);

                    function loadSelectorConditionDirective() {
                        var loadSelectorConditionDirectiveDeferred = UtilsService.createPromiseDeferred();

                        selectorConditionDirectiveReadyDeferred.promise.then(function () {

                            var selectorConditionDirectivePayload = {
                                beDefinitionId: beDefinitionId
                            };
                            if (payload != undefined && payload.beRuntimeSelectorFilter != undefined) {
                                selectorConditionDirectivePayload.genericBESelectorCondition = payload.beRuntimeSelectorFilter.GenericBESelectorCondition;
                            }
                            VRUIUtilsService.callDirectiveLoad(selectorConditionDirectiveAPI, selectorConditionDirectivePayload, loadSelectorConditionDirectiveDeferred);
                        });
                        return loadSelectorConditionDirectiveDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var result = selectorConditionDirectiveAPI.getData();

                    if (result != undefined) {
                        return {
                            $type: "Vanrise.GenericData.Business.GenericBERuntimeSelectorFilter,Vanrise.GenericData.Business",
                            NotApplicableInSearch: $scope.scopeModel.IsNotApplicableInSearch,
                            GenericBESelectorCondition: result
                        };
                    }
                    return null;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);