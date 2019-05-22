'use strict';

app.directive('businessprocessVrWorkflowactivityGetbusinessentitySettings', ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService',
    function (UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService) {

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
                        $scope.scopeModel.context = payload.context;
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
                                            if (dataRecordType != undefined) {
                                                outputGridAPI.clearDataSource();
                                                var fields = dataRecordType.Fields;
                                                for (var i = 0; i < fields.length; i++) {
                                                    var fieldName = fields[i].Name;
                                                    $scope.scopeModel.outputItems.push({
                                                        fieldName: fieldName,
                                                        outputValue: outputItems[fieldName]
                                                    });
                                                }
                                                return { promises: [] };
                                            }
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
                    if (field.outputValue != undefined) {
                        fields.push({
                            FieldName: field.fieldName,
                            Value: field.outputValue,
                        });
                    }
                }
                return fields;
            }

        }
        return directiveDefinitionObject;
    }]);