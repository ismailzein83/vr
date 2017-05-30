(function (appControllers) {

    "use strict";

    AssignEndPointEditorController.$inject = ['$scope', 'NP_IVSwitch_EndPointAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService'];

    function AssignEndPointEditorController($scope, NP_IVSwitch_EndPointAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

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
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
            $scope.scopeModel.onSelectorEndPointReady = function (api) {
                selectorEndPointAPI = api;
                selectorEndPointReadyDeferred.resolve();
            };

        }
        function load() {
           $scope.scopeModel.isLoading = true;
           loadAllControls();

        }



       

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData,  loadSelectorEndPoint]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

           function setTitle() {
                
            }
            function loadStaticData() {

               
            }


            function loadSelectorEndPoint() {

                var selectorEndPointLoadDeferred = UtilsService.createPromiseDeferred();

                selectorEndPointReadyDeferred.promise.then(function () {
                    var selectorEndPointPayload = {};

                    VRUIUtilsService.callDirectiveLoad(selectorEndPointAPI, selectorEndPointPayload, selectorEndPointLoadDeferred);
                });

                return selectorEndPointLoadDeferred.promise;
            }
        }

        
        function update() {
            $scope.scopeModel.isLoading = true;

            return NP_IVSwitch_EndPointAPIService.UpdateEndPoint(buildEndPointObjFromScope()).then(function (response) {

                if (VRNotificationService.notifyOnItemUpdated('EndPoint', response, 'Name')) {

                    if ($scope.onEndPointUpdated != undefined) {
                        $scope.onEndPointUpdated(response.UpdatedObject);
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
                Entity:
             {
                 EndPointId: endPointEntity != undefined ? endPointEntity.EndPointId : undefined,
                 Description: $scope.scopeModel.description,
                 LogAlias: $scope.scopeModel.logalias,
                 CodecProfileId: $scope.scopeModel.codecprofileid,
                 TransRuleId: $scope.scopeModel.translationruleid,
                 CurrentState: $scope.scopeModel.currentstate.value,
                 ChannelsLimit: $scope.scopeModel.channelslimit,
                 MaxCallDuration: $scope.scopeModel.maxcallduration,
                 RtpMode: $scope.scopeModel.rtpmode,
                 DomainId: $scope.scopeModel.domaineid,
                 Host: $scope.scopeModel.subnet != undefined ? $scope.scopeModel.host + "/" + $scope.scopeModel.subnet : $scope.scopeModel.host,
                 TechPrefix: $scope.scopeModel.techprefix,
                 SipLogin: $scope.scopeModel.siplogin,
                 SipPassword: $scope.scopeModel.sippassword,
                 EndPointType: $scope.scopeModel.endpointtype
             },

                CarrierAccountId: carrierAccountId

            };
        }
    }

    appControllers.controller('NP_IVSwitch_AssignEndPointEditorController', AssignEndPointEditorController);

})(appControllers);