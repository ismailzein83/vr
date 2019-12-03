(function (app) {

    'use strict';

    DiscountRuleRecordFilterCondition.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VRCommon_VRComponentTypeAPIService', 'Retail_Billing_ChargeTypeAPIService'];

    function DiscountRuleRecordFilterCondition(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VRCommon_VRComponentTypeAPIService, Retail_Billing_ChargeTypeAPIService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/Condition/Runtime/MainExtensions/Templates/DiscountRuleRecordFilterConditionTemplate.html"
        };

        function DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var componentTypeID;
            var dataRecordTypeId;
            var fields;

            var recordFilterDirectiveAPI;
            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
                    recordFilterDirectiveAPI = api;
                    recordFilterDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.validateEditRecordFilter = function () {
                    if (fields == null || fields.length == 0)
                        return true;
                    return false;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var recordFilterGroup;

                    if (payload != undefined) {
                        var oldFieldValuesByName = payload.allFieldValuesByName;
                        var componentTypeIDs = oldFieldValuesByName["TargetType"];
                        componentTypeID = componentTypeIDs != undefined ? componentTypeIDs[0] : undefined;

                        var fieldValue = payload.fieldValue;
                        if (fieldValue != undefined) {
                            dataRecordTypeId = fieldValue.DataRecordTypeID;
                            recordFilterGroup = fieldValue.RecordFilterGroup;
                        }

                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
                    }

                    var rootPromiseNode = { promises: [] };

                    if (dataRecordTypeId != undefined) {
                        rootPromiseNode.promises.push(loadDataRecordTypeFields());
                        rootPromiseNode.getChildNode = function () {
                            return { promises: [loadRecordFilterDirective(recordFilterGroup)] };
                        };
                    }

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {

                    var recordFilterGroup;

                    var data = recordFilterDirectiveAPI.getData();
                    if (data != undefined && data.filterObj != undefined) {
                        recordFilterGroup = data.filterObj;
                    }

                    if (dataRecordTypeId == null || recordFilterGroup == null)
                        return null;

                    return {
                        $type: "Retail.Billing.MainExtensions.DiscountRuleCondition.DiscountRuleRecordFilterCondition, Retail.Billing.MainExtensions",
                        RecordFilterGroup: recordFilterGroup,
                        DataRecordTypeID: dataRecordTypeId
                    };
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {

                    var oldComponentTypeID = componentTypeID;
                    var oldDataRecordTypeId = dataRecordTypeId;

                    var componentTypeIDs = allFieldValuesByFieldNames["TargetType"];
                    componentTypeID = componentTypeIDs != undefined ? componentTypeIDs[0] : undefined;

                    if (componentTypeID == oldComponentTypeID) {
                        return UtilsService.waitPromiseNode({ promises: [] });
                    }

                    if (componentTypeID == undefined) {
                        fields = [];
                        dataRecordTypeId = undefined;
                        return UtilsService.waitPromiseNode({ promises: [loadRecordFilterDirective()] });
                    }

                    return UtilsService.waitPromiseNode({
                        promises: [getDataRecordTypeId(componentTypeID)],
                        getChildNode: function () {
                            if (oldDataRecordTypeId != dataRecordTypeId) {
                                return {
                                    promises: [loadDataRecordTypeFields()],
                                    getChildNode: function () {
                                        return { promises: [loadRecordFilterDirective()] };
                                    }
                                };
                            }
                            else {
                                return { promises: [loadRecordFilterDirective()] };
                            }
                        }
                    });
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getDataRecordTypeId(componentTypeId) {
                return VRCommon_VRComponentTypeAPIService.GetVRComponentType(componentTypeId).then(function (response) {
                    if (response != undefined) {
                        var settings = response.Settings;

                        if (settings != undefined)
                            dataRecordTypeId = settings.TargetRecordTypeId;
                    }
                });
            }

            function loadDataRecordTypeFields() {
                return VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFields(dataRecordTypeId).then(function (response) {
                    if (response != undefined) {
                        fields = [];

                        for (var fieldName in response) {
                            var currentItem = response[fieldName];

                            fields.push({
                                FieldName: currentItem.Name,
                                FieldTitle: currentItem.Title,
                                Type: currentItem.Type
                            });
                        }
                    }
                });
            }

            function loadRecordFilterDirective(recordFilterGroup) {
                var loadRecordFilterDirectiveDeferred = UtilsService.createPromiseDeferred();

                recordFilterDirectiveReadyDeferred.promise.then(function () {

                    var recordFilterDirectivePayload = {
                        FilterGroup: recordFilterGroup,
                        context: buildContext()
                    };
                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, loadRecordFilterDirectiveDeferred);
                });

                return loadRecordFilterDirectiveDeferred.promise;
            }

            function buildContext() {
                var context = {
                    getFields: function () {
                        return fields;
                    }
                };
                return context;
            }
        }

        return directiveDefinitionObject;
    }

    app.directive('retailBillingDiscountruleRecordfiltercondition', DiscountRuleRecordFilterCondition);
})(app);