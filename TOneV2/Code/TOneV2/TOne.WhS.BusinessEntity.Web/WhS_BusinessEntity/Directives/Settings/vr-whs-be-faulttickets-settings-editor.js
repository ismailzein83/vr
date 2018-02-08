'use strict';

app.directive('vrWhsBeFaultticketsSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/Settings/Templates/FaultTicketsSettingsTemplate.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {

            this.initializeController = initializeController;

            var customerSetting;
            var supplierSetting;

            var customerSerialNumberEditorAPI;
            var customerSerialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierSerialNumberEditorAPI;
            var supplierSerialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var customerOpenMailTemplateSelectorAPI;
            var customerOpenMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var customerPendingMailTemplateSelectorAPI;
            var customerPendingMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var customerClosedMailTemplateSelectorAPI;
            var customerClosedMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierOpenMailTemplateSelectorAPI;
            var supplierOpenMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierPendingMailTemplateSelectorAPI;
            var supplierPendingMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierClosedMailTemplateSelectorAPI;
            var supplierClosedMailTemplateSelectorReadyDeferred = UtilsService.createPromiseDeferred();



            $scope.scopeModel = {};
            $scope.scopeModel.customerInitialSequence = 0;
            $scope.scopeModel.supplierInitialSequence = 0;
            $scope.scopeModel.onCustomerSerialNumberEditorReady = function (api) {
                customerSerialNumberEditorAPI = api;
                customerSerialNumberEditorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCustomerOpenMailTemplateSelectorReady = function (api) {
                customerOpenMailTemplateSelectorAPI = api;
                customerOpenMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCustomerPendingMailTemplateSelectorReady = function (api) {
                customerPendingMailTemplateSelectorAPI = api;
                customerPendingMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onCustomerClosedMailTemplateSelectorReady = function (api) {
                customerClosedMailTemplateSelectorAPI = api;
                customerClosedMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSupplierOpenMailTemplateSelectorReady = function (api) {
                supplierOpenMailTemplateSelectorAPI = api;
                supplierOpenMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSupplierPendingMailTemplateSelectorReady = function (api) {
                supplierPendingMailTemplateSelectorAPI = api;
                supplierPendingMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSupplierClosedMailTemplateSelectorReady = function (api) {
                supplierClosedMailTemplateSelectorAPI = api;
                supplierClosedMailTemplateSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onSupplierSerialNumberEditorReady = function (api) {
                supplierSerialNumberEditorAPI = api;
                supplierSerialNumberEditorReadyDeferred.resolve();
            };

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var data;
                   
                    if (payload != undefined) {
                        data = payload.data;
                    }
                    if (data != undefined)
                    {
                        customerSetting = data.CustomerSetting;
                        if (customerSetting != undefined)
                            $scope.scopeModel.customerInitialSequence = customerSetting.InitialSequence;
                        supplierSetting = data.SupplierSetting;
                        if (supplierSetting != undefined)
                            $scope.scopeModel.supplierInitialSequence = supplierSetting.InitialSequence;
                    }

                    var loadCustomerSerialNumberEditorPromise = loadCustomerSerialNumberEditor(customerSetting);
                    var loadSupplierSerialNumberEditorPromise = loadSupplierSerialNumberEditor(supplierSetting);
                    var loadCustomerOpenMailTemplateSelectorPromise = loadCustomerOpenMailTemplateSelector();
                    var loadCustomerPendingMailTemplateSelectorPromise = loadCustomerPendingMailTemplateSelector();
                    var loadCustomerClosedMailTemplateSelectorPromise = loadCustomerClosedMailTemplateSelector();
                    var loadSupplierOpenMailTemplateSelectorPromise = loadSupplierOpenMailTemplateSelector();
                    var loadSupplierPendingMailTemplateSelectorPromise = loadSupplierPendingMailTemplateSelector();
                    var loadSupplierClosedMailTemplateSelectorPromise = loadSupplierClosedMailTemplateSelector();

                    promises.push(loadCustomerSerialNumberEditorPromise);
                    promises.push(loadSupplierSerialNumberEditorPromise);
                    promises.push(loadCustomerOpenMailTemplateSelectorPromise);
                    promises.push(loadCustomerPendingMailTemplateSelectorPromise);
                    promises.push(loadCustomerClosedMailTemplateSelectorPromise);
                    promises.push(loadSupplierOpenMailTemplateSelectorPromise);
                    promises.push(loadSupplierPendingMailTemplateSelectorPromise);
                    promises.push(loadSupplierClosedMailTemplateSelectorPromise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.FaultTicketsSettingsData, TOne.WhS.BusinessEntity.Entities",
                        CustomerSetting: {
                            SerialNumberPattern: customerSerialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.customerInitialSequence,
                            OpenMailTemplateId :customerOpenMailTemplateSelectorAPI.getSelectedIds(),
                            PendingMailTemplateId:customerPendingMailTemplateSelectorAPI.getSelectedIds(),
                            ClosedMailTemplateId: customerClosedMailTemplateSelectorAPI.getSelectedIds(),
                        },
                        SupplierSetting : {
                            SerialNumberPattern: supplierSerialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.supplierInitialSequence,
                            OpenMailTemplateId: supplierOpenMailTemplateSelectorAPI.getSelectedIds(),
                            PendingMailTemplateId: supplierPendingMailTemplateSelectorAPI.getSelectedIds(),
                            ClosedMailTemplateId: supplierClosedMailTemplateSelectorAPI.getSelectedIds(),
                }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }



            function loadCustomerSerialNumberEditor(customerSetting) {
                var serialNumberEditorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                customerSerialNumberEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: customerSetting,
                        businessEntityDefinitionId: "e4053d52-8a52-438e-b353-37acf059a938"
                    };
                    VRUIUtilsService.callDirectiveLoad(customerSerialNumberEditorAPI, payload, serialNumberEditorEditorLoadDeferred);
                });
                return serialNumberEditorEditorLoadDeferred.promise;
            }
            function loadSupplierSerialNumberEditor(supplierSetting) {
                var serialNumberEditorEditorLoadDeferred = UtilsService.createPromiseDeferred();
                supplierSerialNumberEditorReadyDeferred.promise.then(function () {
                    var payload = {
                        data: supplierSetting,
                        businessEntityDefinitionId: "551d5b27-a4fb-44e8-82cc-e19fdc1e97ca"
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierSerialNumberEditorAPI, payload, serialNumberEditorEditorLoadDeferred);
                });
                return serialNumberEditorEditorLoadDeferred.promise;
            }
            function loadCustomerOpenMailTemplateSelector() {

                var customerOpenMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                customerOpenMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "293e09ce-4238-4eef-9c60-c069dddc588d",
                        },
                        selectedIds:customerSetting!=undefined?customerSetting.OpenMailTemplateId:undefined

                    };
                    VRUIUtilsService.callDirectiveLoad(customerOpenMailTemplateSelectorAPI, payload, customerOpenMailTemplateSelectorLoadDeferred);
                });
                return customerOpenMailTemplateSelectorLoadDeferred.promise;
            }
            function loadCustomerPendingMailTemplateSelector() {

                var customerPendingMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                customerPendingMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "293e09ce-4238-4eef-9c60-c069dddc588d"
                        },
                        selectedIds: customerSetting != undefined ? customerSetting.PendingMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(customerPendingMailTemplateSelectorAPI, payload, customerPendingMailTemplateSelectorLoadDeferred);
                });
                return customerPendingMailTemplateSelectorLoadDeferred.promise;
            }
            function loadCustomerClosedMailTemplateSelector() {

                var customerClosedMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                customerClosedMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "293e09ce-4238-4eef-9c60-c069dddc588d"
                        },
                        selectedIds: customerSetting != undefined ? customerSetting.ClosedMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(customerClosedMailTemplateSelectorAPI, payload, customerClosedMailTemplateSelectorLoadDeferred);
                });
                return customerClosedMailTemplateSelectorLoadDeferred.promise;
            }
            function loadSupplierOpenMailTemplateSelector() {

                var supplierOpenMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                supplierOpenMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "041c536c-8a83-40bc-9941-315a032c74f8"
                        },
                        selectedIds: supplierSetting != undefined ? supplierSetting.OpenMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierOpenMailTemplateSelectorAPI, payload, supplierOpenMailTemplateSelectorLoadDeferred);
                });
                return supplierOpenMailTemplateSelectorLoadDeferred.promise;
            }
            function loadSupplierPendingMailTemplateSelector() {

                var supplierPendingMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                supplierPendingMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "041c536c-8a83-40bc-9941-315a032c74f8"
                        },
                        selectedIds: supplierSetting != undefined ? supplierSetting.PendingMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierPendingMailTemplateSelectorAPI, payload, supplierPendingMailTemplateSelectorLoadDeferred);
                });
                return supplierPendingMailTemplateSelectorLoadDeferred.promise;
            }
            function loadSupplierClosedMailTemplateSelector() {

                var supplierClosedMailTemplateSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                supplierClosedMailTemplateSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            VRMailMessageTypeId: "041c536c-8a83-40bc-9941-315a032c74f8"
                        },
                        selectedIds: supplierSetting != undefined ? supplierSetting.ClosedMailTemplateId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(supplierClosedMailTemplateSelectorAPI, payload, supplierClosedMailTemplateSelectorLoadDeferred);
                });
                return supplierClosedMailTemplateSelectorLoadDeferred.promise;
            }


        }

        return directiveDefinitionObject;
    }]);