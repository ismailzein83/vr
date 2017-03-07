(function (app) {

    'use strict';

    AccountpayloadDefinitionManagementDirective.$inject = ['UtilsService', 'VRNotificationService', 'VRUIUtilsService'];

    function AccountpayloadDefinitionManagementDirective(UtilsService, VRNotificationService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountpayloadDefinitionManagementCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/AccountSecurityDefinition/Templates/AccountSecurityDefinitionManagementTemplate.html'
        };

        function AccountpayloadDefinitionManagementCtor($scope, ctrl) {
            this.initializeController = initializeController;
            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var editPermissionAPI;
            var editPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            // package and account package security

            var viewPackagePermissionAPI;
            var viewPackagePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPackagePermissionAPI;
            var addPackagePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var editPackagePermissionAPI;
            var editPackagePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var viewAccountPackagePermissionAPI;
            var viewAccountPackagePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addAccountPackagePermissionAPI;
            var addAccountPackagePermissionReadyDeferred = UtilsService.createPromiseDeferred();

            // product security

            var viewProductPermissionAPI;
            var viewProductPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addProductPermissionAPI;
            var addProductPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var editProductPermissionAPI;
            var editProductPermissionReadyDeferred = UtilsService.createPromiseDeferred();

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
                //
                $scope.scopeModel.onViewPackageRequiredPermissionReady = function (api) {
                    viewPackagePermissionAPI = api;
                    viewPackagePermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddPackageRequiredPermissionReady = function (api) {
                    addPackagePermissionAPI = api;
                    addPackagePermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onEditPackageRequiredPermissionReady = function (api) {
                    editPackagePermissionAPI = api;
                    editPackagePermissionReadyDeferred.resolve();
                };


                $scope.scopeModel.onViewAccountPackageRequiredPermissionReady = function (api) {
                    viewAccountPackagePermissionAPI = api;
                    viewAccountPackagePermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddAccountPackageRequiredPermissionReady = function (api) {
                    addAccountPackagePermissionAPI = api;
                    addAccountPackagePermissionReadyDeferred.resolve();
                };
                //
                $scope.scopeModel.onViewProductRequiredPermissionReady = function (api) {
                    viewProductPermissionAPI = api;
                    viewProductPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddProductRequiredPermissionReady = function (api) {
                    addProductPermissionAPI = api;
                    addProductPermissionReadyDeferred.resolve();
                };
                $scope.scopeModel.onEditProductRequiredPermissionReady = function (api) {
                    editProductPermissionAPI = api;
                    editProductPermissionReadyDeferred.resolve();
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

                    // package

                    var loadViewPackageRequiredPermissionPromise = loadViewPackageRequiredPermission();
                    promises.push(loadViewPackageRequiredPermissionPromise);

                    var loadAddPackageRequiredPermissionPromise = loadAddPackageRequiredPermission();
                    promises.push(loadAddPackageRequiredPermissionPromise);

                    var loadEditPackageRequiredPermissionPromise = loadEditPackageRequiredPermission();
                    promises.push(loadEditPackageRequiredPermissionPromise);

                    var loadViewAccountPackageRequiredPermissionPromise = loadViewAccountPackageRequiredPermission();
                    promises.push(loadViewAccountPackageRequiredPermissionPromise);

                    var loadAddAccountPackageRequiredPermissionPromise = loadAddAccountPackageRequiredPermission();
                    promises.push(loadAddAccountPackageRequiredPermissionPromise);

                    // product

                    var loadViewProductRequiredPermissionPromise = loadViewProductRequiredPermission();
                    promises.push(loadViewProductRequiredPermissionPromise);

                    var loadAddProductRequiredPermissionPromise = loadAddProductRequiredPermission();
                    promises.push(loadAddProductRequiredPermissionPromise);

                    var loadEditProductRequiredPermissionPromise = loadEditProductRequiredPermission();
                    promises.push(loadEditProductRequiredPermissionPromise);

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

                    //
                    function loadViewPackageRequiredPermission() {
                        var viewPackagePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewPackagePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.ViewPackageRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewPackagePermissionAPI, dataPayload, viewPackagePermissionLoadDeferred);
                        });
                        return viewPackagePermissionLoadDeferred.promise;
                    }


                    function loadAddPackageRequiredPermission() {
                        var addPackagePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addPackagePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.AddPackageRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addPackagePermissionAPI, dataPayload, addPackagePermissionLoadDeferred);
                        });
                        return addPackagePermissionLoadDeferred.promise;
                    }

                    function loadEditPackageRequiredPermission() {
                        var editPackagePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        editPackagePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.EditPackageRequiredPermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(editPackagePermissionAPI, dataPayload, editPackagePermissionLoadDeferred);
                        });
                        return editPackagePermissionLoadDeferred.promise;
                    }

                    function loadViewAccountPackageRequiredPermission() {
                        var viewAccountPackagePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewAccountPackagePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.ViewAccountPackageRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewAccountPackagePermissionAPI, dataPayload, viewAccountPackagePermissionLoadDeferred);
                        });
                        return viewAccountPackagePermissionLoadDeferred.promise;
                    }


                    function loadAddAccountPackageRequiredPermission() {
                        var addAccountPackagePermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addAccountPackagePermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.AddAccountPackageRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addAccountPackagePermissionAPI, dataPayload, addAccountPackagePermissionLoadDeferred);
                        });
                        return addAccountPackagePermissionLoadDeferred.promise;
                    }

                    //

                    function loadViewProductRequiredPermission() {
                        var viewProductPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        viewProductPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.ViewProductRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(viewProductPermissionAPI, dataPayload, viewProductPermissionLoadDeferred);
                        });
                        return viewProductPermissionLoadDeferred.promise;
                    }


                    function loadAddProductRequiredPermission() {
                        var addProductPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        addProductPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.AddProductRequiredPermission || undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(addProductPermissionAPI, dataPayload, addProductPermissionLoadDeferred);
                        });
                        return addProductPermissionLoadDeferred.promise;
                    }

                    function loadEditProductRequiredPermission() {
                        var editProductPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                        editProductPermissionReadyDeferred.promise.then(function () {
                            var dataPayload = {
                                data: payload && payload.EditProductRequiredPermission || undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(editProductPermissionAPI, dataPayload, editProductPermissionLoadDeferred);
                        });
                        return editProductPermissionLoadDeferred.promise;
                    }


                    return UtilsService.waitMultiplePromises(promises);
                };

                

                api.getData = function () {
                    return {
                        ViewRequiredPermission: viewPermissionAPI.getData(),
                        AddRequiredPermission: addPermissionAPI.getData(),
                        EditRequiredPermission: editPermissionAPI.getData(),
                        ViewPackageRequiredPermission: viewPackagePermissionAPI.getData(),
                        AddPackageRequiredPermission: addPackagePermissionAPI.getData(),
                        EditPackageRequiredPermission: editPackagePermissionAPI.getData(),
                        ViewAccountPackageRequiredPermission: viewAccountPackagePermissionAPI.getData(),
                        AddAccountPackageRequiredPermission: addAccountPackagePermissionAPI.getData(),
                        ViewProductRequiredPermission: viewProductPermissionAPI.getData(),
                        AddProductRequiredPermission :addProductPermissionAPI.getData(),
                        EditProductRequiredPermission: editProductPermissionAPI.getData()
                    }
                   
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('retailBeAccountsecuritydefinitionManagement', AccountpayloadDefinitionManagementDirective);

})(app);