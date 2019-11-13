//(function (app) {

//    'use strict';

//    DiscountRuleConditionCustomObjectRuntime.$inject = ['UtilsService', 'VRUIUtilsService', 'VR_GenericData_GenericBEDefinitionAPIService', 'VRCommon_VRComponentTypeAPIService', 'Retail_Billing_RatePlanAPIService','Retail_Billing_ChargeTypeAPIService'];

//    function DiscountRuleConditionCustomObjectRuntime(UtilsService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VRCommon_VRComponentTypeAPIService, Retail_Billing_RatePlanAPIService, Retail_Billing_ChargeTypeAPIService) {

//        var directiveDefinitionObject = {
//            restrict: 'E',
//            scope: {
//                onReady: '=',
//                isrequired: '=',
//                normalColNum: '@'
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: 'ctrl',
//            bindToController: true,
//            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingDiscountRule/Condition/Runtime/Templates/DiscountRuleConditionRuntimeTemplate.html"
//        };

//        function DiscountRuleConditionCustomObjectRuntimeDirectiveCtor(ctrl, $scope, attrs) {
//            this.initializeController = initializeController;
//            var businessEntityDefinitionId = "a92c2c45-a93f-4fb5-a73f-41c20931b650";
//            var dataRecordTypeId;
//            var fields;

//            var recordFilterDirectiveAPI;
//            var recordFilterDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onRecordFilterDirectiveReady = function (api) {
//                    recordFilterDirectiveAPI = api;
//                    recordFilterDirectiveReadyDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var fieldValue;
//                    var fieldType;
//                    var recordFilterGroup;
//                    var rootPromiseNode = { promises: [] };

                    


//                    if (payload != undefined) {
//                        fieldValue = payload.fieldValue;
//                        dataRecordTypeId = fieldValue.DataRecordTypeID != undefined && fieldValue.DataRecordTypeID != "00000000-0000-0000-0000-000000000000" ? fieldValue.DataRecordTypeID : undefined;
//                        recordFilterGroup = fieldValue.RecordFilterGroup;

//                        fieldType = payload.fieldType;

//                        $scope.scopeModel.fieldTitle = payload.fieldTitle;
//                    }

//                    if (dataRecordTypeId != undefined) {
//                        rootPromiseNode.promises.push(loadDataRecordTypeFields());
//                        rootPromiseNode.getChildNode = function () {
//                            return { promises: [loadRecordFilterDirective(recordFilterGroup)] };
//                        };
//                    }

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };


//                api.getData = function () {
//                    var recordFilter;
//                    var data = recordFilterDirectiveAPI.getData();

//                    if (data != undefined && data.filterObj != undefined) {
//                        recordFilter = data.filterObj;
//                    }

//                    return {
//                        $type: "Retail.Billing.MainExtensions.DiscountRuleCondition.DiscountRuleRecordFilterCondition, Retail.Billing.MainExtensions",
//                        RecordFilterGroup: recordFilter,
//                        DataRecordTypeID: dataRecordTypeId
//                    };
//                };

//                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
//                    var oldDataRecordTypeId = dataRecordTypeId;
//                    var newComponenetTypeId = allFieldValuesByFieldNames["TargetType"];

//                    if (newComponenetTypeId != undefined || (newComponenetTypeId == undefined && dataRecordTypeId != undefined))
//                        return getDataRecordTypeId(newComponenetTypeId).then(function () {
//                            if (newComponenetTypeId == undefined || oldDataRecordTypeId != dataRecordTypeId)
//                                UtilsService.waitPromiseNode({
//                                    promises: [loadDataRecordTypeFields()],
//                                    getChildNode: function () {
//                                        return { promises: [loadRecordFilterDirective()] };
//                                    }
//                                });
//                        });

//                    return UtilsService.waitPromiseNode({ promises: [] });
//                };

//                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
//                    ctrl.onReady(api);
//                }
//            }

//            function getDataRecordTypeId(componentTypeId) {
//                return VRCommon_VRComponentTypeAPIService.GetVRComponentType(componentTypeId).then(function (response) {

//                    if (response != undefined && response.Settings != undefined) {
//                        var extendedSettings = response.Settings.ExtendedSettings;
//                        if (extendedSettings != undefined)
//                            dataRecordTypeId = extendedSettings.TargetRecordTypeId;
//                    }
//                });
//            }

//            function loadDataRecordTypeFields() {
//                //Retail_Billing_RatePlanAPIService.EvaluateRecurringCharge({
//                //    RatePlanId: 10,
//                //    ContractService: {
//                //        ServiceType: "7918f7a5-39e6-40c0-aae5-52c1fb7dfdeb",
//                //        ServiceTypeOption: "606232a9-9378-4ed2-bfa1-e16ea5cd112d",
//                //        ChargeableCondition:"b5ff8ef1-52f6-497a-889f-ccb2dce73f27"
//                //    }
//                //});
//                //Retail_Billing_ChargeTypeAPIService.GetChargeDescription({
//                //    Charge: {
//                //        $type: "Retail.Billing.MainExtensions.RetailBillingCharge.RetailBillingCustomCodeCharge,Retail.Billing.MainExtensions",
//                //        RetailBillingChargeTypeId: "e2a105de-d822-417c-ba97-7c41dc99cd6f",
//                //        FieldValues: {
//                //        FixedVoiceTiers :[
//                //            {
//                //                "UpToVolume": 60,
//                //                "UnitPrice": 100
//                //            },
//                //            {
//                //                "UpToVolume": 200,
//                //                "UnitPrice": 80
//                //            },
//                //            {
//                //                "UpToVolume": null,
//                //                "UnitPrice": 60
//                //            }]
//                //        }
//                //    }
//                //});
            
//                //Retail_Billing_RatePlanAPIService.EvaluateActionCharge({
//                //    RatePlanId: 11, 
//                //    ChargeTargetContractServiceAction: {
//                //        ServiceType: "49dd51e5-a435-4605-b6ca-bec750d16eaa",
//                //        ServiceTypeOption: "ace71f0e-1fd2-4ea0-87f3-05c37f5b5f34",
//                //        ActionType:"46960154-3596-4aa5-b88b-1532f40baf5e"
//                //    }
//                //});


//                return VR_GenericData_GenericBEDefinitionAPIService.GetDataRecordTypeFields(dataRecordTypeId).then(function (response) {

//                    if (response != undefined) {
//                        fields = [];

//                        for (var fieldName in response) {
//                            var currentItem = response[fieldName];

//                            fields.push({
//                                FieldName: currentItem.Name,
//                                FieldTitle: currentItem.Title,
//                                Type: currentItem.Type
//                            });
//                        }
//                    }
//                });
//            }

//            function loadRecordFilterDirective(recordFilterGroup) {
//                var loadRecordFilterDirectiveDeferred = UtilsService.createPromiseDeferred();

//                recordFilterDirectiveReadyDeferred.promise.then(function () {

//                    var recordFilterDirectivePayload = {
//                        FilterGroup: recordFilterGroup,
//                        context: buildContext()
//                    };
//                    VRUIUtilsService.callDirectiveLoad(recordFilterDirectiveAPI, recordFilterDirectivePayload, loadRecordFilterDirectiveDeferred);
//                });

//                return loadRecordFilterDirectiveDeferred.promise;
//            }

//            function buildContext() {
//                var context = {
//                    getFields: function () {
//                        return fields;
//                    }
//                };
//                return context;
//            }
//        }

//        return directiveDefinitionObject;
//    }

//    app.directive('retailBillingDiscountruleconditionCustomobjectRuntime', DiscountRuleConditionCustomObjectRuntime);
//})(app);