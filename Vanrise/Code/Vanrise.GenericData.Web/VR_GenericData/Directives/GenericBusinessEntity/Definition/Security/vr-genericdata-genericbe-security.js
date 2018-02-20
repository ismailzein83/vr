(function (app) {

    'use strict';

    GenericdataGenericbeSecurity.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function GenericdataGenericbeSecurity(UtilsService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new GenericdataGenericbeSecurityCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/Security/Templates/GenericBeSecurityTemplate.html'
        };

        function GenericdataGenericbeSecurityCtor($scope, ctrl) {
            this.initializeController = initializeController;
            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var editPermissionAPI;
            var editPermissionReadyDeferred = UtilsService.createPromiseDeferred();

         

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    addPermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onEditRequiredPermissionReady = function (api) {
                    editPermissionAPI = api;
                    editPermissionReadyDeferred.resolve();
                };
               
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var loadViewRequiredPermissionPromise = loadViewRequiredPermission();
                    promises.push(loadViewRequiredPermissionPromise);

                    var loadAddRequiredPermissionPromise = loadAddRequiredPermission();
                    promises.push(loadAddRequiredPermissionPromise);

                    var loadEditRequiredPermissionPromise = loadEditRequiredPermission();
                    promises.push(loadEditRequiredPermissionPromise);

                   
                    function loadViewRequiredPermission() {
                        var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.ViewRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                        });
                        return viewSettingPermissionLoadDeferred.promise;
                    }
                 

                    function loadAddRequiredPermission() {
                        var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data:payload && payload.AddRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addPermissionAPI, dataPayload, addPermissionLoadDeferred);
                        });
                        return addPermissionLoadDeferred.promise;
                    }

                    function loadEditRequiredPermission() {
                        var editPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        editPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data:payload && payload.EditRequiredPermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(editPermissionAPI, dataPayload, editPermissionLoadDeferred);
                        });
                        return editPermissionLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                

                api.getData = function () {
                    return {
                        ViewRequiredPermission: viewPermissionAPI.getData(),
                        AddRequiredPermission: addPermissionAPI.getData(),
                        EditRequiredPermission: editPermissionAPI.getData()
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrGenericdataGenericbeSecurity', GenericdataGenericbeSecurity);

})(app);