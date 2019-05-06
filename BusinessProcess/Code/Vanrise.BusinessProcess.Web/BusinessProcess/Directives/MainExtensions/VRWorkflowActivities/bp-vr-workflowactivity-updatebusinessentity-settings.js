'use strict';

app.directive('businessprocessVrWorkflowactivityUpdatebusinessentitySettings', ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService',
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
            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityUpdateBusinessEntitySettingsTemplate.html'
        };

        function addBusinessEntitySettings(ctrl, $scope, $attrs) {

            var inputItems;
            var inputGridAPI;
            var inputGridPromiseReadyDefferd = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.inputItems = [];

                $scope.scopeModel.onInputGridReady = function (api) {
                    inputGridAPI = api;
                    inputGridPromiseReadyDefferd.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var rootPromiseNode = {};
                    inputItems = {};
                    $scope.scopeModel.isSucceeded = undefined;
                    $scope.scopeModel.userId = undefined;
                    var genericBEDefinitionSettings;
                    var dataRecordType;
                    if (payload != undefined) {
                        $scope.scopeModel.context = payload.context;
                        var settings = payload.settings;

                        if (settings != undefined) {
                            $scope.scopeModel.isSucceeded = settings.IsSucceeded;
                            $scope.scopeModel.userId = settings.UserId;

                            if (settings.InputItems != undefined) {
                                var items = settings.InputItems;
                                for (var i = 0; i < items.length; i++) {
                                    var inputItem = items[i];
                                    inputItems[inputItem.FieldName] = inputItem.Value;
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
                                                inputGridAPI.clearDataSource();
                                                var fields = dataRecordType.Fields;
                                                for (var i = 0; i < fields.length; i++) {
                                                    var fieldName = fields[i].Name;
                                                    $scope.scopeModel.inputItems.push({
                                                        fieldName: fieldName,
                                                        inputValue: inputItems[fieldName]
                                                    });
                                                }
                                                return { promises: [] };
                                            }
                                        }
                                    };
                                }
                            };
                        }
                        else {
                            rootPromiseNode.promises = [];
                        }
                    }
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities.VRWorkflowUpdateGenericBEActivity, Vanrise.BusinessProcess.MainExtensions",
                        IsSucceeded: $scope.scopeModel.isSucceeded,
                        UserId: $scope.scopeModel.userId,
                        InputItems: $scope.scopeModel.inputItems.length > 0 ? getInputFields() : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getInputFields() {
                var fields = [];
                for (var i = 0; i < $scope.scopeModel.inputItems.length; i++) {
                    var field = $scope.scopeModel.inputItems[i];
                    if (field.inputValue != undefined) {
                        fields.push({
                            FieldName: field.fieldName,
                            Value: field.inputValue,
                        });
                    }
                }
                return fields;
            }

        }
        return directiveDefinitionObject;
    }]);