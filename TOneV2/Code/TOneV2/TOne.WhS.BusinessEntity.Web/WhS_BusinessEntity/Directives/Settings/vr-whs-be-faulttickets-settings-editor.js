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

            var customerSerialNumberEditorAPI;
            var customerSerialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();

            var supplierSerialNumberEditorAPI;
            var supplierSerialNumberEditorReadyDeferred = UtilsService.createPromiseDeferred();

            $scope.scopeModel = {};
            $scope.scopeModel.customerInitialSequence = 0;
            $scope.scopeModel.supplierInitialSequence = 0;
            $scope.scopeModel.onCustomerSerialNumberEditorReady = function (api) {
                customerSerialNumberEditorAPI = api;
                customerSerialNumberEditorReadyDeferred.resolve();
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
                    var customerSetting;
                    var supplierSetting;
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
                    promises.push(loadCustomerSerialNumberEditorPromise);
                    promises.push(loadSupplierSerialNumberEditorPromise);


                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.Entities.FaultTicketsSettingsData, TOne.WhS.BusinessEntity.Entities",
                        CustomerSetting: {
                            SerialNumberPattern: customerSerialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.customerInitialSequence
                        },
                        SupplierSetting : {
                            SerialNumberPattern: supplierSerialNumberEditorAPI.getData().serialNumberPattern,
                            InitialSequence: $scope.scopeModel.supplierInitialSequence
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


        }

        return directiveDefinitionObject;
    }]);