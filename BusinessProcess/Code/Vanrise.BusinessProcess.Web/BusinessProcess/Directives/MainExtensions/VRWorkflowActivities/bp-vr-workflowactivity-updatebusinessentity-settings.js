'use strict';

app.directive('businessprocessVrWorkflowactivityUpdatebusinessentitySettings', ['UtilsService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VRUIUtilsService','VR_Sec_UserService',
    function (UtilsService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService, VRUIUtilsService, VR_Sec_UserService) {

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

            var context;
            var settings; 
            var isSucceededExpressionBuilderDirectiveAPI;
            var isSucceededExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var userIdExpressionBuilderDirectiveAPI;
            var userIdExpressionBuilderPromiseReadyDeffered = UtilsService.createPromiseDeferred();

            var inputGridPromiseReadyDefferd = UtilsService.createPromiseDeferred();

            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.fields = [];
                $scope.scopeModel.inputItems = [];

                $scope.scopeModel.onIsSucceededExpressionBuilderDirectiveReady = function (api) {
                    isSucceededExpressionBuilderDirectiveAPI = api;
                    isSucceededExpressionBuilderPromiseReadyDeffered.resolve();
                };
                $scope.scopeModel.onUserIdExpressionBuilderDirectiveReady = function (api) {
                    userIdExpressionBuilderDirectiveAPI = api;
                    userIdExpressionBuilderPromiseReadyDeffered.resolve();
                };
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
                    var genericBEDefinitionSettings;
                    var dataRecordType;
                    if (payload != undefined) {
                        context = payload.context;
                           settings = payload.settings;

                        if (settings != undefined) {
                            $scope.scopeModel.isSucceeded = settings.IsSucceeded;

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
                                            var promises = [];

                                            if (dataRecordType != undefined) {
                                                inputGridAPI.clearDataSource();
                                                var fields = dataRecordType.Fields;
                                                for (var i = 0; i < fields.length; i++) {
                                                    var fieldObject = {
                                                        payload: fields[i],
                                                        inputValueExpressionBuilderPromiseReadyDeffered: UtilsService.createPromiseDeferred(),
                                                        inputValueExpressionBuilderPromiseLoadDeffered: UtilsService.createPromiseDeferred()
                                                    };
                                                    promises.push(fieldObject.inputValueExpressionBuilderPromiseLoadDeffered.promise);
                                                    prepareField(fieldObject);
                                                }
                                            }

                                            function prepareField(fieldObject) {
                                                var dataItem = {
                                                    entity: { fieldName: fieldObject.payload.Name }
                                                };
                                                dataItem.onInputValueExpressionBuilderDirectiveReady = function (api) {
                                                    dataItem.inputValueExpressionBuilderDirectiveAPI = api;
                                                    fieldObject.inputValueExpressionBuilderPromiseReadyDeffered.resolve();
                                                };
                                                fieldObject.inputValueExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                                                    var payload = {
                                                        context: context,
                                                        value: inputItems[fieldObject.payload.Name],
                                                        fieldEntity: {
                                                            fieldType: fieldObject.payload.Type,
                                                            fieldTitle: fieldObject.payload.Title
                                                        }
                                                    };
                                                    VRUIUtilsService.callDirectiveLoad(dataItem.inputValueExpressionBuilderDirectiveAPI, payload, fieldObject.inputValueExpressionBuilderPromiseLoadDeffered);
                                                });
                                                $scope.scopeModel.inputItems.push(dataItem);
                                            }

                                            promises.push(loadIsSucceededExpressionBuilder());
                                          
                                            promises.push(loadUserIdExpressionBuilder());

                                            return { promises: promises };
                                        }

                                    };
                                }
                            };
                        }
                        else {
                            rootPromiseNode.promises = [];
                        }
                        return UtilsService.waitPromiseNode(rootPromiseNode);

                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities.VRWorkflowUpdateGenericBEActivity, Vanrise.BusinessProcess.MainExtensions",
                        IsSucceeded: isSucceededExpressionBuilderDirectiveAPI != undefined ? isSucceededExpressionBuilderDirectiveAPI.getData() : undefined,
                        UserId: userIdExpressionBuilderDirectiveAPI != undefined ? userIdExpressionBuilderDirectiveAPI.getData() : undefined,
                    InputItems: $scope.scopeModel.inputItems.length > 0 ? getInputFields() : undefined,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadIsSucceededExpressionBuilder() {
                var isSucceededExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                isSucceededExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.IsSucceeded : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(isSucceededExpressionBuilderDirectiveAPI, payload, isSucceededExpressionBuilderPromiseLoadDeffered);
                });
                return isSucceededExpressionBuilderPromiseLoadDeffered.promise;
            }
            function loadUserIdExpressionBuilder() {

                var userIdExpressionBuilderPromiseLoadDeffered = UtilsService.createPromiseDeferred();
                userIdExpressionBuilderPromiseReadyDeffered.promise.then(function () {
                    var payload = {
                        context: context,
                        value: settings != undefined ? settings.UserId : undefined,
                        fieldEntity: {
                            fieldType: VR_Sec_UserService.getUserIdFieldType(),
                            fieldTitle:"User"
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(userIdExpressionBuilderDirectiveAPI, payload, userIdExpressionBuilderPromiseLoadDeffered);
                });
                return userIdExpressionBuilderPromiseLoadDeffered.promise;
            }
            function getInputFields() {

                var fields = [];
                for (var i = 0; i < $scope.scopeModel.inputItems.length; i++) {
                    var field = $scope.scopeModel.inputItems[i];
                    addField(field, fields);
                }
                return fields;
            }
            function addField(field, fields) {

                if (field.inputValueExpressionBuilderDirectiveAPI != undefined) {

                    var objValue = field.inputValueExpressionBuilderDirectiveAPI.getData();
                    if (objValue != undefined)
                        fields.push({
                            FieldName: field.entity.fieldName,
                            Value: objValue,
                        });
                }

            }
        }
        return directiveDefinitionObject;
    }]);