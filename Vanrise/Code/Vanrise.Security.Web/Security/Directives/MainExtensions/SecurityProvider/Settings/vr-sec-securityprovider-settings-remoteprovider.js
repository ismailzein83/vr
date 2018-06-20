app.directive("vrSecSecurityproviderSettingsRemoteprovider", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
                normalColNum: '@'
            },

            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RemoteSecurityProvider($scope, ctrl, $attrs);
                ctor.initializeController();
            },

            controllerAs: "remoteCtrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Security/Directives/MainExtensions/SecurityProvider/Settings/Templates/RemoteProviderTemplate.html"
        };

        function RemoteSecurityProvider($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var vrConnectionSelectorAPI;
            var vrConnectionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.isConnectionDisabled = false;

                $scope.scopeModel.onVRConnectionSelectorReady = function (api) {
                    vrConnectionSelectorAPI = api;
                    vrConnectionSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var connectionId;
                    var promises = [];

                    if (payload != undefined && payload.securityProviderEntity != undefined) {
                        connectionId = payload.securityProviderEntity.VRConnectionId;
                        if (connectionId != undefined) {
                            $scope.scopeModel.isConnectionDisabled = true;
                        }
                    }

                    var vrConnectionSelectorLoadPromise = getVRConnectionSelectorLoadPromise();
                    promises.push(vrConnectionSelectorLoadPromise);

                    function getVRConnectionSelectorLoadPromise() {
                        var vrConnectionSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                        vrConnectionSelectorReadyDeferred.promise.then(function () {
                            var connectionSelectorPayload;
                            if (connectionId != undefined) {
                                connectionSelectorPayload = { selectedIds: connectionId };
                            }
                            VRUIUtilsService.callDirectiveLoad(vrConnectionSelectorAPI, connectionSelectorPayload, vrConnectionSelectorLoadPromiseDeferred);
                        });
                        return vrConnectionSelectorLoadPromiseDeferred.promise;
                    };


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Security.MainExtensions.SecurityProvider.RemoteSecurityProvider,Vanrise.Security.MainExtensions",
                        VRConnectionId: vrConnectionSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);