"use strict";

app.directive("retailBillingChargetypeCustomcode", ["UtilsService", "VRUIUtilsService", "VRNotificationService", "VR_GenericData_DataRecordFieldAPIService", "Retail_Billing_CustomCodeChargeTypeAPIService", "RetailBilling_CustomCodeChargeTypeService",
    function (UtilsService, VRUIUtilsService, VRNotificationService, VR_GenericData_DataRecordFieldAPIService, Retail_Billing_CustomCodeChargeTypeAPIService, RetailBilling_CustomCodeChargeTypeService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BillingChargeTypeSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Billing/Elements/Directives/RetailBillingChargeType/MainExtensions/Templates/CustomCodeChargeTypeTemplate.html"
        };

        function BillingChargeTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var chargeSettingsRecordTypeFields = [];
            var editorChargeSettingsRecordTypeFields = [];
            var priceLogicErrorMessages;
            var descriptionLogicErrorMessages;
            var context;

            var chargeSettingsRecordTypeSelectorAPI;
            var chargeSettingsRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chargeSettingsEditorDefinitionDirectiveApi;
            var chargeSettingsEditorDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onChargeSettingsRecordTypeSelectorReady = function (api) {
                    chargeSettingsRecordTypeSelectorAPI = api;
                    chargeSettingsRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onChargeSettingsEditorDefinitionDirectiveReady = function (api) {
                    chargeSettingsEditorDefinitionDirectiveApi = api;
                    chargeSettingsEditorDefinitionDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onChargeSettingsRecordTypeSelectorChanged = function (selectedRecordTypeId) {
                    if (selectedRecordTypeId == undefined)
                        return;

                    getChargeSettingsRecordTypeFields(selectedRecordTypeId.DataRecordTypeId).then(function () {

                        if (chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred != undefined) {
                            chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred.resolve();
                        }
                        else {
                            chargeSettingsEditorDefinitionDirectiveReadyDeferred.promise.then(function () {

                                var setLoader = function (value) { $scope.scopeModel.isLoading = value; };
                                var chargeSettingsEditorDefinitionDirectivePayload = { context: getContext() };

                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, chargeSettingsEditorDefinitionDirectiveApi, chargeSettingsEditorDefinitionDirectivePayload, setLoader);
                            });
                        }
                    });
                };

                $scope.scopeModel.onChargeSettingsRecordTypeDeselect = function (selectedRecordTypeId) {
                    chargeSettingsRecordTypeFields.length = 0;
                };

                $scope.scopeModel.tryCompileChargeTypeCustomCodePriceLogic = function () {
                    $scope.scopeModel.isCompilingPriceLogic = true;

                    var obj = {
                        TargetRecordTypeId: context.getTargetRecordTypeId(),
                        ChargeSettingsRecordTypeId: chargeSettingsRecordTypeSelectorAPI.getSelectedIds(),
                        PricingLogic: $scope.scopeModel.pricingLogic
                    };

                    return Retail_Billing_CustomCodeChargeTypeAPIService.TryCompileChargeTypeCustomCodePriceLogic(obj).then(function (response) {
                        $scope.scopeModel.isCompilingPriceLogic = false;
                        if (response != undefined)
                            priceLogicErrorMessages = response.ErrorMessages;

                        if (priceLogicErrorMessages == undefined || priceLogicErrorMessages.length == 0) {
                            VRNotificationService.showSuccess("Pricing Logic compiled successfully.");
                            return true;
                        }
                        else {
                            VRNotificationService.showWarning("Pricing Logic Compilation Failed.");
                            return false;
                        }
                    });
                };

                $scope.scopeModel.tryCompileChargeTypeCustomCodeDescriptionLogic = function () {
                    $scope.scopeModel.isCompilingDescriptionLogic = true;

                    var obj = {
                        ChargeSettingsRecordTypeId: chargeSettingsRecordTypeSelectorAPI.getSelectedIds(),
                        DescriptionLogic: $scope.scopeModel.descriptionLogic
                    };

                    return Retail_Billing_CustomCodeChargeTypeAPIService.TryCompileChargeTypeCustomCodeDescriptionLogic(obj).then(function (response) {
                        $scope.scopeModel.isCompilingDescriptionLogic = false;
                        if (response != undefined)
                            descriptionLogicErrorMessages = response.ErrorMessages;

                        if (descriptionLogicErrorMessages == undefined || descriptionLogicErrorMessages.length == 0) {
                            VRNotificationService.showSuccess("Description Logic compiled successfully.");
                            return true;
                        }
                        else {
                            VRNotificationService.showWarning("Description Logic Compilation Failed.");
                            return false;
                        }
                    });
                };

                $scope.scopeModel.priceLogicErrorsValidator = function () {
                    return getErrorMessages(priceLogicErrorMessages);
                };

                $scope.scopeModel.descriptionLogicErrorsValidator = function () {
                    return getErrorMessages(descriptionLogicErrorMessages);
                };

        
                $scope.scopeModel.openPriceLogicExpressionBuilder = function () {

                    var onSetValue = function (value) {
                        if (value != undefined && value.CodeExpression != undefined) {
                            $scope.scopeModel.pricingLogic = value.CodeExpression;
                        }
                    };

                    var targetRecordTypeFields = [];
                    context.getTargetRecordTypeFields(targetRecordTypeFields).then(function () {

                        var params = {
                            targetRecordTypeFields: targetRecordTypeFields,
                            chargeSettingsRecordTypeFields: chargeSettingsRecordTypeFields,
                            expression: getExpression($scope.scopeModel.pricingLogic),
                            label:"Pricing Logic"
                        };
                        RetailBilling_CustomCodeChargeTypeService.openExpressionEditorBuilder(onSetValue, params);
                    });
                };
                $scope.scopeModel.openDescriptionLogicExpressionBuilder = function () {

                    var onSetValue = function (value) {
                        if (value != undefined && value.CodeExpression != undefined) {
                            $scope.scopeModel.descriptionLogic = value.CodeExpression;
                        }
                    };

                    var params = {
                        chargeSettingsRecordTypeFields: chargeSettingsRecordTypeFields,
                        expression: getExpression($scope.scopeModel.descriptionLogic),
                        label: "Description Logic"
                    };

                    RetailBilling_CustomCodeChargeTypeService.openExpressionEditorBuilder(onSetValue, params);
                };

                function getExpression(expression) {
                    return {
                        CodeExpression: expression
                    };
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var extendedSettings;
                    var promises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        if (context != undefined) {


                            context.beforeSaveHandler = function () {
                                var promises = [];
                                promises.push($scope.scopeModel.tryCompileChargeTypeCustomCodePriceLogic());
                                promises.push($scope.scopeModel.tryCompileChargeTypeCustomCodeDescriptionLogic());

                                return UtilsService.waitPromiseNode({ promises: promises }).then(function () {
                                    if ((priceLogicErrorMessages == undefined || priceLogicErrorMessages.length == 0) && (descriptionLogicErrorMessages == undefined || descriptionLogicErrorMessages.length == 0))
                                        return true;
                                    return false;
                                });
                            };
                        }

                        extendedSettings = payload != undefined ? payload.extendedSettings : undefined;
                    }

                    promises.push(loadDataRecordTypeSelector());

                    if (extendedSettings != undefined) {
                        chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        $scope.scopeModel.pricingLogic = extendedSettings.PricingLogic;
                        $scope.scopeModel.descriptionLogic = extendedSettings.DescriptionLogic;

                        if (extendedSettings.ChargeSettingsRecordTypeId != undefined) {
                            promises.push(getChargeSettingsRecordTypeFields(extendedSettings.ChargeSettingsRecordTypeId));
                        }
                    };

                    function loadDataRecordTypeSelector() {
                        var chargeSettingsRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        chargeSettingsRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: extendedSettings != undefined ? extendedSettings.ChargeSettingsRecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(chargeSettingsRecordTypeSelectorAPI, payload, chargeSettingsRecordTypeSelectorLoadDeferred);
                        });
                        return chargeSettingsRecordTypeSelectorLoadDeferred.promise;
                    }

                    function loadChargeSettingsEditorDefinitionDirective() {
                        var loadChargeSettingsEditorDefinitionDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        chargeSettingsEditorDefinitionDirectiveReadyDeferred.promise.then(function () {
                            var editorPayload = {
                                settings: extendedSettings != undefined ? extendedSettings.ChargeSettingsEditorDefinition : undefined,
                                context: getContext()
                            };

                            VRUIUtilsService.callDirectiveLoad(chargeSettingsEditorDefinitionDirectiveApi, editorPayload, loadChargeSettingsEditorDefinitionDirectivePromiseDeferred);
                        });
                        return loadChargeSettingsEditorDefinitionDirectivePromiseDeferred.promise;
                    }

                    return UtilsService.waitPromiseNode({
                        promises: promises,
                        getChildNode: function () {
                            var promises = [];

                            if (extendedSettings != undefined && extendedSettings.ChargeSettingsRecordTypeId != undefined) {

                                var loadChargeSettingsEditorDefinitionDirectivePromise = loadChargeSettingsEditorDefinitionDirective();
                                promises.push(loadChargeSettingsEditorDefinitionDirectivePromise);
                            }

                            return {
                                promises: promises
                            };
                        }
                    }).then(function () {
                        chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred = undefined;
                    });
                };

                api.getData = function () {

                    return {
                        $type: "Retail.Billing.MainExtensions.RetailBillingChargeType.RetailBillingCustomCodeChargeType,Retail.Billing.MainExtensions",
                        PricingLogic: $scope.scopeModel.pricingLogic,
                        DescriptionLogic: $scope.scopeModel.descriptionLogic,
                        ChargeSettingsRecordTypeId: chargeSettingsRecordTypeSelectorAPI.getSelectedIds(),
                        ChargeSettingsEditorDefinition: chargeSettingsEditorDefinitionDirectiveApi != undefined ? chargeSettingsEditorDefinitionDirectiveApi.getData() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getChargeSettingsRecordTypeFields(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId, null).then(function (response) {
                    editorChargeSettingsRecordTypeFields.length = 0;
                    chargeSettingsRecordTypeFields.length = 0;

                    if (response != undefined) {
                        var filter = $scope.scopeModel.filterValue != undefined ? $scope.scopeModel.filterValue.toLowerCase() : "";

                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            editorChargeSettingsRecordTypeFields.push(currentField.Entity);

                            if (currentField.Entity != undefined) {
                                chargeSettingsRecordTypeFields.push({
                                    Title: currentField.Entity.Title,
                                    Value: "ChargeSettings." + currentField.Entity.Name,
                                    hideItem: currentField.Entity.Title.toLowerCase().indexOf(filter) == -1
                                });
                            }
                        }
                    }
                });
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                currentContext.hasItems = function () {
                    return $scope.scopeModel.accounts.length > 0;
                };

                currentContext.getDataRecordTypeId = function () {
                    return chargeSettingsRecordTypeSelectorAPI.getSelectedIds();
                };

                currentContext.getRecordTypeFields = function () {
                    var data = [];
                    for (var i = 0; i < editorChargeSettingsRecordTypeFields.length; i++) {
                        data.push(editorChargeSettingsRecordTypeFields[i]);
                    }
                    return data;
                };

                currentContext.getFields = function () {
                    var dataFields = [];

                    for (var i = 0; i < editorChargeSettingsRecordTypeFields.length; i++) {
                        dataFields.push({
                            FieldName: editorChargeSettingsRecordTypeFields[i].Name,
                            FieldTitle: editorChargeSettingsRecordTypeFields[i].Title,
                            Type: editorChargeSettingsRecordTypeFields[i].Type
                        });
                    }
                    return dataFields;
                };

                currentContext.getFieldType = function (fieldName) {
                    for (var i = 0; i < editorChargeSettingsRecordTypeFields.length; i++) {
                        var field = editorChargeSettingsRecordTypeFields[i];
                        if (field.Name == fieldName)
                            return field.Type;
                    }
                };

                return currentContext;
            }

            function getErrorMessages(errorMessages) {
                if (errorMessages == undefined || errorMessages.length == 0)
                    return null;

                var errorMessage = '';
                for (var i = 0; i < errorMessages.length; i++) {
                    errorMessage += (i + 1) + ') ' + errorMessages[i] + '\n';
                }
                return errorMessage;
            }
        }
        return directiveDefinitionObject;
    }
]);