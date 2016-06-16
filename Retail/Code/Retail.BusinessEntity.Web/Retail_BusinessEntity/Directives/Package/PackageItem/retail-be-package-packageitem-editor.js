(function (app) {

    'use strict';

    PackageitemEditorDirective.$inject = ['Retail_BE_PackageService', 'UtilsService'];

    function PackageitemEditorDirective(Retail_BE_PackageService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var packageitemEditor = new PackageitemEditor($scope, ctrl, $attrs);
                packageitemEditor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/PackageItem/Templates/PackageItemEditorTemplate.html"
        };

        function PackageitemEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var gridAPI;
            var counter = 0;

            function initializeController() {
                ctrl.packageItems = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(getDirectiveAPI());
                    }
                };

                ctrl.addPackageItem = function () {
                    var onPackageItemAdded = function (packageItemObj) {
                        ctrl.packageItems.push({
                            Entity: packageItemObj.PackageItem
                        });
                    }
                    Retail_BE_PackageService.addPackageItem(onPackageItemAdded);
                }

                ctrl.removePart = function (partObj) {
                    ctrl.packageItems.splice(ctrl.packageItems.indexOf(partObj), 1);
                }

                ctrl.validateParts = function () {
                    if (ctrl.packageItems.length > 0)
                        return null;
                    return "One package item at least should be added.";
                }
                defineMenuActions();
            }

            function getDirectiveAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        ctrl.packageItems.length = 0;
                        if (payload.packageItems) {
                            for (var i = 0; i < payload.packageItems.length; i++) {
                                var currentItemAction = payload.packageItems[i];
                                ctrl.packageItems.push({
                                    Entity: currentItemAction
                                });
                            }
                        }
                    }
                };

                api.getData = function () {
                    var packageItems = [];
                    for (var i = 0; i < ctrl.packageItems.length; i++) {
                        var packageItem = ctrl.packageItems[i];
                        packageItems.push(packageItem.Entity);
                    }
                    return packageItems;
                }

                return api;
            }

            function defineMenuActions() {
                ctrl.partsGridMenuActions = [{
                    name: 'Edit',
                    clicked: editPackageItem
                }];
            }

            function editPackageItem(packageItem) {
                var onPackageItemUpdated = function (packageItemObj) {
                    ctrl.packageItems[ctrl.packageItems.indexOf(packageItem)] = {
                        Entity: packageItemObj.PackageItem
                    };
                }
                Retail_BE_PackageService.editPackageItem(packageItem.Entity, onPackageItemUpdated);
            }
        }
    }

    app.directive('retailBePackagePackageitemEditor', PackageitemEditorDirective);

})(app);