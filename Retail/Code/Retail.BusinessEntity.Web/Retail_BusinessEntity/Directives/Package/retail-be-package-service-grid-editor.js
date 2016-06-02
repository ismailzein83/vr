(function (app) {

    'use strict';

    RetailBePackageServiceGridEditorDirective.$inject = ['Retail_BE_PackageService', 'UtilsService','Retail_BE_PackageAPIService'];

    function RetailBePackageServiceGridEditorDirective(Retail_BE_PackageService, UtilsService, Retail_BE_PackageAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var retailBePackageServiceGridEditor = new RetailBePackageServiceGridEditor($scope, ctrl, $attrs);
                retailBePackageServiceGridEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Package/Templates/ServiceGridEditorTemplate.html"
        };

        function RetailBePackageServiceGridEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var templateConfigs = [];
            function initializeController() {
                ctrl.services = [];

                ctrl.isValidServices = function () {

                    if (ctrl.services.length > 0)
                        return null;
                    return "At least one service should be added.";
                }
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addService = function () {
                    var onServiceAdded = function (serviceObj) {
                        ctrl.services.push(serviceObj);
                    }
                    Retail_BE_PackageService.addService(onServiceAdded);
                }

                ctrl.removeService = function (serviceObj) {
                    ctrl.services.splice(ctrl.services.indexOf(serviceObj), 1);
                }

                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    var promises = [];
                    promises.push(getServicesTemplateConfigs());
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        if (payload != undefined && payload.packageSettings != undefined) {
                            ctrl.services.length = 0;
                            if (payload.packageSettings.Services && payload.packageSettings.Services.length > 0) {
                                for (var y = 0; y < payload.packageSettings.Services.length; y++) {
                                    var currentService = payload.packageSettings.Services[y];
                                    ctrl.services.push(currentService);
                                }
                            }
                        }
                    });
                };

                api.getData = function () {
                    var services = [];
                    for (var i = 0; i < ctrl.services.length ; i++) {
                        var service = ctrl.services[i];
                        services.push(service);
                    }
                    return services;
                }
                return api;
            }

            function defineMenuActions() {
                ctrl.servicesGridMenuActions = [{
                    name: 'Edit',
                    clicked: editService
                }];
            }


            function getServicesTemplateConfigs() {
                return Retail_BE_PackageAPIService.GetServicesTemplateConfigs().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            templateConfigs.push(response[i]);
                        }
                    }
                });
            }

            function editService(service) {
                var onServiceUpdated = function (serviceObj) {
                    ctrl.services[ctrl.services.indexOf(service)] = serviceObj;
                }
                Retail_BE_PackageService.editService(service, onServiceUpdated);
            }
        }
    }

    app.directive('retailBePackageServiceGridEditor', RetailBePackageServiceGridEditorDirective);

})(app);