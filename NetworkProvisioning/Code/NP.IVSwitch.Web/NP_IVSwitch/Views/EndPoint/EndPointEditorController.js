(function (appControllers) {

    "use strict";

    EndPointEditorController.$inject = ['$scope', 'NP_IVSwitch_EndPointAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_StateEnum', 'NP_IVSwitch_TraceEnum', 'NP_IVSwitch_RtpModeEnum', 'NP_IVSwitch_EndPointEnum'];

    function EndPointEditorController($scope, NP_IVSwitch_EndPointAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum, NP_IVSwitch_TraceEnum, NP_IVSwitch_RtpModeEnum, NP_IVSwitch_EndPointEnum) {

        var isEditMode;

        var endPointId;
        var carrierAccountId;
        var endPointEntity;

 
        var selectorCodecProfileAPI;
        var selectorCodecProfileReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTranslationRuleAPI;
        var selectorTranslationRuleReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTariffAPI;
        var selectorTariffReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorEndPointAPI;
        var selectorEndPointReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorRouteTableAPI;
        var selectorRouteTableReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorDomainAPI;
        var selectorDomainReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                endPointId = parameters.EndPointId;
                carrierAccountId = parameters.CarrierAccountId;
            }

            isEditMode = (endPointId != undefined);
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

            $scope.scopeModel.onSelectorTariffReady = function (api) {
                selectorTariffAPI = api;
                selectorTariffReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorEndPointReady = function (api) {
                 selectorEndPointAPI = api; 
                selectorEndPointReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorRouteTableReady = function (api) {
                selectorRouteTableAPI = api;
                selectorRouteTableReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorDomainReady = function (api) {
                selectorDomainAPI = api;
                selectorDomainReadyDeferred.resolve();
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

            $scope.scopeModel.onSelectionTariffChanged = function (SelectedItem) {

                 if (SelectedItem != undefined) {
                    $scope.scopeModel.tariffid = SelectedItem.TariffId;
                }
            }

            $scope.scopeModel.onSelectionEndPointChanged = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.endpointtype = SelectedItem.value;

                    if ($scope.scopeModel.endpointtype == 0) {
                        $scope.scopeModel.ShowSIP = true;
                        $scope.scopeModel.ShowACL = false;
                    }
                    else {
                        $scope.scopeModel.ShowSIP = false;
                        $scope.scopeModel.ShowACL = true;
                    }

                 }
            }

            $scope.scopeModel.onSelectionRouteTableChanged = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.routetableid = SelectedItem.RouteTableId;
                }
            }

            $scope.scopeModel.onSelectionDomainChanged = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.domaineid = SelectedItem.DomainId;
                 }
            }

            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.currentstate = SelectedItem;
                }
            }

            $scope.scopeModel.onSelectionChangedRtpMode = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.selectedrtpmode = SelectedItem;
                    $scope.scopeModel.rtpmode = $scope.scopeModel.selectedrtpmode.value
                }
 
            }

        }
        function load() {

            $scope.scopeModel.isLoading = true;
            if (isEditMode) {  
                getEndPoint().then(function () {
                    $scope.scopeModel.DisableSelector = true;
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



        function getEndPoint() {
            return NP_IVSwitch_EndPointAPIService.GetEndPoint(endPointId).then(function (response) {
                endPointEntity = response;
             });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorCodecProfile, loadSelectorTranslationRule, loadSelectorTariff, loadSelectorEndPoint, loadSelectorRouteTable, loadSelectorDomain]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var endPointName = (endPointEntity != undefined) ? endPointEntity.Description : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(endPointName, 'EndPoint');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('EndPoint');
                }
            }
            function loadStaticData() {

                $scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_StateEnum);
 
                $scope.scopeModel.rtpmodes = UtilsService.getArrayEnum(NP_IVSwitch_RtpModeEnum);

                $scope.scopeModel.endpointtypes = UtilsService.getArrayEnum(NP_IVSwitch_EndPointEnum);

               
                if (endPointEntity == undefined) {

                    $scope.scopeModel.currentstate = $scope.scopeModel.states[0];
                    $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[0];

                    return;
                }

                $scope.scopeModel.description = endPointEntity.Description;
                $scope.scopeModel.logalias = endPointEntity.LogAlias;
                $scope.scopeModel.channelslimit = endPointEntity.ChannelsLimit;
                $scope.scopeModel.host = endPointEntity.Host;

                $scope.scopeModel.tariffid = endPointEntity.TariffId;
                $scope.scopeModel.codecprofileid = endPointEntity.CodecProfileId;
                $scope.scopeModel.translationruleid = endPointEntity.TranslationRuleId;
                $scope.scopeModel.channelslimit = endPointEntity.ChannelsLimit;
                $scope.scopeModel.routetableid = endPointEntity.RouteTableId;
                $scope.scopeModel.maxcallduration = endPointEntity.MaxCallDuration;
                 $scope.scopeModel.domaineid = endPointEntity.DomainId;
                $scope.scopeModel.techprefix = endPointEntity.TechPrefix;
                $scope.scopeModel.siplogin = endPointEntity.SipLogin;
                $scope.scopeModel.sippassword = endPointEntity.SipPassword;
                $scope.scopeModel.endpointtype = endPointEntity.EndPointType;
  
                if (endPointEntity.CurrentState != undefined)
                    $scope.scopeModel.currentstate = $scope.scopeModel.states[endPointEntity.CurrentState - 1];
                if (endPointEntity.RtpMode != undefined) {
                     $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[endPointEntity.RtpMode - 1];
                    if ($scope.scopeModel.selectedrtpmode != undefined)
                        $scope.scopeModel.rtpmode = $scope.scopeModel.selectedrtpmode.value;
                }
                else
                    $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[0];

                if (endPointEntity.EndPointType != undefined)
                    $scope.scopeModel.endpointtype = $scope.scopeModel.endpointtypes[endPointEntity.EndPointType];
             }

            function loadSelectorCodecProfile() {
                var selectorCodecProfileLoadDeferred = UtilsService.createPromiseDeferred();

                selectorCodecProfileReadyDeferred.promise.then(function () {
                    var selectorCodecProfilePayload = {};

                    if (endPointEntity != undefined && endPointEntity.CodecProfileId != undefined)
                        selectorCodecProfilePayload.selectedIds = endPointEntity.CodecProfileId;

                    VRUIUtilsService.callDirectiveLoad(selectorCodecProfileAPI, selectorCodecProfilePayload, selectorCodecProfileLoadDeferred);
                });

                return selectorCodecProfileLoadDeferred.promise;
            }

            function loadSelectorTranslationRule() {
                var selectorTranslationRuleLoadDeferred = UtilsService.createPromiseDeferred();

                selectorTranslationRuleReadyDeferred.promise.then(function () {
                    var selectorTranslationRulePayload = {};
                    if (endPointEntity != undefined && endPointEntity.TransRuleId != undefined)
                        selectorTranslationRulePayload.selectedIds = endPointEntity.TransRuleId;

                    VRUIUtilsService.callDirectiveLoad(selectorTranslationRuleAPI, selectorTranslationRulePayload, selectorTranslationRuleLoadDeferred);
                });

                return selectorTranslationRuleLoadDeferred.promise;
            }

            function loadSelectorTariff() {
                var selectorTariffLoadDeferred = UtilsService.createPromiseDeferred();

                selectorTariffReadyDeferred.promise.then(function () {
                    var selectorTariffPayload = {};
                    if (endPointEntity != undefined && endPointEntity.TariffId != undefined)
                        selectorTariffPayload.selectedIds = endPointEntity.TariffId;

                    VRUIUtilsService.callDirectiveLoad(selectorTariffAPI, selectorTariffPayload, selectorTariffLoadDeferred);
                });

                return selectorTariffLoadDeferred.promise;
            }

            function loadSelectorEndPoint() {

                var selectorEndPointLoadDeferred = UtilsService.createPromiseDeferred();
 
                selectorEndPointReadyDeferred.promise.then(function () {
                    var selectorEndPointPayload = {};
                    if (endPointEntity != undefined && endPointEntity.EndPointType!= undefined)
                        selectorEndPointPayload.selectedIds = endPointEntity.EndPointType;
                    
                    VRUIUtilsService.callDirectiveLoad(selectorEndPointAPI, selectorEndPointPayload, selectorEndPointLoadDeferred);
                });

                return selectorEndPointLoadDeferred.promise;
            }

            function loadSelectorRouteTable() {
                var selectorRouteTableLoadDeferred = UtilsService.createPromiseDeferred();

                selectorRouteTableReadyDeferred.promise.then(function () {
                    var selectorRouteTablePayload = {};
                    if (endPointEntity != undefined && endPointEntity.RouteTableId != undefined)
                        selectorRouteTablePayload.selectedIds = endPointEntity.RouteTableId;
                    
                    VRUIUtilsService.callDirectiveLoad(selectorRouteTableAPI, selectorRouteTablePayload, selectorRouteTableLoadDeferred);
                });

                return selectorRouteTableLoadDeferred.promise;
            }

            function loadSelectorDomain() {
                var selectorDomainLoadDeferred = UtilsService.createPromiseDeferred();

                selectorDomainReadyDeferred.promise.then(function () {
                    var selectorDomainPayload = {};
                    if (endPointEntity != undefined && endPointEntity.DomainId != undefined)
                        selectorDomainPayload.selectedIds = endPointEntity.DomainId;

                    VRUIUtilsService.callDirectiveLoad(selectorDomainAPI, selectorDomainPayload, selectorDomainLoadDeferred);
                });

                return selectorDomainLoadDeferred.promise;
            }
        }

        function insert() {
            $scope.scopeModel.isLoading = true;

            
            return NP_IVSwitch_EndPointAPIService.AddEndPoint(buildEndPointObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('EndPoint', response, 'Name')) {

                    if ($scope.onEndPointAdded != undefined)
                        $scope.onEndPointAdded(response.InsertedObject);
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
           //     AccountId: accountId,
                Description: $scope.scopeModel.description,
                TariffId: $scope.scopeModel.tariffid,
                LogAlias: $scope.scopeModel.logalias,
                CodecProfileId: $scope.scopeModel.codecprofileid,
                TransRuleId: $scope.scopeModel.translationruleid,
                CurrentState: $scope.scopeModel.currentstate.value,
                ChannelsLimit: $scope.scopeModel.channelslimit,
                RouteTableId : $scope.scopeModel.routetableid,
                MaxCallDuration : $scope.scopeModel.maxcallduration,
                RtpMode: $scope.scopeModel.rtpmode ,
                DomainId :$scope.scopeModel.domaineid,
                Host: $scope.scopeModel.host,
                TechPrefix: $scope.scopeModel.techprefix,
                SipLogin: $scope.scopeModel.siplogin,
                SipPassword: $scope.scopeModel.sippassword,
                EndPointType: $scope.scopeModel.endpointtype,
             },

                 CarrierAccountId:   carrierAccountId,
               
            };
        }
    }

    appControllers.controller('NP_IVSwitch_EndPointEditorController', EndPointEditorController);

})(appControllers);