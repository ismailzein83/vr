(function (app) {

    'use strict';

    ProvisionerUsersettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService','Retail_BE_AccountTypeAPIService'];

    function ProvisionerUsersettingsDirective(UtilsService, VRUIUtilsService, Retail_BE_AccountTypeAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProvisionerUsersettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/ProvisioningScreenedNumbers/Templates/ProvisionUserSettingsTemplate.html"

        };
        function ProvisionerUsersettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.pin = '0000';
                $scope.scopeModel.showBusinessTrunkFields = true;
                $scope.scopeModel.maximumRegistrations = 3;
                $scope.scopeModel.validatePinNumber = function () {
                    if($scope.scopeModel.pin != undefined && $scope.scopeModel.pin.length == 4)
                    {
                        return null;
                    }
                    return "Pin should be in the form of XXXX";
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var provisionerDefinitionSettings;
                    var provisionerRuntimeSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        var accountBEDefinitionId = payload.accountBEDefinitionId;
                        var accountId = payload.accountId;
                        provisionerDefinitionSettings = payload.provisionerDefinitionSettings;
                        provisionerRuntimeSettings = payload.provisionerRuntimeSettings;
                        if (provisionerDefinitionSettings != undefined) {
                            $scope.scopeModel.loginPassword = provisionerDefinitionSettings.LoginPassword;
                            $scope.scopeModel.pin = provisionerDefinitionSettings.Pin;
                            promises.push(getAccountGenericFieldValues());
                        }
                        if (provisionerRuntimeSettings != undefined)
                        {
                            $scope.scopeModel.loginPassword = provisionerRuntimeSettings.LoginPassword;
                            $scope.scopeModel.pin = provisionerRuntimeSettings.Pin;
                            $scope.scopeModel.firstName = provisionerRuntimeSettings.FirstName;
                            $scope.scopeModel.lastName = provisionerRuntimeSettings.LastName;
                            $scope.scopeModel.loginName = provisionerRuntimeSettings.LoginName;
                        }
                        function getAccountGenericFieldValues() {
                            var accountGenericFieldNames = [];
                            if (provisionerDefinitionSettings != undefined) {
                                if (provisionerDefinitionSettings.FirstNameField != undefined)
                                    accountGenericFieldNames.push(provisionerDefinitionSettings.FirstNameField);
                                if (provisionerDefinitionSettings.LoginNameField != undefined)
                                    accountGenericFieldNames.push(provisionerDefinitionSettings.LoginNameField);
                                if (provisionerDefinitionSettings.LastNameField != undefined)
                                    accountGenericFieldNames.push(provisionerDefinitionSettings.LastNameField);
                            }
                            if (accountGenericFieldNames.length > 0)
                            {
                                return Retail_BE_AccountTypeAPIService.GetAccountGenericFieldValues(accountBEDefinitionId, accountId, UtilsService.serializetoJson(accountGenericFieldNames)).then(function (response) {
                                    if (provisionerDefinitionSettings != undefined && response != undefined) {
                                        if (provisionerDefinitionSettings.FirstNameField != undefined)
                                            $scope.scopeModel.firstName = response[provisionerDefinitionSettings.FirstNameField];
                                        if (provisionerDefinitionSettings.LoginNameField != undefined)
                                            $scope.scopeModel.loginName = response[provisionerDefinitionSettings.LoginNameField];
                                        if (provisionerDefinitionSettings.LastNameField != undefined)
                                            $scope.scopeModel.lastName = response[provisionerDefinitionSettings.LastNameField];

                                        if ($scope.scopeModel.loginName != undefined) {
                                            $scope.scopeModel.loginName = $scope.scopeModel.loginName.toLowerCase();
                                            $scope.scopeModel.loginName = UtilsService.trim($scope.scopeModel.loginName, ' ');
                                            $scope.scopeModel.loginName = UtilsService.replaceAll($scope.scopeModel.loginName, ' ', '');
                                        }
                                    }
                                });
                            }
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                api.showBusinessTrunkFields = function (value) {
                    $scope.scopeModel.showBusinessTrunkFields = value;
                    if(!value)
                    {
                        $scope.scopeModel.maximumRegistrations = undefined;
                        $scope.scopeModel.concurrentCalls = undefined;
                    }else
                    {
                        $scope.scopeModel.maximumRegistrations = 3;
                    }
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
              
                function getData() {
                    var data = {
                        FirstName: $scope.scopeModel.firstName,
                        LastName: $scope.scopeModel.lastName,
                        LoginName: $scope.scopeModel.loginName,
                        LoginPassword: $scope.scopeModel.loginPassword,
                        Pin: $scope.scopeModel.pin,
                        MaxRegistrations: $scope.scopeModel.maximumRegistrations,
                        MaxCalls: $scope.scopeModel.concurrentCalls
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerProvisionuserSettings', ProvisionerUsersettingsDirective);

})(app);