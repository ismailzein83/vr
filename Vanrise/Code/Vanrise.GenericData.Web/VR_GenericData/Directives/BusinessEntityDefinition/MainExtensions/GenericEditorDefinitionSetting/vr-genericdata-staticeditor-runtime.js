"use strict";

app.directive("vrGenericdataStaticeditorRuntime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new StaticEditorRuntimeSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/StaticEditorRuntimeSettingTemplate.html"
        };
        function StaticEditorRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var editorRuntimeDirectiveApi;
            var editorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

            var selectedValues;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeDirectiveApi = api;
                    editorRuntimeDirectivePromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined)
                    {
                        selectedValues = payload.selectedValues;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;

                        if(definitionSettings != undefined)
                        {
                            $scope.scopeModel.runtimeEditor = definitionSettings.DirectiveName;
                        }
                    }
                    promises.push(loadBusinessEntityDefinitionSelector());
                      
                    function loadBusinessEntityDefinitionSelector() {
                        var editorRuntimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                        editorRuntimeDirectivePromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                definitionSettings : definitionSettings,
                                selectedValues : selectedValues,
                                dataRecordTypeId: dataRecordTypeId,
                                historyId: historyId
                            };
                            VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveApi, directivePayload, editorRuntimeDirectiveLoadDeferred);
                        });
                        return editorRuntimeDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    editorRuntimeDirectiveApi.setData(dicData);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }


        }

        return directiveDefinitionObject;
    }
]);