(function (appControllers) {
    "use strict";
    applyDraftOnMultipleCustomersController.$inject = ['$scope', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService', 'WhS_Sales_RatePlanAPIService', 'WhS_BE_SalePriceListOwnerTypeEnum'];

    function applyDraftOnMultipleCustomersController($scope, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService, WhS_Sales_RatePlanAPIService, WhS_BE_SalePriceListOwnerTypeEnum) {

        var ownerId;
        var customersSelectorApi;
        var customersSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                ownerId = parameters.ownerId;
            }
        }

        function defineScope() {
            $scope.title = "Apply on multiple subscribers";
            $scope.hintText = "Rate's BED of the subscribers can either follow the same BED of their publisher, or can be calculated according to the system parameters for increased and decreased rates.";
            $scope.followPublisherHintText = "Subscribers can follow the publisher routing product in case of rate change";
            $scope.datasource = [];
            $scope.selectedvalues = [];
            $scope.gridDataSource = [];
            $scope.onCustomersSelectorReady = function (api) {
                customersSelectorApi = api;
                customersSelectorReadyPromiseDeferred.resolve();
            };
            $scope.isGridDataValid = function () {
                return ($scope.gridDataSource.length == 0) ? 'No customers selected' : null;
            };

            $scope.add = function () {
                addToGrid();
            };

            $scope.isAddButtonDisabled = function () {
                if ($scope.selectedvalues == null || $scope.selectedvalues.length == 0) {
                    return true;
                }
            };
            $scope.applyDraftOnMultipleCustomers = function () {
                return applyDraftOnMultipleCustomers();
            };

            $scope.remove = function (dataRow) {
                var entities = UtilsService.getPropValuesFromArray($scope.gridDataSource, 'Entity');
                if (entities == undefined)
                    return;
                var index = UtilsService.getItemIndexByVal(entities, dataRow.Entity.Id, 'Id');
                $scope.gridDataSource.splice(index, 1);

                $scope.isLoading = true;
                loadCustomersSelector().finally(function () {
                    $scope.isLoading = false;
                });
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadDraftSubscriberOwnerIds, loadFollowPublisherRatesBED, loadFollowPublisherRoutingProduct])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
        
        function loadFollowPublisherRatesBED() {
            return WhS_Sales_RatePlanAPIService.GetFollowPublisherRatesBED().then(function (response) {
                $scope.followPublisherRatesBED = response;
            });
        }
         function loadFollowPublisherRoutingProduct() {
             return WhS_Sales_RatePlanAPIService.GetFollowPublisherRoutingProduct().then(function (response) {
                 console.log(response);
                 $scope.followPublisherRoutingProduct = response;
            });
            }

        function loadCustomersSelector() {
            $scope.datasource = [];
            $scope.selectedvalues = [];
            var GetSubscriberOwnersInput = {
                OwnerId: ownerId,
                ExecludedOwnerIds: getAddedOwnerIds(),
            };
            return WhS_Sales_RatePlanAPIService.GetSubscriberOwners(GetSubscriberOwnersInput).then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.datasource.push(response[i]);
                    }
                }
            });
        }

        function loadDraftSubscriberOwnerIds() {
            var loadDraftSubscriberOwnerIdsPromiseDeferred = UtilsService.createPromiseDeferred();
            WhS_Sales_RatePlanAPIService.GetDraftSubscriberOwnerEntities(WhS_BE_SalePriceListOwnerTypeEnum.Customer.value, ownerId).
            then(function (response) {
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        var customer = response[i];
                        $scope.gridDataSource.push({ Entity: customer });
                    }
                }

                loadCustomersSelector().finally(function (response) {
                    loadDraftSubscriberOwnerIdsPromiseDeferred.resolve();
                });
            });
            return loadDraftSubscriberOwnerIdsPromiseDeferred.promise;
        }

        function applyDraftOnMultipleCustomers() {
            if ($scope.executeApplyDraftOnMultipleCustomersProcess != undefined) {
                $scope.executeApplyDraftOnMultipleCustomersProcess($scope.gridDataSource, $scope.followPublisherRatesBED, $scope.followPublisherRoutingProduct);
            }
            $scope.modalContext.closeModal();
        }

        function getAddedOwnerIds() {
            var addedOwnerIds = [];
            if ($scope.gridDataSource != null) {
                for (var i = 0; i < $scope.gridDataSource.length; i++)
                    addedOwnerIds.push($scope.gridDataSource[i].Entity.EntityId);
            }
            return addedOwnerIds;
        }

        function addToGrid() {

            if ($scope.selectedvalues != undefined) {
                for (var i = 0; i < $scope.selectedvalues.length; i++) {
                    var customer = $scope.selectedvalues[i];
                    var entity = {
                        Id: ($scope.gridDataSource.length + 1),
                        EntityId: customer.CarrierAccountId,
                        EntityName: customer.Name
                    };
                    $scope.gridDataSource.push({ Entity: entity });
                }

                $scope.isLoading = true;
                loadCustomersSelector().finally(function () {
                    $scope.isLoading = false;
                });
            }
        }
    }

    appControllers.controller('VRCommon_ApplyDraftOnMultipleCustomersController', applyDraftOnMultipleCustomersController);
})(appControllers);
