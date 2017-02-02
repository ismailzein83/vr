'use strict';

app.directive('retailBeVisibilitypackagedefinitionManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityPackageDefinition(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionPackageDefinitions/Templates/VisibilityPackageDefinitionManagementTemplate.html';
            }
        };

        function VisibilityPackageDefinition(ctrl, $scope) {
            this.initializeController = initializeController;

            var packageDefinitionSelectorAPI;
            var packageDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.packageDefinitionDefinitions = [];
                $scope.scopeModel.packageDefinitions = [];

                $scope.scopeModel.onPackageDefinitionsSelectorReady = function (api) {
                    packageDefinitionSelectorAPI = api;
                    packageDefinitionSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectPackageDefinition = function (selectedItem) {

                    $scope.scopeModel.packageDefinitions.push({
                        PackageDefinitionId: selectedItem.PackageDefinitionId,
                        Name: selectedItem.Name
                    });
                };
                $scope.scopeModel.onDeselectPackageDefinition = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.packageDefinitions, deselectedItem.PackageDefinitionId, 'PackageDefinitionId');
                    $scope.scopeModel.packageDefinitions.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedPackageDefinitions, deletedItem.PackageDefinitionId, 'PackageDefinitionId');
                    $scope.scopeModel.selectedPackageDefinitions.splice(index, 1);
                    $scope.scopeModel.onDeselectPackageDefinition(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var packageDefinitions;
                    var accountBEDefinitionId;

                    if (payload != undefined) {
                        packageDefinitions = payload.packageDefinitions;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    var loadPackageDefinitionSelectorPromise = getPackageDefinitionSelectorLoadPromise();
                    promises.push(loadPackageDefinitionSelectorPromise);

                    loadPackageDefinitionSelectorPromise.then(function () {

                        //Loading Grid
                        if ($scope.scopeModel.selectedPackageDefinitions != undefined) {
                            for (var i = 0; i < $scope.scopeModel.selectedPackageDefinitions.length; i++) {
                                var packageDefinitionDefinition = $scope.scopeModel.selectedPackageDefinitions[i];

                                $scope.scopeModel.packageDefinitions.push({
                                    PackageDefinitionId: packageDefinitionDefinition.PackageDefinitionId,
                                    Name: packageDefinitionDefinition.Name
                                });
                            }
                        }
                    });

                    function getPackageDefinitionSelectorLoadPromise() {
                        var packageDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        packageDefinitionSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                includeHiddenPackageDefinitions: true,
                                accountBEDefinitionId: accountBEDefinitionId,
                                selectedIds: []
                            };
                            if (packageDefinitions != undefined) {
                                for (var index = 0; index < packageDefinitions.length; index++) {
                                    selectorPayload.selectedIds.push(packageDefinitions[index].PackageDefinitionId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(packageDefinitionSelectorAPI, selectorPayload, packageDefinitionSelectorLoadDeferred);
                        });

                        return packageDefinitionSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var _packageDefinitions;
                    if ($scope.scopeModel.packageDefinitions.length > 0) {
                        _packageDefinitions = [];
                        for (var i = 0; i < $scope.scopeModel.packageDefinitions.length; i++) {
                            var currentPackageDefinition = $scope.scopeModel.packageDefinitions[i];
                            _packageDefinitions.push({
                                PackageDefinitionId: currentPackageDefinition.PackageDefinitionId
                            });
                        }
                    }
                    return _packageDefinitions
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);



