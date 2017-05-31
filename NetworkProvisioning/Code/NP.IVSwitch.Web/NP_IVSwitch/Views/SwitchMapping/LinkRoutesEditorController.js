(function (appControllers) {

    "use strict";

    LinkRoutesEditorController.$inject = ['$scope', 'NP_IVSwitch_SwitchMappingAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function LinkRoutesEditorController($scope, NP_IVSwitch_SwitchMappingAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService) {


        var carrierAccountId;
     

        var selectorRouteAPI;
        var selectorRouteReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                carrierAccountId = parameters.CarrierAccountId;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.DisableSelector = false;

            $scope.scopeModel.save = function () {
                return linkWRoutesToCarrierAccount();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onSelectorRouteReady = function (api) {
                selectorRouteAPI = api;
                selectorRouteReadyDeferred.resolve();
            };
            
            $scope.scopeModel.validateRouteAccount = function () {
                console.log($scope.scopeModel.selectedRoutes);
                var selectedRoutes = $scope.scopeModel.selectedRoutes;
                if (selectedRoutes == undefined)
                    return null;
                if (selectedRoutes.length == 0 || selectedRoutes.length == 1)
                    return null;
                if (selectedRoutes.length > 1) {
                    var accountId = selectedRoutes[0].AccountId;
                    for (var i = 1 ; i < selectedRoutes.length; i++) {
                        if (selectedRoutes[i].AccountId != accountId)
                            return "Please  select Routes having the same AccountId.";
                        else
                            continue;
                    }
                }               
                return null;
            };


        }
        function load() {
           $scope.scopeModel.isLoading = true;
           loadAllControls();

        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle,loadSelectorRoute]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           function setTitle() {
               $scope.title = "Link Routes to CarrierAccount";
           }
           function loadSelectorRoute() {
                var selectorRouteLoadDeferred = UtilsService.createPromiseDeferred();

                selectorRouteReadyDeferred.promise.then(function () {
                    var selectorRoutePayload = {
                        filter: {
                            AssignableToCarrierAccountId: carrierAccountId
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(selectorRouteAPI, selectorRoutePayload, selectorRouteLoadDeferred);
                });

                return selectorRouteLoadDeferred.promise;
            }
        }

        
        function linkWRoutesToCarrierAccount() {
            $scope.scopeModel.isLoading = true;
            return NP_IVSwitch_SwitchMappingAPIService.LinkCarrierToRoutes(buildRouteObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Switch Mapping', response)) {
                    if ($scope.onRouteLinked != undefined) {
                        $scope.onRouteLinked(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildRouteObjFromScope() {
            return {             
                CarrierAccountId: carrierAccountId,
                RouteIds: selectorRouteAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('NP_IVSwitch_LinkRoutesEditorController', LinkRoutesEditorController);

})(appControllers);