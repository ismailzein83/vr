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
                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addService = function () {
                    var onServiceAdded = function (serviceObj) {

                       var type = UtilsService.getItemByVal(templateConfigs, serviceObj.ConfigId, 'ExtensionConfigurationId');
                       if (type != undefined)
                           serviceObj.ServiceType = type.Title;
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
                        if (payload != undefined && payload.serviceEntity != undefined) {
                            ctrl.services.length = 0;
                            if (payload.serviceEntity.Services && payload.serviceEntity.Services.length > 0) {
                                for (var y = 0; y < payload.serviceEntity.Services.length; y++) {
                                    var currentService = payload.serviceEntity.Services[y];
                                    var type = UtilsService.getItemByVal(templateConfigs, currentService.ConfigId, 'ExtensionConfigurationId');
                                    if (type != undefined)
                                        currentService.ServiceType = type.Title;
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

            function editService(serviceStyle) {
                var onServiceUpdated = function (serviceObj) {
                    var type = UtilsService.getItemByVal(templateConfigs, serviceObj.ConfigId, 'ExtensionConfigurationId');
                    if (type != undefined)
                        serviceObj.ServiceType = type.Title;
                    ctrl.services[ctrl.services.indexOf(serviceStyle)] = serviceObj;
                }
                Retail_BE_PackageService.editService(serviceStyle, onServiceUpdated);
            }
        }
    }

    app.directive('retailBePackageServiceGridEditor', RetailBePackageServiceGridEditorDirective);

})(app);