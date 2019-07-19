(function (app) {

    'use strict';

    ExecuteActionEditorOnValueChangedDefinition.$inject = ["UtilsService"];

    function ExecuteActionEditorOnValueChangedDefinition(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ExecuteActionEditorOnValueChangedDefinitionCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorExecuteActionSetting/Templates/ExecuteActionOnValueChangedDefinitionTemplate.html"

        };

        function ExecuteActionEditorOnValueChangedDefinitionCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var dataRecordFieldsSelectorAPI;
            var dataRecordFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedFields = [];

                $scope.scopeModel.onDataRecordFieldsSelectorReady = function (api) {
                    dataRecordFieldsSelectorAPI = api;
                    dataRecordFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCallOnValueChanged = function () {
                    $scope.scopeModel.selectedFields.length = 0;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var context;
                    var definitionSettings;
                    var onFieldsChanged;

                    if (payload != undefined) {
                        context = payload.context;
                        definitionSettings = payload.definitionSettings;
                        if (definitionSettings != undefined) {
                            onFieldsChanged = definitionSettings.OnFieldsChanged;
                        }
                    }

                    if (context != undefined) {
                        $scope.scopeModel.dataRecordFields = context.getFields();
                    }

                    loadStaticData();

                    if (onFieldsChanged != undefined) {
                        var dataRecordFieldsSelectorLoadedPromise = loadDataRecordFieldsSelector();
                        promises.push(dataRecordFieldsSelectorLoadedPromise);
                    }

                    function loadStaticData() {
                        if (definitionSettings != undefined) {
                            $scope.scopeModel.callOnValueChanged = definitionSettings.CallOnValueChanged;
                        }
                    }

                    function loadDataRecordFieldsSelector() {
                        var dataRecordFieldsSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        dataRecordFieldsSelectorReadyPromiseDeferred.promise.then(function () {

                            for (var i = 0; i < onFieldsChanged.length; i++) {
                                var selectedField = UtilsService.getItemByVal($scope.scopeModel.dataRecordFields, onFieldsChanged[i].FieldName, "FieldName");
                                if (selectedField != null)
                                    $scope.scopeModel.selectedFields.push(selectedField);
                            }

                            dataRecordFieldsSelectorLoadPromiseDeferred.resolve();
                        });

                        return dataRecordFieldsSelectorLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    function getOnFieldsChanged() {
                        if (!$scope.scopeModel.callOnValueChanged || $scope.scopeModel.selectedFields.length == 0)
                            return undefined;

                        var onFieldsChanged = [];
                        for (var i = 0; i < $scope.scopeModel.selectedFields.length; i++)
                            onFieldsChanged.push({ FieldName: $scope.scopeModel.selectedFields[i].FieldName });

                        return onFieldsChanged;
                    }

                    return {
                        CallOnValueChanged: $scope.scopeModel.callOnValueChanged,
                        OnFieldsChanged: getOnFieldsChanged()
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataExecuteactioneditorsettingDefinitionOnvaluechanged', ExecuteActionEditorOnValueChangedDefinition);

})(app);