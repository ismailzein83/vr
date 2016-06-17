(function (app) {

    'use strict';

    PackageItemEditorDirective.$inject = ['Retail_BE_PackageService', 'UtilsService', 'Retail_BE_ServiceTypeAPIService'];

    function PackageItemEditorDirective(Retail_BE_PackageService, UtilsService, Retail_BE_ServiceTypeAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageItemEditor = new PackageItemEditor($scope, ctrl, $attrs);
                packageItemEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageItem/Templates/PackageItemEditorTemplate.html"
        };

        function PackageItemEditor($scope, ctrl, $attrs) {

            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                ctrl.packageSettings = [];
                ctrl.serviceTypes = [];
                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.addPackageItem = function () {
                    var onPackageItemAdded = function (addedPackageItem) {
                        ctrl.packageSettings.push({ Entity: addedPackageItem });
                    };
                    Retail_BE_PackageService.addPackageItem(onPackageItemAdded);
                };
                ctrl.removePackageItem = function (packageSetting) {
                    ctrl.packageSettings.splice(ctrl.packageSettings.indexOf(packageSetting), 1);
                };
                ctrl.validatePackageItems = function () {
                    if (ctrl.packageSettings.length > 0)
                        return null;
                    return "Please add at least one package item";
                };
            
                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    ctrl.packageSettings.length = 0;
                   
                    var packageSettings;

                    if (payload != undefined) {
                        packageSettings = payload.packageSettings;
                    }
                    return getServiceTypes().then(function () {
                        if (packageSettings != undefined && packageSettings.Services != undefined) {
                            for (var i = 0; i < packageSettings.Services.length; i++) {
                                var service = packageSettings.Services[i];
                                var  serviceType = UtilsService.getItemByVal(ctrl.serviceTypes, service.ServiceTypeId, "ServiceTypeId");
                                if (serviceType != undefined)
                                    service.serviceTypeTitle = serviceType.Title;
                                ctrl.packageSettings.push({ Entity: service });
                            }
                        }

                    });
                };

                api.getData = function () {
                    var packageSettings = [];
                    for (var i = 0; i < ctrl.packageSettings.length; i++)
                    {
                        var packageSetting = ctrl.packageSettings[i];
                        packageSettings.push(packageSetting.Entity);
                    }
                    return packageSettings;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                ctrl.menuActions = [{
                    name: 'Edit',
                    clicked: editPackageItem
                }];
            }
            function editPackageItem(packageItem) {
                var onPackageItemUpdated = function (updatedPackageItem) {
                    ctrl.packageSettings[ctrl.packageSettings.indexOf(packageItem)] = { Entity: updatedPackageItem };
                };
                Retail_BE_PackageService.editPackageItem(packageItem.Entity, onPackageItemUpdated);
            }
            function getServiceTypes()
            {
                return Retail_BE_ServiceTypeAPIService.GetServiceTypesInfo().then(function (response) {
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            ctrl.serviceTypes.push(response[i]);
                        }
                    }
                });
            }
        }
    }

    app.directive('retailBePackagePackageitemEditor', PackageItemEditorDirective);

})(app);