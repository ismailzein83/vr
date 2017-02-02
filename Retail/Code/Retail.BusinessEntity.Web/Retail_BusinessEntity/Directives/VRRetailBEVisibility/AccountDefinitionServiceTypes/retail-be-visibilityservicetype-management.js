'use strict';

app.directive('retailBeVisibilityservicetypeManagement', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new VisibilityServiceType(ctrl, $scope);
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
                return '/Client/Modules/Retail_BusinessEntity/Directives/VRRetailBEVisibility/AccountDefinitionServiceTypes/Templates/VisibilityServiceTypeManagementTemplate.html';
            }
        };

        function VisibilityServiceType(ctrl, $scope) {
            this.initializeController = initializeController;

            var serviceTypeSelectorAPI;
            var serviceTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.serviceTypeDefinitions = [];
                $scope.scopeModel.serviceTypes = [];

                $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                    serviceTypeSelectorAPI = api;
                    serviceTypeSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onSelectServiceType = function (selectedItem) {

                    $scope.scopeModel.serviceTypes.push({
                        ServiceTypeId: selectedItem.ServiceTypeId,
                        Name: selectedItem.Title
                    });
                };
                $scope.scopeModel.onDeselectServiceType = function (deselectedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.serviceTypes, deselectedItem.ServiceTypeId, 'ServiceTypeId');
                    $scope.scopeModel.serviceTypes.splice(index, 1);
                };

                $scope.scopeModel.onDeleteRow = function (deletedItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedServiceTypes, deletedItem.ServiceTypeId, 'ServiceTypeId');
                    $scope.scopeModel.selectedServiceTypes.splice(index, 1);
                    $scope.scopeModel.onDeselectServiceType(deletedItem);
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var serviceTypes;
                    var accountBEDefinitionId;

                    if (payload != undefined) {
                        serviceTypes = payload.serviceTypes;
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                    }

                    var loadServiceTypeSelectorPromise = getServiceTypeSelectorLoadPromise();
                    promises.push(loadServiceTypeSelectorPromise);

                    loadServiceTypeSelectorPromise.then(function () {

                        //Loading Grid
                        if ($scope.scopeModel.selectedServiceTypes != undefined) {
                            for (var i = 0; i < $scope.scopeModel.selectedServiceTypes.length; i++) {
                                var serviceTypeDefinition = $scope.scopeModel.selectedServiceTypes[i];

                                $scope.scopeModel.serviceTypes.push({
                                    ServiceTypeId: serviceTypeDefinition.ServiceTypeId,
                                    Name: serviceTypeDefinition.Title
                                });
                            }
                        }
                    });

                    function getServiceTypeSelectorLoadPromise() {
                        var serviceTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        serviceTypeSelectorPromiseDeferred.promise.then(function () {

                            var selectorPayload = {
                                includeHiddenServiceTypes: true,
                                accountBEDefinitionId: accountBEDefinitionId,
                                selectedIds: []
                            };
                            if (serviceTypes != undefined) {
                                for (var index = 0; index < serviceTypes.length; index++) {
                                    selectorPayload.selectedIds.push(serviceTypes[index].ServiceTypeId);
                                }
                            }

                            VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, selectorPayload, serviceTypeSelectorLoadDeferred);
                        });

                        return serviceTypeSelectorLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var _serviceTypes;
                    if ($scope.scopeModel.serviceTypes.length > 0) {
                        _serviceTypes = [];
                        for (var i = 0; i < $scope.scopeModel.serviceTypes.length; i++) {
                            var currentServiceType = $scope.scopeModel.serviceTypes[i];
                            _serviceTypes.push({
                                ServiceTypeId: currentServiceType.ServiceTypeId
                            });
                        }
                    }
                    return _serviceTypes
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);



