(function (app) {

    'use strict';

    PackageItemEditorDirective.$inject = ['Retail_BE_PackageService', 'UtilsService'];

    function PackageItemEditorDirective(Retail_BE_PackageService, UtilsService) {
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
                ctrl.packageItems = [];

                ctrl.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                ctrl.addPackageItem = function () {
                    var onPackageItemAdded = function (addedPackageItem) {
                        ctrl.packageItems.push(addedPackageItem);
                    };
                    Retail_BE_PackageService.addPackageItem(onPackageItemAdded);
                };
                ctrl.removePackageItem = function (packageItem) {
                    ctrl.packageItems.splice(ctrl.packageItems.indexOf(packageItem), 1);
                };
                ctrl.validatePackageItems = function () {
                    if (ctrl.packageItems.length > 0)
                        return null;
                    return "Please add at least one package item";
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.packageItems.length = 0;

                    var packageItems;

                    if (payload != undefined) {
                        packageItems = payload.packageItems;
                    }

                    if (packageItems != undefined) {
                        for (var i = 0; i < packageItems.length; i++) {
                            ctrl.packageItems.push(packageItems[i]);
                        }
                    }
                };

                api.getData = function () {
                    return ctrl.packageItems;
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
                    ctrl.packageItems[ctrl.packageItems.indexOf(packageItem)] = updatedPackageItem.PackageItem;
                };
                Retail_BE_PackageService.editPackageItem(packageItem, onPackageItemUpdated);
            }
        }
    }

    app.directive('retailBePackagePackageitemEditor', PackageItemEditorDirective);

})(app);