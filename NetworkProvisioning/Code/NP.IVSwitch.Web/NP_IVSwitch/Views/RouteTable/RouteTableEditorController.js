(function (appControllers) {

    "use strict";
    routeTableEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteTableAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'NP_IVSwitch_RouteTableViewTypeEnum', 'NP_IVSwitch_EndPointStateEnum'];

    function routeTableEditorController($scope, NP_IVSwitch_RouteTableAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, NP_IVSwitch_RouteTableViewTypeEnum, NP_IVSwitch_EndPointStateEnum) {

        var isEditMode;
        var routeTableViewType;
        var routeTableId;
        var routeTableEntity;
        var endPointSelectorDirectiveAPI;
        var customerSelectorDirectiveAPI;
        var selectedCarrierAccounts = [];
        var selectedEndPoints = [];
        var endPointSelectorDirectiveReadyPromiseDeffered = UtilsService.createPromiseDeferred();
        var customerSelectorDirectiveReadyPromiseDeffered = UtilsService.createPromiseDeferred();
        var selectedCustomerDeffered = UtilsService.createPromiseDeferred();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onEndPointSelectorDirectiveReady = function (api) {
                endPointSelectorDirectiveAPI = api;
                endPointSelectorDirectiveReadyPromiseDeffered.resolve();
            };

            $scope.scopeModel.onCustomerSelectorDirectiveReady = function (api) {
                customerSelectorDirectiveAPI = api;
                customerSelectorDirectiveReadyPromiseDeffered.resolve();

            };

            $scope.scopeModel.saveRouteTable = function () {
                if (isEditMode)
                    return updateRouteTable();
                else
                    return insertRouteTable();

            };

            $scope.scopeModel.hasSaveRouteTablePermission = function () {
                if (isEditMode)
                    return NP_IVSwitch_RouteTableAPIService.HasUpdateRouteTablePermission();
                else
                    return NP_IVSwitch_RouteTableAPIService.HasAddRouteTablePermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onCustomerSelectionChanged = function (option) {
                var accountIds = [];
                for (var i = 0; i < option.length; i++)
                    accountIds.push(option[i].CarrierAccountId);
                $scope.scopeModel.selectedvalues = option;
                if (option != undefined) {
                    if (selectedCustomerDeffered != undefined) {

                        selectedCustomerDeffered.resolve();
                    }
                    else {
                        var endPointPayload = {
                            filter: {
                                AssignableToCarrierAccountId: null,
                                CustomerIds: accountIds,
                                Filters: [{
                                    $type: "NP.IVSwitch.Business.EndPointViewFilter,NP.IVSwitch.Business",
                                    RouteTableId: routeTableId,
                                    RouteTableViewType: routeTableViewType,

                                }]
                            }, selectedIds: selectedEndPoints.length != 0 ? selectedEndPoints : undefined

                        };
                        var modifiedByFieldSetLoader = function (value) { $scope.scopeModel.isModifiedBySelectorLoading = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, endPointSelectorDirectiveAPI, endPointPayload, modifiedByFieldSetLoader);

                    }
                }


            };

        }

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                routeTableId = parameters.RouteTableId;
                routeTableViewType = parameters.RouteTableViewType;
            }
            isEditMode = (routeTableId != undefined);
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getRouteTable().then(function () {
                    loadAllControls().finally(function () {
                        routeTableEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        }

        defineScope();
        loadParameters();
        load();


        function getRouteTable() {
            var RouteTableViewType= (routeTableViewType == NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value) ? NP_IVSwitch_RouteTableViewTypeEnum.ANumber.value : NP_IVSwitch_RouteTableViewTypeEnum.Whitelist.value;
            return NP_IVSwitch_RouteTableAPIService.GetRouteTableById(routeTableId, routeTableViewType).then(function (response) {
                for (var i = 0; i < response.RouteTableInput.EndPoints.length; i++)
                    selectedEndPoints.push(response.RouteTableInput.EndPoints[i].EndPointId);
                var endPointCarrierAccount;
                if (response != undefined && response.EndPointCarrierAccount != undefined)
                    endPointCarrierAccount=response.EndPointCarrierAccount;
                if (endPointCarrierAccount != undefined)
                    for (var i = 0; i < endPointCarrierAccount.length; i++)
                        if (endPointCarrierAccount[i].CarrierAccount != null)
                            if (!selectedCarrierAccounts.includes(endPointCarrierAccount[i].CarrierAccount))
                                selectedCarrierAccounts.push(endPointCarrierAccount[i].CarrierAccount);

                if (selectedCarrierAccounts.length == 0)
                    selectedCarrierAccounts = undefined;
                var directivePayload = {
                    selectedIds: selectedCarrierAccounts,
                    filter: {

                        Filters: [{ $type: "NP.IVSwitch.Business.CarrierAccountEndPointFilter,NP.IVSwitch.Business" }]
                    }
                };
                VRUIUtilsService.callDirectiveLoad(customerSelectorDirectiveAPI, directivePayload, undefined);
                routeTableEntity = response;
            });
        }

        function loadAllControls() {

            function cusomerDirective() {
                return customerSelectorDirectiveReadyPromiseDeffered.promise.then(function () {
                    var directivePayload = {
                        filter: {

                            Filters: [{ $type: "NP.IVSwitch.Business.CarrierAccountEndPointFilter,NP.IVSwitch.Business" }]
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(customerSelectorDirectiveAPI, directivePayload, undefined);
                });
            }

            function setTitle() {
                if (isEditMode && routeTableEntity != undefined) {
                    $scope.title = UtilsService.buildTitleForUpdateEditor(routeTableEntity.RouteTableInput.RouteTable.Name, "Route Table");
                }
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Route Table");
            }

            function loadStaticData() {
                if (routeTableEntity != undefined) {
                    $scope.scopeModel.name = routeTableEntity.RouteTableInput.RouteTable.Name;
                    $scope.scopeModel.description = routeTableEntity.RouteTableInput.RouteTable.Description;
                    $scope.scopeModel.pScore = routeTableEntity.RouteTableInput.RouteTable.PScore;
                }
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, cusomerDirective]).then(function () {

                selectedCustomerDeffered = undefined;

            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }

        function buildParentObjectFromScope() {
            var scopeObject = {
                RouteTableViewType: routeTableViewType ,

                RouteTable: {
                    RouteTableId: (routeTableId != undefined) ? routeTableId : undefined,
                    Name: $scope.scopeModel.name,
                    Description: $scope.scopeModel.description,
                    PScore: $scope.scopeModel.pScore
                },
                EndPoints: getEndPoints()
            };
            return scopeObject;
        }

        function getEndPoints() {
            var endPointsIds = [];
            var endPoints = endPointSelectorDirectiveAPI.getSelectedIds();
            for (var index = 0; index < endPoints.length; index++) {
                var itemTab = { EndPointId: endPoints[index] };
                endPointsIds.push(itemTab);
            }
            return endPointsIds;
        }

        function insertRouteTable() {
            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScope();
            return NP_IVSwitch_RouteTableAPIService.AddRouteTable(routeTableObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("RouteTable", response, "Name")) {
                    if ($scope.onRouteTableAdded != undefined) {
                        $scope.onRouteTableAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateRouteTable() {
            $scope.scopeModel.isLoading = true;
            var routeTableObject = buildParentObjectFromScope();
            NP_IVSwitch_RouteTableAPIService.UpdateRouteTable(routeTableObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("RouteTable", response, "Name")) {
                    if ($scope.onRouteTableUpdated != undefined) {
                        $scope.onRouteTableUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        }

    }
    appControllers.controller('NP_IVSwitch_RouteTableEditorController', routeTableEditorController);
})(appControllers);