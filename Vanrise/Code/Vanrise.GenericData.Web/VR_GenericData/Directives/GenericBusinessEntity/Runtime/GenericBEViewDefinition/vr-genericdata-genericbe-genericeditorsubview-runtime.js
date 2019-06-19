﻿(function (app) {

    'use strict';

    GenericEditorSubViewRuntimeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService'];

    function GenericEditorSubViewRuntimeDirective(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericEditorSubViewRuntimeCtrol($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEViewDefinition/Templates/GenericEditorSubViewViewTemplate.html'
        };

        function GenericEditorSubViewRuntimeCtrol($scope, ctrl) {
            this.initializeController = initializeController;

            var businessEntityDefinitionId;
            var genericBEGridView;
            var genericBusinessEntityId;
            var parentBEEntity;
            var genericEditorDefinitionSetting;
            var genericBEDefinitionSettings;

            var editorRuntimeDirectiveAPI;
            var editorRuntimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeDirectiveAPI = api;
                    editorRuntimeDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        businessEntityDefinitionId = payload.businessEntityDefinitionId;
                        genericBEGridView = payload.genericBEGridView;
                        genericBusinessEntityId = payload.genericBusinessEntityId;
                        parentBEEntity = payload.parentBEEntity;

                        if (genericBEGridView != undefined && genericBEGridView.Settings != undefined && genericBEGridView.Settings.GenericEditorDefinitionSetting != undefined) {
                            genericEditorDefinitionSetting = genericBEGridView.Settings.GenericEditorDefinitionSetting;
                        }
                    }

                    $scope.scopeModel.isEditorDirectiveLoading = true;
                    if (businessEntityDefinitionId != undefined)
                        initialPromises.push(loadBusinessEntityDefinitionSettings());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            directivePromises.push(loadEditorRuntimeDirective());

                            return {
                                promises: directivePromises,
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode).finally(function () {
                        $scope.scopeModel.isEditorDirectiveLoading = false;
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function loadBusinessEntityDefinitionSettings() {
                return VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(businessEntityDefinitionId).then(function (response) {
                    if (response != undefined)
                        genericBEDefinitionSettings = response;
                });
            }

            function loadEditorRuntimeDirective() {
                var loadEditorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                editorRuntimeDirectiveReadyPromiseDeferred.promise.then(function () {
                    var defaultValues = {};

                    if (parentBEEntity != undefined && parentBEEntity.FieldValues != undefined) {
                        var fieldValues = parentBEEntity.FieldValues;
                        for (var key in fieldValues) {
                            if (key != "$type" && !fieldValues[key].isHidden)
                                defaultValues[key] = fieldValues[key].Value;
                        }
                    }

                    var runtimeEditorPayload = {
                        selectedValues: defaultValues,
                        dataRecordTypeId: genericBEDefinitionSettings != undefined ? genericBEDefinitionSettings.DataRecordTypeId : undefined,
                        definitionSettings: genericEditorDefinitionSetting,
                        runtimeEditor: genericEditorDefinitionSetting != undefined ? genericEditorDefinitionSetting.RuntimeEditor : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveAPI, runtimeEditorPayload, loadEditorRuntimeDirectivePromiseDeferred);
                });
                return loadEditorRuntimeDirectivePromiseDeferred.promise;
            }
        }
    }

    app.directive('vrGenericdataGenericbeGenericeditorsubviewRuntime', GenericEditorSubViewRuntimeDirective);

})(app);