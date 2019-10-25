(function (app) {

    'use strict';

    BillingChargeTypeExtendedSettingsDirective.$inject = ['Retail_Billing_ChargeTypeAPIService', 'UtilsService', 'VRUIUtilsService', 'VR_GenericData_DataRecordFieldAPIService'];

    function BillingChargeTypeExtendedSettingsDirective(Retail_Billing_ChargeTypeAPIService, UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BillingChargeTypeExtendedSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function BillingChargeTypeExtendedSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectorAPI;
            var context;

            var targetRecordTypeSelectorAPI;
            var targetRecordTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var directiveAPI;
            var directiveReadyDeferred;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.templateConfigs = [];
                $scope.scopeModel.selectedTemplateConfig;

                $scope.scopeModel.onTargetRecordTypeSelectorReady = function (api) {
                    targetRecordTypeSelectorAPI = api;
                    targetRecordTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onSelectorReady = function (api) {
                    selectorAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.onDirectiveReady = function (api) {
                    directiveAPI = api;
                    var directivePayload = {
                        context: getContext()
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    selectorAPI.clearDataSource();
                    var extendedSettings;

                    if (payload != undefined) {
                        context = payload.context;
                        extendedSettings = payload.extendedSettings;
                    }

                    var promises = [];

                    if (extendedSettings != undefined) {
                        var loadDirectivePromise = loadDirective();
                        promises.push(loadDirectivePromise);
                    }

                    promises.push(loadTargetRecordTypeSelector());

                    var getChargeTypeExtendedSettingsConfigsPromise = getChargeTypeExtendedSettingsConfigs();
                    promises.push(getChargeTypeExtendedSettingsConfigsPromise);

                    function getChargeTypeExtendedSettingsConfigs() {
                        return Retail_Billing_ChargeTypeAPIService.GetChargeTypeExtendedSettingsConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.templateConfigs.push(response[i]);
                                }
                                if (extendedSettings != undefined) {
                                    $scope.scopeModel.selectedTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.templateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                            }
                        });
                    }

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

                    function loadDirective() {
                        var directiveLoadDeferred = UtilsService.createPromiseDeferred();
                        directiveReadyDeferred = UtilsService.createPromiseDeferred();

                        directiveReadyDeferred.promise.then(function () {
                            directiveReadyDeferred = undefined;
                            var directivePayload = {
                                extendedSettings: extendedSettings,
                                context: getContext()
                            };
                            VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        });
                        return directiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data;

                    if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined)
                        data = directiveAPI.getData();

                    if (data != undefined) {
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                        data.TargetRecordTypeId = targetRecordTypeSelectorAPI.getSelectedIds();
                        return data;
                    }
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};

                currentContext.getTargetRecordTypeId = function () {
                    return targetRecordTypeSelectorAPI.getSelectedIds();
                };

                currentContext.getTargetRecordTypeFields = function (targetRecordTypeFields) {
                    if ($scope.scopeModel.selectedTargetRecordType != undefined) {
                        return VR_GenericData_DataRecordFieldAPIService.GetDataRecordFieldsInfo($scope.scopeModel.selectedTargetRecordType.DataRecordTypeId, null).then(function (response) {
                            if (response != undefined) {
                                var filter = $scope.scopeModel.filterValue != undefined ? $scope.scopeModel.filterValue.toLowerCase() : "";

                                for (var i = 0; i < response.length; i++) {
                                    var currentField = response[i];

                                    if (currentField.Entity != undefined)
                                        targetRecordTypeFields.push({
                                            Title: currentField.Entity.Title,
                                            Value: "Target." + currentField.Entity.Name,
                                            hideItem: currentField.Entity.Title.toLowerCase().indexOf(filter) == -1
                                        });
                                }
                            }
                        });
                    }
                    else {
                        var getTargetRecordTypeFieldsPromise = UtilsService.createPromiseDeferred();
                        getTargetRecordTypeFieldsPromise.resolve();
                        return getTargetRecordTypeFieldsPromise.promise;
                    }
                };
                return currentContext;
            };
        }

        function getTemplate(attrs) {
            var label = "label='Charge Type'";
            if (attrs.hidelabel != undefined) {
                label = "";
            }
            var template =
                '<vr-row > '
                + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + '<vr-genericdata-datarecordtype-selector on-ready="scopeModel.onTargetRecordTypeSelectorReady" label="Target Record Type" showaddbutton '
                + ' selectedvalues="scopeModel.selectedTargetRecordType">'
                + '</vr-genericdata-datarecordtype-selector> '
                + '</vr-columns>'
                + '<vr-columns colnum="{{ctrl.normalColNum}}">'
                + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                + ' datasource="scopeModel.templateConfigs"'
                + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                + ' datavaluefield="ExtensionConfigurationId"'
                + ' datatextfield="Title"'
                + label
                + ' isrequired="true"'
                + 'hideremoveicon>'
                + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
                + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.DefinitionEditor" '
                + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" '
                + 'isrequired="ctrl.isrequired" customvalidate="ctrl.customvalidate"></vr-directivewrapper>';

            return template;
        }
    }

    app.directive('retailBillingChargetypeExtendedsettings', BillingChargeTypeExtendedSettingsDirective);
})(app);