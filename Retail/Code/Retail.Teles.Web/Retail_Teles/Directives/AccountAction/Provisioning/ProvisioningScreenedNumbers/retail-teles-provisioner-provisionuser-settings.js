(function (app) {

    'use strict';

    ProvisionerUsersettingsDirective.$inject = ["UtilsService", 'VRUIUtilsService'];

    function ProvisionerUsersettingsDirective(UtilsService, VRUIUtilsService) {
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
                    console.log(payload);

                    var provisionUserSettings;
                    if (payload != undefined) {
                        mainPayload = payload;
                        provisionUserSettings = payload.provisionUserSettings;
                        if (provisionUserSettings != undefined) {
                            $scope.scopeModel.centrexFeatSet = provisionUserSettings.CentrexFeatSet;
                           
                            if (provisionUserSettings.UserAccountSetting != undefined)
                            {
                                $scope.scopeModel.firstName = provisionUserSettings.UserAccountSetting.FirstName;
                                $scope.scopeModel.lastName = provisionUserSettings.UserAccountSetting.LastName;
                                $scope.scopeModel.loginName = provisionUserSettings.UserAccountSetting.LoginName;
                                $scope.scopeModel.loginPassword = provisionUserSettings.UserAccountSetting.LoginPassword;
                                $scope.scopeModel.pin = provisionUserSettings.UserAccountSetting.Pin;
                            }
                     
                        }

                    }

                    var promises = [];

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        UserAccountSetting:{
                            FirstName: $scope.scopeModel.firstName,
                            LastName: $scope.scopeModel.lastName,
                            LoginName: $scope.scopeModel.loginName,
                            LoginPassword: $scope.scopeModel.loginPassword,
                            Pin: $scope.scopeModel.pin,
                        }
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailTelesProvisionerProvisionuserSettings', ProvisionerUsersettingsDirective);

})(app);