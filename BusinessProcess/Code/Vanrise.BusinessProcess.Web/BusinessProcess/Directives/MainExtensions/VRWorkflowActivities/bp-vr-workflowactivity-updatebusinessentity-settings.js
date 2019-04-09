//'use strict';

//app.directive('businessprocessVrWorkflowactivityUpdatebusinessentitySettings', ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VR_GenericData_DataRecordTypeAPIService', 'VR_GenericData_GenericBEDefinitionAPIService',
//    function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordTypeAPIService, VR_GenericData_GenericBEDefinitionAPIService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                remove: '='
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new addBusinessEntitySettings(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: '/Client/Modules/BusinessProcess/Directives/MainExtensions/VRWorkflowActivities/Templates/VRWorkflowActivityUpdateBusinessEntitySettingsTemplate.html'
//        };

//        function addBusinessEntitySettings(ctrl, $scope, $attrs) {

//            var inputItems = [];

//            this.initializeController = initializeController;
//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.fields = [];
//                $scope.scopeModel.inputItems = [];

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var rootPromiseNode = {};
//                    var genericBEDefinitionSettings;
//                    var fieldsInfo;

//                    $scope.scopeModel.context = payload.context;
//                    if (payload.businessEntityDefinitionId != undefined) {
//                        var beDefinitionId = payload.businessEntityDefinitionId;
//                        var promise = VR_GenericData_GenericBEDefinitionAPIService.GetGenericBEDefinitionSettings(beDefinitionId).then(function (response) {
//                            genericBEDefinitionSettings = response;
//                        });
//                        rootPromiseNode.promises = [promise];
//                        rootPromiseNode.getChildNode = function () {
//                            if (genericBEDefinitionSettings != undefined && genericBEDefinitionSettings.DataRecordTypeId != undefined) {
//                                var dataRecordTypeId = genericBEDefinitionSettings.DataRecordTypeId;
//                                var childPromise = VR_GenericData_DataRecordTypeAPIService.GetDataRecordType(dataRecordTypeId).then(function (response) {
//                                    fieldsInfo = response;
//                                });
//                                return {
//                                    promises: [childPromise],
//                                    getChildNode: function () {
//                                        if (fieldsInfo != undefined) {
//                                            gridAPI.clearDataSource();

//                                            for (var i = 0; i < fieldsInfo.Fields.length; i++) {
//                                                var inputItem = {
//                                                    payload: fieldsInfo.Fields[i],

//                                                };

//                                                addInputGrid(inputItem);


//                                            }


//                                            return { promises: [] };
//                                        }
//                                    }
//                                };
//                            }
//                        }
//                    }
//                    else {
//                        rootPromiseNode.promises = [];
//                    }
//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    return {
//                        IsSucceeded: $scope.scopeModel.isSucceeded,
//                        InputItems: $scope.scopeModel.inputItems.length > 0 ? getInputFields() : null,
//                    };
//                };
//                function addInputGrid(inputItem) {
//                    var dataItem = {
//                        id: $scope.scopeModel.inputItems.length + 1,
//                        fieldName: inputItem.payload.Name,
//                    };

//                    if (inputItems.length > 0) {
//                        var item = inputItems.find(x => x.FieldName === dataItem.fieldName);
//                        if (item != undefined) {
//                            dataItem.inputValue = item.Value;
//                        }
//                    }
//                    $scope.scopeModel.inputItems.push(dataItem);
//                }

//                function getInputFields() {
//                    var fields = [];
//                    for (var i = 0; i < $scope.scopeModel.inputItems.length; i++) {
//                        var field = $scope.scopeModel.inputItems[i];
//                        if (field.inputValue != undefined) {
//                            fields.push({
//                                FieldName: field.fieldName,
//                                Value: field.inputValue,
//                            });
//                        }
//                    }
//                    return fields;
//                }
//                if (ctrl.onReady != null)
//                    ctrl.onReady(api);
//            }


//        }
//        return directiveDefinitionObject;
//    }]);