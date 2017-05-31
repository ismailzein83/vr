(function (appControllers) {

    "use strict";

    LinkEndPointsEditorController.$inject = ['$scope', 'NP_IVSwitch_SwitchMappingAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function LinkEndPointsEditorController($scope, NP_IVSwitch_SwitchMappingAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService) {


        var carrierAccountId;
     

        var selectorEndPointAPI;
        var selectorEndPointReadyDeferred = UtilsService.createPromiseDeferred();

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
                return linkWEndPointsToCarrierAccount();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onSelectorEndPointReady = function (api) {
                selectorEndPointAPI = api;
                selectorEndPointReadyDeferred.resolve();
            };

            $scope.scopeModel.validateEndPointAccount = function () {
                var selectedEndPoints = $scope.scopeModel.selectedEndPoints;
                if (selectedEndPoints == undefined)
                    return null;
                if (selectedEndPoints.length == 0 || selectedEndPoints.length == 1)
                    return null;
                if (selectedEndPoints.length > 1) {
                    var accountId = selectedEndPoints[0].AccountId;
                    for (var i = 1 ; i < selectedEndPoints.length; i++) {
                        if (selectedEndPoints[i].AccountId != accountId)
                            return "Please  select EndPoints having the same AccountId.";
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
            return UtilsService.waitMultipleAsyncOperations([setTitle,loadSelectorEndPoint]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           function setTitle() {
               $scope.title = "Link EndPoints to CarrierAccount";
           }
           function loadSelectorEndPoint() {
                var selectorEndPointLoadDeferred = UtilsService.createPromiseDeferred();

                selectorEndPointReadyDeferred.promise.then(function () {
                    var selectorEndPointPayload = {
                        filter: {
                            AssignableToCarrierAccountId: carrierAccountId
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(selectorEndPointAPI, selectorEndPointPayload, selectorEndPointLoadDeferred);
                });

                return selectorEndPointLoadDeferred.promise;
            }
        }

        
        function linkWEndPointsToCarrierAccount() {
            $scope.scopeModel.isLoading = true;
            return NP_IVSwitch_SwitchMappingAPIService.LinkCarrierToEndPoints(buildEndPointObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Switch Mapping', response)) {
                    if ($scope.onEndPointLinked != undefined) {
                        $scope.onEndPointLinked(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildEndPointObjFromScope() {
            return {             
                CarrierAccountId: carrierAccountId,
                EndPointIds: selectorEndPointAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('NP_IVSwitch_LinkEndPointsEditorController', LinkEndPointsEditorController);

})(appControllers);