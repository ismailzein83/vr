(function (app) {

    'use strict';

    RowsContainerEditorFieldsDefinitionDirective.$inject = ['VRUIUtilsService','UtilsService','VR_GenericData_ContainerTypeEnum'];

    function RowsContainerEditorFieldsDefinitionDirective(VRUIUtilsService, UtilsService, VR_GenericData_ContainerTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RowsContainerEditorDefinitionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/RowsContainer/Templates/RowsContainerEditorDefinitionFieldsSettingTemplate.html'
        };

        function RowsContainerEditorDefinitionCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var context;
            var setFieldsNumber;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.datasource = [];

                $scope.scopeModel.addField = function () {

                    var dataItem= prepareFieldData(undefined, undefined);
                    $scope.scopeModel.datasource.push(dataItem);
                    setFieldsNumber($scope.scopeModel.datasource.length);
                };

                $scope.scopeModel.onRemoveField = function (field) {
                    var index = $scope.scopeModel.datasource.indexOf(field);
                    if (index != -1)
                        $scope.scopeModel.datasource.splice(index, 1);
                    setFieldsNumber($scope.scopeModel.datasource.length);
                };

                $scope.scopeModel.disableAddField = function () {
                    if (context == undefined)
                        return true;

                    return false;
                };

                $scope.scopeModel.isValid = function () {
                    if ($scope.scopeModel.datasource == undefined || $scope.scopeModel.datasource.length == 0)
                        return "You Should add at least one row.";

                    return null;
                };

                defineAPI();
            }
            function prepareFieldData(fieldObject) {
                if (fieldObject == undefined)
                    fieldObject = {};

                var fieldData = {
                    directiveAPI: undefined,
                    fieldEditorDefinitionReadyPromiseDeferred: UtilsService.createPromiseDeferred()
                };

                fieldData.onGenericBEFieldEditorDefinitionDirectiveReady = function (api) {
                    fieldData.directiveAPI = api;
                    fieldData.fieldEditorDefinitionReadyPromiseDeferred.resolve();
                };
                var payload = {
                    settings: fieldObject.payload,
                    context: context,
                    containerType: VR_GenericData_ContainerTypeEnum.Row.value
                };
                fieldData.fieldEditorDefinitionReadyPromiseDeferred.promise.then(function () {

                    if (fieldObject.fieldEditorDefinitionLoadPromiseDeferred != undefined) {
                        VRUIUtilsService.callDirectiveLoad(fieldData.directiveAPI, payload, fieldObject.fieldEditorDefinitionLoadPromiseDeferred);
                    }
                    else {
                        fieldData.fieldEditorDefinitionReadyPromiseDeferred.promise.then(function () {
                            var setLoader = function (value) { fieldData.isEditorDirectiveLoading = value; };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, fieldData.directiveAPI, payload, setLoader);
                        });
                    }
                });
                

                fieldData.onFieldEditorDefinitionSelectionChanged = function (selectedFieldEditor) {
                    fieldData.title = "";
                    if (selectedFieldEditor == undefined)
                        return;

                    fieldData.title = selectedFieldEditor.Title;
                };

                return fieldData;
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var fields;
                    var promises = [];
                    if (payload != undefined) {
                        fields = payload.fields;
                        context = payload.context;
                        setFieldsNumber = payload.setFieldsNumber;
                    }

                    if (fields != undefined) {
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            var fieldObject = {
                                payload: field,
                                fieldEditorDefinitionLoadPromiseDeferred: UtilsService.createPromiseDeferred()
                            };
                            promises.push(fieldObject.fieldEditorDefinitionLoadPromiseDeferred.promise);
                            $scope.scopeModel.datasource.push(prepareFieldData(fieldObject));
                        }
                    }
                    return UtilsService.waitPromiseNode({ promises: promises });
                };

                api.getData = function () {
                    var rowContainers = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var field = $scope.scopeModel.datasource[i];
                        rowContainers.push(field.directiveAPI != undefined ? field.directiveAPI.getData() : undefined);
                    }
                    return rowContainers;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataRowscontainereditorFieldsDefinition', RowsContainerEditorFieldsDefinitionDirective);

})(app);