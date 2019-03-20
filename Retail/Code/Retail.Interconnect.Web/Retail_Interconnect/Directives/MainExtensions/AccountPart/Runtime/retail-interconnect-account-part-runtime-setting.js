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
            var selectorAPI;
            var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isSMSModuleEnabled = Retail_Interconnect_RetailModuleService.isSMSModuleEnabled();

                $scope.scopeModel.onSmsServiceTypesSelectorReady = function (api) {
                    selectorAPI = api;
                    selectorReadyPromiseDeferred.resolve();
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
                        }

                    }
                    function loadSelector(payloadEntity) {
                        var loadpromise = UtilsService.createPromiseDeferred();
                        var selectorPayload = {
                            SMSServiceTypeIds: payloadEntity.SMSServiceTypeIds
                        };
                        selectorReadyPromiseDeferred.promise
                            .then(function () {
                                VRUIUtilsService.callDirectiveLoad(selectorAPI, selectorPayload, loadpromise);
                            });

                        return loadpromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var sMSServiceTypeIdsObj = selectorAPI != undefined ? selectorAPI.getData() : undefined;
                    return {
                        $type: 'Retail.Interconnect.Business.AccountPartInterconnectSetting,Retail.Interconnect.Business',
                        RepresentASwitch: $scope.scopeModel.representASwitch,
                        SMSServiceTypeIds: sMSServiceTypeIdsObj != undefined ? sMSServiceTypeIdsObj.SMSServiceTypeIds : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);

