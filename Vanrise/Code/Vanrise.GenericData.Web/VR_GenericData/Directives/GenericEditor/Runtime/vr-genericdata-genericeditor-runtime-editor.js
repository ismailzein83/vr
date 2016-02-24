'use strict';
app.directive('vrGenericdataGenericeditorRuntimeEditor', ['UtilsService','VRUIUtilsService','VR_GenericData_DataRecordFieldTypeConfigAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldTypeConfigAPIService) {

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
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/GenericEditor/Runtime/Templates/EditorTemplate.html';
            }

        };

        function EditorCtor(ctrl, $scope) {
            function initializeController() {
                ctrl.sections = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                 
                    if (payload.sections != undefined) {
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
                }

                api.getData = function () {
                    return {
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function prepareExtendedRowsObject(section) {

                section.onSectionDirectiveReady = function (api) {
                    section.rowsAPI = api;
                    section.readyPromiseDeferred.resolve();
                }
                var payload = { context: getContext(), rows: section.Rows }
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
                    getRuntimeEditor: getRuntimeEditor
                };
                return context;
            }

            function getRuntimeEditor(configId)
            {
                if( $scope.fieldTypeConfigs != undefined)
                {
                    var dataRecordFieldTypeConfig = UtilsService.getItemByVal($scope.fieldTypeConfigs, configId, 'DataRecordFieldTypeConfigId');
                    if (dataRecordFieldTypeConfig != undefined)
                        return dataRecordFieldTypeConfig.RuntimeEditor;
                }
            }

            function loadRunTimeFieldTypeTemplates() {
                return VR_GenericData_DataRecordFieldTypeConfigAPIService.GetDataRecordFieldTypes().then(function (response) {
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