"use strict";

app.directive("vrCommonConnectionstring", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ConnectionStringCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
            },
            templateUrl: "/Client/Modules/Common/Directives/ConnectionString/Templates/ConnectionStringTemplate.html"
        };

        function ConnectionStringCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var connectionStringType;

            function initializeController() {
                connectionStringType = {
                    ConnectionString: { value: 0, description: "Connection String" },
                    ConnectionStringNameAppSettingsName: { value: 1, description: "Connection String Name/AppSetting Name " }
                };

                $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
                $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                $scope.showConnectionString = true;
                $scope.showConnectionStringNameAppSettingsName = false;
                $scope.showConnectionStringNameAppSettings = false;

                $scope.onConnectionStringTypeSelectionChanged = function () {
                    if ($scope.selectedConnectionStringType != undefined) {

                        switch ($scope.selectedConnectionStringType.value) {
                            case connectionStringType.ConnectionString.value:
                                $scope.showConnectionString = true;
                                $scope.showConnectionStringNameAppSettingsName = false;
                                break;
                            case connectionStringType.ConnectionStringNameAppSettingsName.value:
                                $scope.showConnectionString = false;
                                $scope.showConnectionStringNameAppSettingsName = true;
                                if ($scope.connectionStringAppSettingName == undefined && $scope.connectionStringName == undefined) {
                                    $scope.showConnectionStringNameAppSettings = true;
                                }
                                else {
                                    $scope.showConnectionStringNameAppSettings = false;
                                }
                                break;
                        }
                    }
                };

                $scope.onConnectionStringNameOrAppSettingsValueChange = function (changedValue) {
                    if (changedValue != undefined) {
                        $scope.showConnectionStringNameAppSettings = false;
                    }
                    else {
                        if (($scope.connectionStringName == undefined || $scope.connectionStringName === "") && ($scope.connectionStringAppSettingName == undefined || $scope.connectionStringAppSettingName === "")) {
                            $scope.showConnectionStringNameAppSettings = true;
                        }
                        else {
                            $scope.showConnectionStringNameAppSettings = false;
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                       
                        $scope.connectionString = payload.ConnectionString;
                        $scope.connectionStringName = payload.ConnectionStringName;
                        $scope.connectionStringAppSettingName = payload.ConnectionStringAppSettingName;

                        if ($scope.connectionString != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                        } else if ($scope.connectionStringName != undefined || $scope.connectionStringAppSettingName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringNameAppSettingsName;
                        } 
                    }
                };

                api.getData = function () {
                    return {
                        ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined,
                        ConnectionStringName: $scope.showConnectionStringNameAppSettingsName ? $scope.connectionStringName : undefined,
                        ConnectionStringAppSettingName: $scope.showConnectionStringNameAppSettingsName ? $scope.connectionStringAppSettingName : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);