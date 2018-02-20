'use strict';
app.directive('vrGenericdataGenericbusinessentityRuntimeEditor', ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new EditorCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/Editor/Templates/EditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            var selectedValues;

            function initializeController() {
                ctrl.sections = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                  
                    if (payload != undefined && payload.sections != undefined) {
                        selectedValues = payload.selectedValues;
                        ctrl.sections.length = 0;
                        var promises = [];

                        var editorpromise = UtilsService.createPromiseDeferred();
                        var editorPromises = [];
                        for (var i = 0; i < payload.sections.length; i++) {
                            var section = payload.sections[i];
                            section.readyPromiseDeferred = UtilsService.createPromiseDeferred();
                            section.loadPromiseDeferred = UtilsService.createPromiseDeferred();
                            editorPromises.push(section.loadPromiseDeferred.promise);
                        }

                        loadRunTimeFieldTypeTemplates().then(function () {
                            for (var i = 0; i < payload.sections.length; i++) {
                                var section = payload.sections[i];
                                prepareExtendedRowsObject(section);
                            }
                            UtilsService.waitMultiplePromises(editorPromises).then(function () {
                                editorpromise.resolve();
                            }).catch(function (error) {
                                editorpromise.reject(error);
                            });
                        });
                        promises.push(editorpromise.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    var sections = {};
                    for (var i = 0; i < ctrl.sections.length; i++) {
                        var section = ctrl.sections[i];
                        sections = UtilsService.mergeObject(sections, section.rowsAPI.getData(), false);
                    }
                    return sections;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareExtendedRowsObject(section) {

                section.onSectionDirectiveReady = function (api) {
                    section.rowsAPI = api;
                    section.readyPromiseDeferred.resolve();
                };
                var payload = { context: getContext(), rows: section.Rows };
                if (section.readyPromiseDeferred != undefined) {
                    section.readyPromiseDeferred.promise.then(function () {
                        section.readyPromiseDeferred = undefined;

                        VRUIUtilsService.callDirectiveLoad(section.rowsAPI, payload, section.loadPromiseDeferred);
                    });
                }
                ctrl.sections.push(section);
            }

            function getContext() {
                var context = {
                    getRuntimeEditor: getRuntimeEditor,
                    getFieldPathValue: getFieldPathValue
                };
                return context;
            }

            function getFieldPathValue(fieldPath)
            {
                if (selectedValues != undefined && fieldPath != undefined)
                return selectedValues[fieldPath];
            }

            function getRuntimeEditor(configId)
            {
                if( $scope.fieldTypeConfigs != undefined)
                {
                    var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.fieldTypeConfigs, configId, 'ExtensionConfigurationId');
                    if (dataRecordFieldTypeConfig != undefined)
                        return dataRecordFieldTypeConfig.RuntimeEditor;
                }
            }

            function loadRunTimeFieldTypeTemplates() {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldTypeConfigs().then(function (response) {
                    if (response) {
                        $scope.fieldTypeConfigs = response;
                    }
                });
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);