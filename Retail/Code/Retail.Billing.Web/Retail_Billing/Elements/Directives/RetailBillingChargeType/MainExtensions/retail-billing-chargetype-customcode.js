"use strict";

app.directive("retailBillingChargetypeCustomcode", ["UtilsService", "VRUIUtilsService", "VR_GenericData_DataRecordFieldAPIService", "Retail_Billing_CustomCodeChargeTypeAPIService",
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService, Retail_Billing_CustomCodeChargeTypeAPIService) {

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
            var errorMessages;
            var context;

            var targetRecordTypeSelectorAPI;
            var targetRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chargeSettingsRecordTypeSelectorAPI;
            var chargeSettingsRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var chargeSettingsEditorDefinitionDirectiveApi;
            var chargeSettingsEditorDefinitionDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
            var chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred;

            var textAreaAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.filterValue = "";
                $scope.scopeModel.chargeSettingsRecordTypeFields = [];
                $scope.scopeModel.targetRecordTypeFields = [];

                $scope.scopeModel.onTextAreaReady = function (api) {
                    textAreaAPI = api;
                };

                $scope.scopeModel.onTargetRecordTypeSelectorReady = function (api) {
                    targetRecordTypeSelectorAPI = api;
                    targetRecordTypeSelectorReadyDeferred.resolve();
                };

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

                $scope.scopeModel.onTargetRecordTypeSelectorChanged = function (selectedRecordTypeId) {
                    if (selectedRecordTypeId == undefined)
                        return;

                    getTargetRecordTypeFields(selectedRecordTypeId.DataRecordTypeId);
                };

                $scope.scopeModel.insertText = function (item) {
                    if (textAreaAPI != undefined) {
                        textAreaAPI.appendAtCursorPosition(item.Value);
                    }
                };

                $scope.scopeModel.filterValueChanged = function (value) {
                    if (value == undefined || value.length == 0) {
                        setHideItemsFalse($scope.scopeModel.chargeSettingsRecordTypeFields);
                        setHideItemsFalse($scope.scopeModel.targetRecordTypeFields);
                        return;
                    }

                    var filter = value.toLowerCase();

                    var targetRecordTypeFieldsLength = $scope.scopeModel.targetRecordTypeFields.length;

                    for (var i = 0; i < targetRecordTypeFieldsLength; i++) {
                        var variable = $scope.scopeModel.targetRecordTypeFields[i].Title.toLowerCase();
                        if (variable.indexOf(filter) == -1)
                            $scope.scopeModel.targetRecordTypeFields[i].hideItem = true;
                        else
                            $scope.scopeModel.targetRecordTypeFields[i].hideItem = false;
                    }

                    var chargeSettingsRecordTypeFieldsLength = $scope.scopeModel.chargeSettingsRecordTypeFields.length;

                    for (var j = 0; j < chargeSettingsRecordTypeFieldsLength; j++) {
                        var argument = $scope.scopeModel.chargeSettingsRecordTypeFields[j].Title.toLowerCase();
                        if (argument.indexOf(filter) == -1)
                            $scope.scopeModel.chargeSettingsRecordTypeFields[j].hideItem = true;
                        else
                            $scope.scopeModel.chargeSettingsRecordTypeFields[j].hideItem = false;
                    }
                };

                $scope.scopeModel.tryCompileChargeTypeCustomCode = function () {

                    var obj = {
                        TargetRecordTypeId: targetRecordTypeSelectorAPI.getSelectedIds(),
                        ChargeSettingsRecordTypeId: chargeSettingsRecordTypeSelectorAPI.getSelectedIds(),
                        PricingLogic: $scope.scopeModel.pricingLogic
                    };

                    return Retail_Billing_CustomCodeChargeTypeAPIService.TryCompileChargeTypeCustomCode(obj).then(function (response) {
                        if (response != undefined)
                            errorMessages = response.ErrorMessages;
                        return !(errorMessages != undefined && errorMessages.length > 0);
                    });
                };

                $scope.scopeModel.hasErrorsValidator = function () {
                    if (errorMessages == undefined || errorMessages.length == 0)
                        return null;

                    var errorMessage = '';
                    for (var i = 0; i < errorMessages.length; i++) {
                        errorMessage += (i + 1) + ') ' + errorMessages[i] + '\n';
                    }
                    return errorMessage;
                };

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
                            context.beforeSaveHandler = $scope.scopeModel.tryCompileChargeTypeCustomCode;
                        }

                        extendedSettings = payload != undefined ? payload.extendedSettings : undefined;
                    }

                    promises.push(loadTargetRecordTypeSelector());
                    promises.push(loadDataRecordTypeSelector());

                    if (extendedSettings != undefined) {
                        chargeSettingsEditorDefinitionDirectiveSelectedPromiseDeferred = UtilsService.createPromiseDeferred();
                        $scope.scopeModel.pricingLogic = extendedSettings.PricingLogic;

                        if (extendedSettings.ChargeSettingsRecordTypeId != undefined) {
                            promises.push(getChargeSettingsRecordTypeFields(extendedSettings.ChargeSettingsRecordTypeId));
                        }

                        if (extendedSettings.TargetRecordTypeId != undefined) {
                            promises.push(getTargetRecordTypeFields(extendedSettings.TargetRecordTypeId));
                        }
                    };

                    function loadTargetRecordTypeSelector() {
                        var targetRecordTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        targetRecordTypeSelectorReadyDeferred.promise.then(function () {
                            var payload = {
                                selectedIds: extendedSettings != undefined ? extendedSettings.TargetRecordTypeId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(targetRecordTypeSelectorAPI, payload, targetRecordTypeSelectorLoadDeferred);
                        });
                        return targetRecordTypeSelectorLoadDeferred.promise;
                    }

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

                    var obj = {
                        $type: "Retail.Billing.MainExtensions.RetailBillingChargeType.RetailBillingCustomCodeChargeType,Retail.Billing.MainExtensions",
                        TargetRecordTypeId: targetRecordTypeSelectorAPI.getSelectedIds(),
                        PricingLogic: $scope.scopeModel.pricingLogic,
                        ChargeSettingsRecordTypeId: chargeSettingsRecordTypeSelectorAPI.getSelectedIds(),
                        ChargeSettingsEditorDefinition: chargeSettingsEditorDefinitionDirectiveApi != undefined ? chargeSettingsEditorDefinitionDirectiveApi.getData() : undefined
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getTargetRecordTypeFields(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId, null).then(function (response) {
                    $scope.scopeModel.targetRecordTypeFields.length = 0;

                    if (response != undefined) {
                        var filter = $scope.scopeModel.filterValue != undefined ? $scope.scopeModel.filterValue.toLowerCase() : "";

                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];

                            if (currentField.Entity != undefined)
                                $scope.scopeModel.targetRecordTypeFields.push({
                                    Title: currentField.Entity.Title,
                                    Value: "Target." + currentField.Entity.Name,
                                    hideItem: currentField.Entity.Title.toLowerCase().indexOf(filter) == -1
                                });
                        }
                    }
                });
            }

            function getChargeSettingsRecordTypeFields(recordTypeId) {
                return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo(recordTypeId, null).then(function (response) {
                    chargeSettingsRecordTypeFields.length = 0;
                    $scope.scopeModel.chargeSettingsRecordTypeFields.length = 0;

                    if (response != undefined) {
                        var filter = $scope.scopeModel.filterValue != undefined ? $scope.scopeModel.filterValue.toLowerCase() : "";

                        for (var i = 0; i < response.length; i++) {
                            var currentField = response[i];
                            chargeSettingsRecordTypeFields.push(currentField.Entity);

                            if (currentField.Entity != undefined) {
                                $scope.scopeModel.chargeSettingsRecordTypeFields.push({
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
                    for (var i = 0; i < chargeSettingsRecordTypeFields.length; i++) {
                        data.push(chargeSettingsRecordTypeFields[i]);
                    }
                    return data;
                };

                currentContext.getFields = function () {
                    var dataFields = [];

                    for (var i = 0; i < chargeSettingsRecordTypeFields.length; i++) {
                        dataFields.push({
                            FieldName: chargeSettingsRecordTypeFields[i].Name,
                            FieldTitle: chargeSettingsRecordTypeFields[i].Title,
                            Type: chargeSettingsRecordTypeFields[i].Type
                        });
                    }
                    return dataFields;
                };

                currentContext.getFieldType = function (fieldName) {
                    for (var i = 0; i < chargeSettingsRecordTypeFields.length; i++) {
                        var field = chargeSettingsRecordTypeFields[i];
                        if (field.Name == fieldName)
                            return field.Type;
                    }
                };

                return currentContext;
            }

            function setHideItemsFalse(objectsList) {

                if (objectsList == undefined || objectsList.length == 0)
                    return;

                for (var i = 0; i < objectsList.length; i++)
                    objectsList[i].hideItem = false;
            }
        }
        return directiveDefinitionObject;
    }
]);