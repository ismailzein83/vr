(function (app) {

    'use strict';
    DataStoreSettingRDBDirective.$inject = ['UtilsService'];

    function DataStoreSettingRDBDirective(UtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var directiveConstructor = new DirectiveConstructor($scope, ctrl);
                directiveConstructor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {
                    }
                };
            },
            templateUrl: function (element, attrs) {
                return "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStoreSetting/Templates/DataStoreSettingsRDBTemplate.html";
            }
        };
        function DirectiveConstructor($scope, ctrl) {

            this.initializeController = initializeController;

            var connectionStringType;

            function initializeController() {
                connectionStringType = {
                    ConnectionString: { value: 0, description: "Connection String" },
                    ConnectionStringNameAppSettings: { value: 1, description: "Connection String Name/AppSettingName" },
                };

                $scope.connectionStringType = UtilsService.getArrayEnum(connectionStringType);
                $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                $scope.showConnectionString = true;
                $scope.showConnectionStringNameAppSettings = false;

                $scope.showConnectionStringName = false;
                $scope.showConnectionStringAppSettingsName = false;

                $scope.onConnectionStringTypeSelectionChanged = function () {
                    if ($scope.selectedConnectionStringType != undefined) {

                        switch ($scope.selectedConnectionStringType.value) {
                            case connectionStringType.ConnectionString.value:
                                $scope.showConnectionString = true;
                                $scope.showConnectionStringNameAppSettings = false;
                                $scope.showConnectionStringName = false;
                                $scope.showConnectionStringAppSettingsName = false;
                                break;
                            case connectionStringType.ConnectionStringNameAppSettings.value:
                                $scope.showConnectionStringNameAppSettings = true;
                                $scope.showConnectionString = false;
                                if ($scope.connectionStringAppSettingName == undefined) {
                                    $scope.showConnectionStringName = true;
                                }
                                else {
                                    $scope.showConnectionStringName = false;
                                }
                                if ($scope.connectionStringName == undefined) {
                                    $scope.showConnectionStringAppSettingsName = true;
                                }
                                else {
                                    $scope.showConnectionStringAppSettingsName = false;
                                }
                                break;
                        }
                    }
                };

                $scope.onConnectionStringNameValueChange = function (changedValue) {
                    if (changedValue != undefined && $scope.connectionStringAppSettingName == undefined) {
                        $scope.showConnectionStringAppSettingsName = false;
                    }
                    if (changedValue == undefined) {
                        if ($scope.connectionStringAppSettingName != undefined) {
                            $scope.showConnectionStringName = false;
                        }
                        else {
                            $scope.showConnectionStringAppSettingsName = true;
                            $scope.showConnectionStringName = true;
                        }
                    }
                };

                $scope.onConnectionStringAppSettingsValueChange = function (changedValue) {
                    if (changedValue != undefined && $scope.connectionStringName == undefined) {
                        $scope.showConnectionStringName = false;
                    }
                    if (changedValue == undefined) {
                        if ($scope.connectionStringName != undefined) {
                            $scope.showConnectionStringAppSettingsName = false;
                        }
                        else {
                            $scope.showConnectionStringName = true;
                            $scope.showConnectionStringAppSettingsName = true;
                        }
                    }
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.RDBDataStoreSettings, Vanrise.GenericData.RDBDataStorage",
                        ConnectionStringName: $scope.showConnectionStringNameAppSettings ? $scope.connectionStringName : undefined,
                        ConnectionString: $scope.showConnectionString ? $scope.connectionString : undefined,
                        ConnectionStringAppSettingName: $scope.showConnectionStringNameAppSettings ? $scope.connectionStringAppSettingName : undefined
                    };
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.data != undefined) {

                        $scope.connectionStringName = payload.data.ConnectionStringName;
                        $scope.connectionString = payload.data.ConnectionString;
                        $scope.connectionStringAppSettingName = payload.data.ConnectionStringAppSettingName;

                        if ($scope.connectionStringName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringNameAppSettings;
                        } else if ($scope.connectionString != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionString;
                        } else if ($scope.connectionStringAppSettingName != undefined) {
                            $scope.selectedConnectionStringType = connectionStringType.ConnectionStringNameAppSettings;
                        }
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
    app.directive('vrGenericdataDatastoresettingRdb', DataStoreSettingRDBDirective);
})(app);