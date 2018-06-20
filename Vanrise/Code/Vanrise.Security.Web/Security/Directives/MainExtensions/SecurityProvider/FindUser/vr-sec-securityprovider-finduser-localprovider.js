'use strict';

app.directive('vrSecSecurityproviderFinduserLocalprovider', ['UtilsService', 'VR_Sec_SecurityAPIService',
    function (UtilsService, VR_Sec_SecurityAPIService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new LocalproviderCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/MainExtensions/SecurityProvider/FindUser/Templates/LocalProviderTemplate.html"
        };

        function LocalproviderCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var connectionId;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.showPasswordSection = true;
                $scope.scopeModel.isEditMode = false;

                $scope.scopeModel.validatePasswords = function () {
                    if ($scope.password != $scope.confirmedPassword)
                        return 'Passwords do not match';
                    return null;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var securityProviderId;
                    var context;

                    if (payload != undefined) {
                        context = payload.context;
                        securityProviderId = payload.securityProviderId;
                        $scope.scopeModel.email = payload.email;
                        $scope.scopeModel.enablePasswordExpiration = payload.enablePasswordExpiration;

                        if ($scope.scopeModel.email != undefined) {
                            $scope.scopeModel.isEditMode = true;
                            $scope.scopeModel.showPasswordSection = false;
                        }
                    }

                    if (context != undefined) {
                        connectionId = context.connectionId;
                    }

                    var loadPasswordHintPromise = loadPasswordHint();
                    if (loadPasswordHintPromise != null)
                        promises.push(loadPasswordHintPromise);

                    function loadPasswordHint() {
                        if ($scope.scopeModel.isEditMode)
                            return null;

                        if (connectionId == undefined) {
                            return VR_Sec_SecurityAPIService.GetPasswordValidationInfo(securityProviderId).then(function (response) {
                                $scope.scopeModel.passwordHint = response.RequirementsMessage;
                                $scope.scopeModel.requiredPassword = response.RequiredPassword;
                            });
                        }
                        else {
                            var input = { VRConnectionId: connectionId, SecurityProviderId: securityProviderId };
                            return VR_Sec_SecurityAPIService.GetRemotePasswordValidationInfo(input).then(function (response) {
                                $scope.scopeModel.passwordHint = response.RequirementsMessage;
                                $scope.scopeModel.requiredPassword = response.RequiredPassword;
                            });
                        }
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {
                    return {
                        email: $scope.scopeModel.email,
                        password: $scope.scopeModel.password,
                        enablePasswordExpiration: $scope.scopeModel.enablePasswordExpiration
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }
]);