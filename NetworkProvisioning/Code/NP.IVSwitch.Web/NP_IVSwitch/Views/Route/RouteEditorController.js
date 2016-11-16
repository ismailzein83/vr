(function (appControllers) {

    "use strict";

    RouteEditorController.$inject = ['$scope', 'NP_IVSwitch_RouteAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_StateEnum','NP_IVSwitch_TraceEnum'];

    function RouteEditorController($scope, NP_IVSwitch_RouteAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum, NP_IVSwitch_TraceEnum) {

        var isEditMode;
 
        var routeId;
        var accountId;
        var routeEntity;

        var selectorDirectiveAPI;
        var selectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
 
            if (parameters != undefined && parameters != null) {
                routeId = parameters.routeId;
                accountId = parameters.AccountId;
               }


            isEditMode = (routeId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

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

            $scope.scopeModel.onSelectorDirectiveReady = function (api) {
                selectorDirectiveAPI = api;
                selectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectionChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.TypeId = SelectedItem.value;
                }
            }

            $scope.scopeModel.onSelectionCodecProfileChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.codecprofileid = SelectedItem.CodecProfileId;
                    console.log(SelectedItem)
                }
            }

            

            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.currentstate = SelectedItem;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var routeName = (routeEntity != undefined) ? routeEntity.FirstName : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(routeName, 'Route');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('Route');
                }
            }
            function loadStaticData() {

                $scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_StateEnum);
                $scope.scopeModel.trace = UtilsService.getArrayEnum(NP_IVSwitch_TraceEnum);

                $scope.scopeModel.currenttrace = $scope.scopeModel.trace[0]; // always disabled

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
                $scope.scopeModel.pscore = routeEntity.PScore;
            
                 $scope.scopeModel.currentstate = $scope.scopeModel.states[routeEntity.CurrentState - 1];
             }

            function loadSelectorDirective() {
                var selectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                selectorDirectiveReadyDeferred.promise.then(function () {
                    var selectorDirectivePayload;
                     
                    VRUIUtilsService.callDirectiveLoad(selectorDirectiveAPI, selectorDirectivePayload, selectorDirectiveLoadDeferred);
                });

                return selectorDirectiveLoadDeferred.promise;
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
       

            return NP_IVSwitch_RouteAPIService.UpdateRoute(buildRouteObjFromScope()).then(function (response) {


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
                RouteId: routeEntity != undefined ? routeEntity.RouteId : undefined,
                AccountId: accountId ,
                Description :$scope.scopeModel.description,
                ChannelsLimit: $scope.scopeModel.channelslimit,
                LogAlias : $scope.scopeModel.logalias,
                Host : $scope.scopeModel.host,
                Port: $scope.scopeModel.port,
                ConnectionTimeOut :$scope.scopeModel.connectiontimeout,
                PScore:$scope.scopeModel.pscore,                
                CurrentState: $scope.scopeModel.currentstate.value,
                EnableTrace: $scope.scopeModel.currenttrace.value,
                CodecProfileId : $scope.scopeModel.codecprofileid,

              

            };
        }
    }

    appControllers.controller('NP_IVSwitch_RouteEditorController', RouteEditorController);

})(appControllers);