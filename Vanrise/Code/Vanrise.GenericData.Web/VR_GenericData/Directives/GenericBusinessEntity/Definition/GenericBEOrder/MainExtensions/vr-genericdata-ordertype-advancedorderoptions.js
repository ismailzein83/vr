"use strict";

app.directive("vrGenericdataOrdertypeAdvancedorderoptions", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_GenericBEDefinitionService","VR_GenericData_OrderDirectionEnum",
    function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_GenericBEDefinitionService, VR_GenericData_OrderDirectionEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericBEBulkActionUpdate($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEOrder/MainExtensions/Templates/AdvancedOrderTypeTemplate.html'

        };

        function GenericBEBulkActionUpdate($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            var dataRecordTypeFieldsSelectorAPI;
            var dataRecordTypeFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.selectedFields = [];
                ctrl.datasource = [];


                $scope.scopeModel.onDataRecordTypeFieldsSelectorDirectiveReady = function (api) {
                    dataRecordTypeFieldsSelectorAPI = api;
                    dataRecordTypeFieldsSelectorReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFieldSelected = function (item) {
                    ctrl.datasource.push({
                        FieldName: item.Name
                    });
                };

                $scope.scopeModel.onFieldDeselected = function (item) {
                    var index = UtilsService.getItemIndexByVal(ctrl.datasource, item.Name, "FieldName");
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.onDeleteField = function (dataItem) {
                    var index = ctrl.datasource.indexOf(dataItem);
                    var selectedFieldIndex = UtilsService.getItemIndexByVal($scope.scopeModel.selectedFields, dataItem.FieldName, "Name");
                    $scope.scopeModel.selectedFields.splice(selectedFieldIndex, 1);
                    ctrl.datasource.splice(index, 1);
                };

                $scope.scopeModel.deselectAllFields = function () {
                    ctrl.datasource.length = 0;
                };

                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    context = payload.context;
                    var dataRecordTypeId = context.getDataRecordTypeId();
                    var promises = [];
                    $scope.scopeModel.orderDirectionList = UtilsService.getArrayEnum(VR_GenericData_OrderDirectionEnum);
                    if (dataRecordTypeId != undefined) {
                        if (payload != undefined && payload.advancedOrderOptions != undefined && payload.advancedOrderOptions.Fields != undefined) {
                            var fields = payload.advancedOrderOptions.Fields;
                            selectedIds = [];
                            for (var i = 0; i < fields.length; i++) {
                                selectedIds.push(fields[i].FieldName);
                            }
                        }
                        var loadDataRecordTypeFieldsSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadDataRecordTypeFieldsSelectorPromiseDeferred.promise);

                        dataRecordTypeFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                            var fieldsPayload = {
                                dataRecordTypeId: dataRecordTypeId,
                                selectedIds: selectedIds
                            };
                            VRUIUtilsService.callDirectiveLoad(dataRecordTypeFieldsSelectorAPI, fieldsPayload, loadDataRecordTypeFieldsSelectorPromiseDeferred);
                        });
                    }
                    var rootPromiseNode = {
                        promises: promises     
                    };
                    rootPromiseNode.getChildNode = function () {
                        if (payload != undefined && payload.advancedOrderOptions != undefined && payload.advancedOrderOptions.Fields != undefined) {
                            var fields = payload.advancedOrderOptions.Fields;
                            for (var i = 0; i < fields.length; i++) {
                                var field = fields[i];
                                ctrl.datasource.push({
                                    FieldName: field.FieldName,
                                    OrderDirection: UtilsService.getItemByVal($scope.scopeModel.orderDirectionList, field.OrderDirection, "value")
                                });
                            }
                        }
                        return { promises: [] };
                    };
                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var fields = [];
                    for (var i = 0; i < ctrl.datasource.length; i++) {
                        var field = ctrl.datasource[i];
                        fields.push({
                            FieldName: field.FieldName,
                            OrderDirection: field.OrderDirection.value,
                        });
                    }
                    var data = {
                        $type: "Vanrise.GenericData.Entities.AdvancedFieldOrderOptions, Vanrise.GenericData.Entities ",
                        Fields: fields,
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

        }

        return directiveDefinitionObject;

    }
]);