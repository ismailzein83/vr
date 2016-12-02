'use strict';

app.directive('retailBePackageGrid', ['VRNotificationService', 'Retail_BE_PackageAPIService', 'Retail_BE_PackageService',
    function (VRNotificationService, Retail_BE_PackageAPIService, Retail_BE_PackageService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var packageGrid = new PackageGridCtor($scope, ctrl, $attrs);
                packageGrid.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/Templates/PackageGridTemplate.html'
        };

        function PackageGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.package = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_PackageAPIService.GetFilteredPackages(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onPackageAdded = function (addedPackage) {
                    gridAPI.itemAdded(addedPackage);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editPackage,
                    //haspermission: hasEditPackagePermission
                });
            }
            function editPackage(packageItem) {
                var onPackageUpdated = function (updatedPackage) {
                    gridAPI.itemUpdated(updatedPackage);
                };

                Retail_BE_PackageService.editPackage(packageItem.Entity.PackageId, onPackageUpdated);
            }
            function hasEditPackagePermission() {
                return Retail_BE_PackageAPIService.HasEditPackagePermission();
            }
        }
    }]);
