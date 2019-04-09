'use strict';
app.directive('vrGenericdataFieldtypeDatarecordtypelistFieldviewRuntime', ['VRUIUtilsService', 'UtilsService', 'VR_GenericData_DataRecordFieldAPIService', 'VR_GenericData_DataRecordTypeService',
    function (VRUIUtilsService, UtilsService, VR_GenericData_DataRecordFieldAPIService, VR_GenericData_DataRecordTypeService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new fieldViewTypeListTypeCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/DataRecoedTypeList/RuntimeViewTypeRuntime/Templates/FieldViewTypeRuntimeTemplate.html';
            }
        };

        function fieldViewTypeListTypeCtor(ctrl, $scope) {

            $scope.scopeModel = {};
            var definitionSettings;
            var selectedValues;
            var fieldName;
            $scope.scopeModel.itemList = [];
            function initializeController() {

                $scope.scopeModel.isItemValid = function () {

                    var itemToAdd = $scope.scopeModel.itemToAdd;
                    if (itemToAdd == undefined || itemToAdd.length == 0 || itemToAdd == '') {
                        return "should add at least one element";
                    }
                    else {
                        for (var j = 0; j < $scope.scopeModel.itemList.length; j++) {
                            var listItem = $scope.scopeModel.itemList[j];
                            if (itemToAdd == listItem) {
                                return "item already exist";
                            }
                        }
                    }
                    return null;
                };
                $scope.scopeModel.addItem = function () {
                    $scope.scopeModel.itemList.push($scope.scopeModel.itemToAdd);
                    $scope.scopeModel.itemToAdd = undefined;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    definitionSettings = payload.definitionSettings != undefined ? payload.definitionSettings.RecordField : undefined;
                    fieldName = definitionSettings.Name;
                    selectedValues = payload.fieldValue;

                    if (selectedValues != undefined) {
                        for (var j = 0; j < selectedValues.length; j++) {
                            $scope.scopeModel.itemList.push(selectedValues[j][fieldName]);
                        }
                    }

                    $scope.scopeModel.fieldTitle = fieldName;
                    var rootPromiseNode = {
                        promises: []
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var returnedData = [];
                    for (var i = 0; i < $scope.scopeModel.itemList.length; i++) {
                        var listData = {};
                        listData[fieldName] = $scope.scopeModel.itemList[i];
                        returnedData.push(listData);
                    }
                    return returnedData;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
            this.initializeController = initializeController;
        }
    }]);