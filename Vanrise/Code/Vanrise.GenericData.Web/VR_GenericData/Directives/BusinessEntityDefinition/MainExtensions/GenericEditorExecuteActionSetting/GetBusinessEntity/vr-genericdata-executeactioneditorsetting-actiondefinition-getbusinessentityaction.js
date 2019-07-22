(function (app) {

    'use strict';

    CallRestAPIActionDefinitionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_CallRestAPIHTTPMethodEnum'];

    function CallRestAPIActionDefinitionDirective(UtilsService, VRUIUtilsService, VR_GenericData_CallRestAPIHTTPMethodEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CallRestAPIActionDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/GetBusinessEntity/Templates/GetBusinessEntityExecuteActionDefinitionTemplate.html'
        };

        function CallRestAPIActionDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var inputItemsGridAPI;
            var inputItemsGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var outputItemsGridAPI;
            var outputItemsGridReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRestApiInputItemsGridReady = function (api) {
                    inputItemsGridAPI = api;
                    inputItemsGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onRestApiOutputItemsGridReady = function (api) {
                    outputItemsGridAPI = api;
                    outputItemsGridReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.isInputValid = function () {
                    var inputItems = inputItemsGridAPI != undefined ? inputItemsGridAPI.getData() : undefined;

                    if (inputItems != undefined && inputItems.length > 0) {
                        for (var i = 0; i < inputItems.length; i++) {
                            var firstInputItem = inputItems[i];
                            if (firstInputItem.PropertyName == undefined)
                                continue;

                            for (var j = i + 1; j < inputItems.length; j++) {
                                var secondInputItem = inputItems[j];
                                if (secondInputItem.PropertyName == undefined)
                                    continue;

                                if (firstInputItem.PropertyName == secondInputItem.PropertyName)
                                    return "Property Name should be Unique!";
                            }
                        }
                    }

                    return null;
                };

                $scope.scopeModel.isOutputValid = function () {
                    var outputItems = outputItemsGridAPI != undefined ? outputItemsGridAPI.getData() : undefined;

                    if (outputItems != undefined && outputItems.length > 0) {
                        for (var i = 0; i < outputItems.length; i++) {
                            var firstOutputItem = outputItems[i];
                            if (firstOutputItem.FieldName == undefined)
                                continue;

                            for (var j = i + 1; j < outputItems.length; j++) {
                                var secondOutputItem = outputItems[j];
                                if (secondOutputItem.FieldName == undefined)
                                    continue;

                                if (firstOutputItem.FieldName == secondOutputItem.FieldName)
                                    return "Field Name should be Unique!";
                            }
                        }
                    }

                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;

                    var apiAction;
                    var httpMethodType;
                    var inputItems;
                    var outputItems;

                    if (payload != undefined) {
                        context = payload.context;

                        var settings = payload.settings;
                        if (settings != undefined) {
                            apiAction = settings.APIAction;
                            httpMethodType = settings.HTTPMethodType;
                            inputItems = settings.InputItems;
                            outputItems = settings.OutputItems;
                        }
                    }

                    loadStaticFields();
                    loadHTTPMethodSelector();

                    var inputItemsGridLoadPromise = loadRestApiInputItemsGrid();
                    promises.push(inputItemsGridLoadPromise);

                    var outputItemsGridLoadPromise = loadRestApiOutputItemsGrid();
                    promises.push(outputItemsGridLoadPromise);

                    function loadStaticFields() {
                        $scope.scopeModel.apiAction = apiAction;
                    }

                    function loadHTTPMethodSelector() {
                        ctrl.datasource = UtilsService.getArrayEnum(VR_GenericData_CallRestAPIHTTPMethodEnum);

                        if (httpMethodType != undefined) {
                            VRUIUtilsService.setSelectedValues(httpMethodType, 'value', $attrs, ctrl);
                        }
                    }

                    function loadRestApiInputItemsGrid() {
                        var inputItemsGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        inputItemsGridReadyPromiseDeferred.promise.then(function () {
                            var inputItemsGridPayload = {
                                beRestAPIItems: inputItems,
                                context: context
                            };

                            VRUIUtilsService.callDirectiveLoad(inputItemsGridAPI, inputItemsGridPayload, inputItemsGridLoadPromiseDeferred);
                        });
                        return inputItemsGridLoadPromiseDeferred.promise;
                    }

                    function loadRestApiOutputItemsGrid() {
                        var outputItemsGridLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        outputItemsGridReadyPromiseDeferred.promise.then(function () {
                            var outputItemsGridPayload = {
                                beRestAPIItems: outputItems,
                                context: context
                            };

                            VRUIUtilsService.callDirectiveLoad(outputItemsGridAPI, outputItemsGridPayload, outputItemsGridLoadPromiseDeferred);
                        });
                        return outputItemsGridLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var definitionData = {
                        $type: "Vanrise.GenericData.MainExtensions.CallRestAPIExecuteActionType, Vanrise.GenericData.MainExtensions",
                        APIAction: $scope.scopeModel.apiAction,
                        HTTPMethodType: ctrl.selectedvalues != undefined ? ctrl.selectedvalues.value : undefined,
                        InputItems: inputItemsGridAPI.getData(),
                        OutputItems: outputItemsGridAPI.getData()
                    };

                    return definitionData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingActiondefinitionGetbusinessentityaction', CallRestAPIActionDefinitionDirective);

})(app);