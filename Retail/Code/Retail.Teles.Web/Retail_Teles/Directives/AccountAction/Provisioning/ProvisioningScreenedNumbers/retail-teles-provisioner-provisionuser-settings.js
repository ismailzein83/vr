﻿(function (app) {

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
                                accountGenericFieldNames.push(provisionerDefinitionSettings.FirstNameField);
                                accountGenericFieldNames.push(provisionerDefinitionSettings.LoginNameField);
                                accountGenericFieldNames.push(provisionerDefinitionSettings.LastNameField);
                            }

                            return Retail_BE_AccountTypeAPIService.GetAccountGenericFieldValues(accountBEDefinitionId, accountId, UtilsService.serializetoJson(accountGenericFieldNames)).then(function (response) {
                                if (provisionerDefinitionSettings != undefined && response != undefined) {
                                    $scope.scopeModel.firstName = response[provisionerDefinitionSettings.FirstNameField];
                                    $scope.scopeModel.lastName = response[provisionerDefinitionSettings.LastNameField];
                                    $scope.scopeModel.loginName = response[provisionerDefinitionSettings.LoginNameField];
                                    if ($scope.scopeModel.loginName != undefined)
                                    {
                                        $scope.scopeModel.loginName = $scope.scopeModel.loginName.toLowerCase();
                                        $scope.scopeModel.loginName = UtilsService.trim($scope.scopeModel.loginName, ' ');
                                        $scope.scopeModel.loginName =  UtilsService.replaceAll($scope.scopeModel.loginName,' ','');
                                    }
                                }
                            });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

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
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerProvisionuserSettings', ProvisionerUsersettingsDirective);

})(app);