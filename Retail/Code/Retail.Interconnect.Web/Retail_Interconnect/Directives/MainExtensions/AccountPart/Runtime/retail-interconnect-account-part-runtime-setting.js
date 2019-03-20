'use strict';

app.directive('retailInterconnectAccountPartRuntimeSetting', ["UtilsService", "VRUIUtilsService", "Retail_Interconnect_RetailModuleService",
    function (UtilsService, VRUIUtilsService, Retail_Interconnect_RetailModuleService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountTypeSettingPartRuntime = new AccountTypeSettingPartRuntime($scope, ctrl, $attrs);
                accountTypeSettingPartRuntime.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Interconnect/Directives/MainExtensions/AccountPart/Runtime/Templates/AccountTypeSettingPartRuntimeTemplate.html'
        };

        function AccountTypeSettingPartRuntime($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var smsServiceTypesSelectorSelectorAPI;
            var smsServiceTypesSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isSMSModuleEnabled = Retail_Interconnect_RetailModuleService.isSMSModuleEnabled();

                $scope.scopeModel.onSmsServiceTypesSelectorReady = function (api) {
                    smsServiceTypesSelectorSelectorAPI = api;
                    smsServiceTypesSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                var partSettings;

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        partSettings = payload.partSettings;
                        if (payload.partSettings != undefined) {
                            $scope.scopeModel.representASwitch = partSettings.RepresentASwitch;
                        }
                    }
                    if ($scope.scopeModel.isSMSModuleEnabled) {
                        promises.push(loadSMSServiceTypesSelector());
                    }
                        
                    function loadSMSServiceTypesSelector() {
                        var loadSMSServiceTypespromise = UtilsService.createPromiseDeferred();
                        smsServiceTypesSelectorReadyPromiseDeferred.promise
                            .then(function () {
                                var sMSServiceTypesIds;
                                if (partSettings != undefined && partSettings.SMSServiceTypes != undefined) {
                                    sMSServiceTypesIds = [];
                                    for (var i = 0; i < partSettings.SMSServiceTypes.length; i++) {
                                        sMSServiceTypesIds.push(partSettings.SMSServiceTypes[i].SMSServiceTypeId);
                                    }
                                }
                                var selectorPayload = {
                                    selectedIds: sMSServiceTypesIds
                                };
                                VRUIUtilsService.callDirectiveLoad(smsServiceTypesSelectorSelectorAPI, selectorPayload, loadSMSServiceTypespromise);
                            });

                        return loadSMSServiceTypespromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var sMSServiceTypes;
                    var sMSServiceTypeIds = smsServiceTypesSelectorSelectorAPI != undefined ? smsServiceTypesSelectorSelectorAPI.getData() : undefined;
                    if (sMSServiceTypeIds != undefined) {
                        sMSServiceTypes = [];
                        for (var i = 0; i < sMSServiceTypeIds.length; i++) {
                            sMSServiceTypes.push({ SMSServiceTypeId: sMSServiceTypeIds[i] });
                        }
                    }
                    return {
                        $type: 'Retail.Interconnect.Business.AccountPartInterconnectSetting,Retail.Interconnect.Business',
                        RepresentASwitch: $scope.scopeModel.representASwitch,
                        SMSServiceTypes: sMSServiceTypes
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

