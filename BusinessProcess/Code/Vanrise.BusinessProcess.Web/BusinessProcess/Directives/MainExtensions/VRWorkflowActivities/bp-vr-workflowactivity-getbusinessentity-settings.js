'use strict';

app.directive('businessprocessVrWorkflowactivityGetbusinessentitySettings', ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService','VRUIUtilsService',
    function (UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                remove: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new addBusinessEntitySettings(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityGetBusinessEntitySettingsTemplate.html'
        };

        function addBusinessEntitySettings(ctrl, $scope, $attrs) {

            var outputItems;
            var outputGridAPI;
            var outputGridPromiseReadyDefferd = UtilsService.createPromiseDeferred();
            var context;
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.outputItems = [];

                $scope.scopeModel.onOutputGridReady = function (api) {
                    outputGridAPI = api;
                    outputGridPromiseReadyDefferd.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = {};
                    outputItems = {};
                    var genericBEDefinitionSettings;
                    var dataRecordType;
                    if (payload != undefined) {
                        context = payload.context;
                        var settings = payload.settings;

                        if (settings != undefined) {

                            if (settings.OutputItems != undefined) {
                                var items = settings.OutputItems;
                                for (var i = 0; i < items.length; i++) {
                                    var outputItem = items[i];
                                    outputItems[outputItem.FieldName] = outputItem.Value;
                                }
                            }
                        }

                        if (payload.businessEntityDefinitionId != undefined) {
                            var beDefinitionId = payload.businessEntityDefinitionId;
                            var promise = VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
                                genericBEDefinitionSettings = response;
                            });
                            rootPromiseNode.promises = [promise];
                            rootPromiseNode.getChildNode = function () {
                                if (genericBEDefinitionSettings != undefined && genericBEDefinitionSettings.DataRecordTypeId != undefined) {
                                    var dataRecordTypeId = genericBEDefinitionSettings.DataRecordTypeId;
                                    var childPromise = VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
                                        dataRecordType = response;
                                    });
                                    return {
                                        promises: [childPromise],
                                        getChildNode: function () {
                                            var promises = [];
                                            if (dataRecordType != undefined) {
                                                outputGridAPI.clearDataSource();
                                                var fields = dataRecordType.Fields;
                                                for (var i = 0; i < fields.length; i++) {
                                                    var fieldObject = {
                                                        payload: fields[i],
                                                        outputValueExpressionBuilderPromiseReadyDeffered: UtilsService.createPromiseDeferred(),
                                                        outputValueExpressionBuilderPromiseLoadDeffered: UtilsService.createPromiseDeferred()
                                                    };
                                                    promises.push(fieldObject.outputValueExpressionBuilderPromiseLoadDeffered.promise);
                                                    prepareField(fieldObject);
                                                }
                                            }
                                            function prepareField(fieldObject) {
                                                var dataItem = {
                                                    entity: { fieldName: fieldObject.payload.Name }
                                                };
                                                dataItem.onOutputValueExpressionBuilderDirectiveReady = function (api) {
                                                    dataItem.outputValueExpressionBuilderDirectiveAPI = api;
                                                    fieldObject.outputValueExpressionBuilderPromiseReadyDeffered.resolve();
                                                };
                                                fieldObject.outputValueExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                                                    var payload = {
                                                        context: context,
                                                        value: outputItems[fieldObject.payload.Name],
                                                    };
                                                    VRUIUtilsService.callDirectiveLoad(dataItem.outputValueExpressionBuilderDirectiveAPI, payload, fieldObject.outputValueExpressionBuilderPromiseLoadDeffered);
                                                });
                                                $scope.scopeModel.outputItems.push(dataItem);
                                            }

                                            return { promises: promises };
                                        }
                                    };
                                }
                            }
                        }
                        else {
                            rootPromiseNode.promises = [];
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities.VRWorkflowGetGenericBEActivity, Vanrise.BusinessProcess.MainExtensions",
                        OutputItems: $scope.scopeModel.outputItems.length > 0 ? getOutputFields() : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getOutputFields() {
                var fields = [];
                for (var i = 0; i < $scope.scopeModel.outputItems.length; i++) {
                    var field = $scope.scopeModel.outputItems[i];
                    var objValue = field.outputValueExpressionBuilderDirectiveAPI.getData();
                    if (objValue != undefined)
                        fields.push({
                            FieldName: field.entity.fieldName,
                            Value: objValue,
                        });
                }
                return fields;
            }
        }
        return directiveDefinitionObject;
    }]);