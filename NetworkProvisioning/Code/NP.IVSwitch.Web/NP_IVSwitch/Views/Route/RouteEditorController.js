(function (appControllers) {

    "use strict";

    RouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_StateEnum','NP_IVSwitch_TransportModeEnum'];

    function RouteEditorController($scope, NP_IVSwitch_RouteAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum,   NP_IVSwitch_TransportModeEnum) {

        var isEditMode;
 
        var routeId;
        var carrierAccountId ;

        var routeEntity;

        var selectorCodecProfileAPI;
        var selectorCodecProfileReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTranslationRuleAPI;
        var selectorTranslationRuleReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
 
            if (parameters != undefined && parameters != null) {
                 routeId = parameters.RouteId;
                 carrierAccountId = parameters.CarrierAccountId;
               }

            isEditMode = (routeId != undefined);
         }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.port = "5060";
            $scope.scopeModel.connectiontimeout = "60";
            $scope.scopeModel.transportmodeid = { value: 1, description: "UDP" };
            $scope.scopeModel.wakeuptime = Date.now();

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.onSelectorCodecProfileReady = function (api) {
                selectorCodecProfileAPI = api;
                selectorCodecProfileReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorTranslationRuleReady = function (api) {
                selectorTranslationRuleAPI = api;
                selectorTranslationRuleReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectionChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.TypeId = SelectedItem.value;
                }
            }

            $scope.scopeModel.onSelectionCodecProfileChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.codecprofileid = SelectedItem.CodecProfileId;
                 }
            }

            $scope.scopeModel.onSelectionTranslationRuleChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.translationruleid = SelectedItem.TranslationRuleId;
                 }
            }                        

            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.currentstate = SelectedItem;
                }
            }

            $scope.scopeModel.onSelectionChangedTransportMode = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.transportmodeid = SelectedItem;
                 }
            }
           
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getRoute().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }            
        }


        function getRoute() {
            return NP_IVSwitch_RouteAPIService.GetRoute(routeId).then(function (response) {
                routeEntity = response;                
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorCodecProfile, loadSelectorTranslationRule]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var routeName = (routeEntity != undefined) ? routeEntity.Description : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(routeName, 'Route');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Route');
                }
            }
            function loadStaticData() {

                $scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_StateEnum);
                $scope.scopeModel.transportmode = UtilsService.getArrayEnum(NP_IVSwitch_TransportModeEnum);

 
                if (routeEntity == undefined) {

                    $scope.scopeModel.currentstate = $scope.scopeModel.states[0];
  
                    return;
                }
                $scope.scopeModel.description = routeEntity.Description;
                $scope.scopeModel.logalias = routeEntity.LogAlias;

                $scope.scopeModel.channelslimit = routeEntity.ChannelsLimit;
                $scope.scopeModel.host = routeEntity.Host;
                $scope.scopeModel.port = routeEntity.Port;
                $scope.scopeModel.connectiontimeout = routeEntity.ConnectionTimeOut;
 
                if (routeEntity.TransportModeId != undefined)
                    $scope.scopeModel.transportmodeid = $scope.scopeModel.transportmode[routeEntity.TransportModeId - 1];
                if (routeEntity.CurrentState != undefined)
                    $scope.scopeModel.currentstate = $scope.scopeModel.states[routeEntity.CurrentState - 1];
                if (routeEntity.WakeUpTime != undefined)
                $scope.scopeModel.wakeuptime = routeEntity.WakeUpTime;                 

             }

            function loadSelectorCodecProfile() {
                var selectorCodecProfileLoadDeferred = UtilsService.createPromiseDeferred();

                selectorCodecProfileReadyDeferred.promise.then(function () {
                    var selectorCodecProfilePayload = {};
 
                    if (routeEntity != undefined && routeEntity.CodecProfileId != 0)  
                        selectorCodecProfilePayload.selectedIds = routeEntity.CodecProfileId;           

                    VRUIUtilsService.callDirectiveLoad(selectorCodecProfileAPI, selectorCodecProfilePayload, selectorCodecProfileLoadDeferred);
                });

                return selectorCodecProfileLoadDeferred.promise;
            }

            function loadSelectorTranslationRule() {
                var selectorTranslationRuleLoadDeferred = UtilsService.createPromiseDeferred();

                selectorTranslationRuleReadyDeferred.promise.then(function () {
                    var selectorTranslationRulePayload = {};
                    if (routeEntity != undefined && routeEntity.TransRuleId != 0)
                        selectorTranslationRulePayload.selectedIds = routeEntity.TransRuleId;

                    VRUIUtilsService.callDirectiveLoad(selectorTranslationRuleAPI, selectorTranslationRulePayload, selectorTranslationRuleLoadDeferred);
                });

                return selectorTranslationRuleLoadDeferred.promise;
            }

        }

        function insert() {
            $scope.scopeModel.isLoading = true;
     
            return NP_IVSwitch_RouteAPIService.AddRoute(buildRouteObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Route', response, 'Name')) {

                    if ($scope.onRouteAdded != undefined)
                        $scope.onRouteAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function update() {
            $scope.scopeModel.isLoading = true;
        
            return NP_IVSwitch_RouteAPIService.UpdateRoute(buildRouteObjFromScope().Entity).then(function (response) {


                if (VRNotificationService.notifyOnItemUpdated('Route', response, 'Name')) {

                    if ($scope.onRouteUpdated != undefined) {
                        $scope.onRouteUpdated(response.UpdatedObject);
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
                  Entity : { RouteId: routeEntity != undefined ? routeEntity.RouteId : undefined,

                      Description: $scope.scopeModel.description,
                      ChannelsLimit: $scope.scopeModel.channelslimit,
                      LogAlias : $scope.scopeModel.logalias,
                      Host : $scope.scopeModel.host,
                      Port: $scope.scopeModel.port,
                      ConnectionTimeOut :$scope.scopeModel.connectiontimeout,
                      CurrentState: $scope.scopeModel.currentstate.value,
                      CodecProfileId: $scope.scopeModel.codecprofileid,
                      TransRuleId: $scope.scopeModel.translationruleid,
                      WakeUpTime: $scope.scopeModel.wakeuptime,
                      TransportModeId: $scope.scopeModel.transportmodeid.value,
                   },

                  CarrierAccountId:   carrierAccountId,
               
            };
        }
    }

    appControllers.controller('NP_IVSwitch_RouteEditorController', RouteEditorController);

})(appControllers);