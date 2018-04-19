'use strict';

app.directive('vrCommonCommunicatorsettingsFtp', ['UtilsService', 'VRUIUtilsService', 'VRCommon_FTPTypeEnum',
    function (UtilsService, VRUIUtilsService, VRCommon_FTPTypeEnum) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ftpSettingCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ftpCtrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Common/Directives/CommunicatorSettings/Templates/FTPCommunicatorSettings.html"
        };

        function ftpSettingCtor(ctrl, $scope, $attrs) {

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.ftpTypes = UtilsService.getArrayEnum(VRCommon_FTPTypeEnum);

                    if (payload != undefined && payload.ftpCommunicatorSettings != undefined) {
                        $scope.scopeModel.selectedFtpType = UtilsService.getItemByVal($scope.scopeModel.ftpTypes, payload.ftpCommunicatorSettings.FTPType, 'value');
                        $scope.scopeModel.directory = payload.ftpCommunicatorSettings.Directory;
                        $scope.scopeModel.serverIP = payload.ftpCommunicatorSettings.ServerIP;
                        $scope.scopeModel.username = payload.ftpCommunicatorSettings.Username;
                        $scope.scopeModel.password = payload.ftpCommunicatorSettings.Password;
                    }
                    else {
                        $scope.scopeModel.selectedFtpType = $scope.scopeModel.ftpTypes[0];
                    }
                };

                api.getData = function () {
                    return {
                        FTPType: $scope.scopeModel.selectedFtpType.value,
                        Directory: $scope.scopeModel.directory,
                        ServerIP: $scope.scopeModel.serverIP,
                        Username: $scope.scopeModel.username,
                        Password: $scope.scopeModel.password
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);