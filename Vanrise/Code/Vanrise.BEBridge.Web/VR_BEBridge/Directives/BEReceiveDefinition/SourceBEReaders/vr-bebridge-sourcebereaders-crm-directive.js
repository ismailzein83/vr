'use strict';

app.directive('vrBebridgeSourcebereadersCrmDirective', ['VRNotificationService',
    function (vrNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ftpSource = new crmSourceReader($scope, ctrl, $attrs);
                ftpSource.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_BEBridge/Directives/BEReceiveDefinition/SourceBEReaders/Templates/BEReceiveDefinitionCRMSourceReadersTemplate.html'
        };

        function crmSourceReader($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.Fields = [];
                $scope.scopeModel.field = '';

                $scope.scopeModel.addEntityField = function () {
                    var fieldIsValid = true;

                    if ($scope.scopeModel.field == undefined || $scope.scopeModel.field.length == 0) {
                        fieldIsValid = false;
                    }
                    else {
                        angular.forEach($scope.scopeModel.Fields, function (item) {
                            if ($scope.scopeModel.field === item) {
                                fieldIsValid = false;
                            }
                        });
                    }

                    if (fieldIsValid)
                        $scope.scopeModel.Fields.push($scope.scopeModel.field);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    if (payload != undefined) {
                        $scope.scopeModel.EntityName = payload.Setting.EntityName;
                        $scope.scopeModel.Fields = payload.Setting.Fields;
                        $scope.scopeModel.BaseAddress = payload.Setting.BaseAddress;
                        $scope.scopeModel.UserName = payload.Setting.UserName;
                        $scope.scopeModel.Password = payload.Setting.Password;
                        $scope.scopeModel.TopRecords = payload.Setting.TopRecords;
                    }
                };
                api.getData = function () {
                    var setting =
                    {
                        EntityName: $scope.scopeModel.EntityName,
                        Fields: $scope.scopeModel.Fields,
                        BaseAddress: $scope.scopeModel.BaseAddress,
                        UserName: $scope.scopeModel.UserName,
                        Password: $scope.scopeModel.Password,
                        TopRecords: $scope.scopeModel.TopRecords
                    };
                    return {
                        $type: "Vanrise.BEBridge.MainExtensions.SourceBEReaders.MicrosoftCRMSourceReader,  Vanrise.BEBridge.MainExtensions",
                        Setting: setting
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
