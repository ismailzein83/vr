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
                            if ($scope.scopeModel.isSMSModuleEnabled)
                                promises.push(loadSelector(partSettings))
                        };
                    };

                    function loadSelector(payloadEntity) {
                        var loadpromise = UtilsService.createPromiseDeferred();
                        var sMSServiceTypesIds = [];
                        var sMSServiceTypes = payloadEntity.SMSServiceTypes;

                        if (sMSServiceTypes != undefined)
                            for (var i = 0; i < sMSServiceTypes.length; i++) {
                                sMSServiceTypesIds.push(sMSServiceTypes[i].SMSServiceTypeId);
                            };

                        var selectorPayload = {
                            SMSServiceTypeIds: sMSServiceTypesIds
                        };

                        smsServiceTypesSelectorReadyPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(smsServiceTypesSelectorSelectorAPI, selectorPayload, loadpromise);
                            });

                        return loadpromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var sMSServiceTypes = [];
                    var sMSServiceTypeIdsObj = smsServiceTypesSelectorSelectorAPI != undefined ? smsServiceTypesSelectorSelectorAPI.getData() : undefined;

                    if (sMSServiceTypeIdsObj != undefined) {
                        var sMSServiceTypeIds = sMSServiceTypeIdsObj.SMSServiceTypeIds;
                        if (sMSServiceTypeIds != undefined)
                            for (var i = 0; i < sMSServiceTypeIds.length; i++) {
                                sMSServiceTypes.push({ SMSServiceTypeId: sMSServiceTypeIds[i] });
                            };
                    };

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

