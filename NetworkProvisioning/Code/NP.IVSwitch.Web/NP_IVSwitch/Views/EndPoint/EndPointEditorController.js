(function (appControllers) {

    "use strict";

    EndPointEditorController.$inject = ['$scope', 'NP_IVSwitch_EndPointAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_StateEnum', 'NP_IVSwitch_RtpModeEnum', 'NP_IVSwitch_EndPointEnum'];

    function EndPointEditorController($scope, NP_IVSwitch_EndPointAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum, NP_IVSwitch_RtpModeEnum, NP_IVSwitch_EndPointEnum) {

        var isEditMode;

        var endPointId;
        var carrierAccountId;
        var endPointEntity;


        var selectorCodecProfileAPI;
        var selectorCodecProfileReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTranslationRuleAPI;
        var selectorTranslationRuleReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorEndPointAPI;
        var selectorEndPointReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorDomainAPI;
        var selectorDomainReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        var isViewHistoryMode;

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                endPointId = parameters.EndPointId;
                carrierAccountId = parameters.CarrierAccountId;
                context = parameters.context;
            }

            isEditMode = (endPointId != undefined);
            isViewHistoryMode = (context != undefined && context.historyId != undefined);
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

            $scope.scopeModel.onSelectorCodecProfileReady = function (api) {
                selectorCodecProfileAPI = api;
                selectorCodecProfileReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorTranslationRuleReady = function (api) {
                selectorTranslationRuleAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingRules = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, selectorTranslationRuleAPI, undefined, setLoader, selectorTranslationRuleReadyDeferred);
            };

            $scope.scopeModel.onSelectorEndPointReady = function (api) {
                selectorEndPointAPI = api;
                selectorEndPointReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectorDomainReady = function (api) {
                selectorDomainAPI = api;
                selectorDomainReadyDeferred.resolve();
            };

            $scope.scopeModel.onSelectionChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.TypeId = SelectedItem.value;
                }
            };

            $scope.scopeModel.onSelectionCodecProfileChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.codecprofileid = SelectedItem.CodecProfileId;
                }
            };

            $scope.scopeModel.onSelectionTranslationRuleChanged = function (SelectedItem) {

                if (SelectedItem != undefined) {
                    $scope.scopeModel.translationruleid = SelectedItem.TranslationRuleId;
                }
            else {
                    $scope.scopeModel.translationruleid = undefined;
                }
            };

            $scope.scopeModel.onSelectionEndPointChanged = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.endpointtype = SelectedItem.value;

                    if ($scope.scopeModel.endpointtype == 4) {
                        $scope.scopeModel.ShowSIP = true;
                        $scope.scopeModel.ShowACL = false;
                    }
                    else {
                        $scope.scopeModel.ShowSIP = false;
                        $scope.scopeModel.ShowACL = true;
                    }

                }
            };

            $scope.scopeModel.onSelectionDomainChanged = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.domaineid = SelectedItem.DomainId;
                }
            };

            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.currentstate = SelectedItem;
                }
            };

            $scope.scopeModel.onSelectionChangedRtpMode = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.selectedrtpmode = SelectedItem;
                    $scope.scopeModel.rtpmode = $scope.scopeModel.selectedrtpmode.value;
                }

            };

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
            else if (isViewHistoryMode) {
                getEndPointHistory().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });

            }

            else {
                loadAllControls();

            }
        }

        function getEndPointHistory() {
            return NP_IVSwitch_EndPointAPIService.GetEndPointHistoryDetailbyHistoryId(context.historyId).then(function (response) {
                endPointEntity = response;

            });
        }

        function getEndPoint() {
            return NP_IVSwitch_EndPointAPIService.GetEndPoint(endPointId).then(function (response) {
                endPointEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorCodecProfile, loadSelectorTranslationRule, loadSelectorEndPoint, loadSelectorDomain]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var endPointName = (endPointEntity != undefined) ? endPointEntity.Description : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(endPointName, 'EndPoint');
                }
                else if (isViewHistoryMode && endPointEntity != undefined)
                    $scope.title = "View End Point: " + endPointEntity.Description;
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
                    $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[1];
                    return;
                }
                var splitChars = '/';
                if (endPointEntity.Host.indexOf(splitChars) >= 0) {
                    var dtlStr = endPointEntity.Host.split(splitChars);
                    $scope.scopeModel.host = dtlStr[0];
                    $scope.scopeModel.subnet = dtlStr[1];
                } else $scope.scopeModel.host = endPointEntity.Host;
                $scope.scopeModel.description = endPointEntity.Description;
                $scope.scopeModel.logalias = endPointEntity.LogAlias;
                $scope.scopeModel.channelslimit = endPointEntity.ChannelsLimit;
                $scope.scopeModel.codecprofileid = endPointEntity.CodecProfileId;
                $scope.scopeModel.translationruleid = endPointEntity.TranslationRuleId;
                $scope.scopeModel.channelslimit = endPointEntity.ChannelsLimit;
                $scope.scopeModel.maxcallduration = endPointEntity.MaxCallDuration;
                $scope.scopeModel.domaineid = endPointEntity.DomainId;
                $scope.scopeModel.techprefix = endPointEntity.TechPrefix;
                $scope.scopeModel.siplogin = endPointEntity.SipLogin;
                $scope.scopeModel.sippassword = endPointEntity.SipPassword;
                $scope.scopeModel.endpointtype = endPointEntity.EndPointType;
                $scope.scopeModel.routeTableBasedRule = endPointEntity.RouteTableBasedRule;
                if (endPointEntity.CurrentState != undefined)
                    $scope.scopeModel.currentstate = $scope.scopeModel.states[endPointEntity.CurrentState - 1];
                if (endPointEntity.RtpMode != undefined) {
                    $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[endPointEntity.RtpMode - 1];
                    if ($scope.scopeModel.selectedrtpmode != undefined)
                        $scope.scopeModel.rtpmode = $scope.scopeModel.selectedrtpmode.value;
                }
                else
                    $scope.scopeModel.selectedrtpmode = $scope.scopeModel.rtpmodes[1];

                if (endPointEntity.EndPointType != undefined)
                    $scope.scopeModel.endpointtype = $scope.scopeModel.endpointtypes[endPointEntity.EndPointType];
            }

            function loadSelectorCodecProfile() {
                var selectorCodecProfileLoadDeferred = UtilsService.createPromiseDeferred();

                selectorCodecProfileReadyDeferred.promise.then(function () {
                    var selectorCodecProfilePayload = {};

                    if (endPointEntity != undefined && endPointEntity.CodecProfileId != undefined && endPointEntity.CodecProfileId!=0)
                        selectorCodecProfilePayload.selectedIds = endPointEntity.CodecProfileId;

                    VRUIUtilsService.callDirectiveLoad(selectorCodecProfileAPI, selectorCodecProfilePayload, selectorCodecProfileLoadDeferred);
                });

                return selectorCodecProfileLoadDeferred.promise;
            }

            function loadSelectorTranslationRule() {
                var selectorTranslationRuleLoadDeferred = UtilsService.createPromiseDeferred();
                selectorTranslationRuleReadyDeferred.promise.then(function () {
                    selectorTranslationRuleReadyDeferred = undefined;
                    var selectorTranslationRulePayload = {};
                    if (endPointEntity != undefined && endPointEntity.TransRuleId != undefined && endPointEntity.TransRuleId!=-2 && endPointEntity.TransRuleId!=0)
                        selectorTranslationRulePayload.selectedIds = endPointEntity.TransRuleId;

                    VRUIUtilsService.callDirectiveLoad(selectorTranslationRuleAPI, selectorTranslationRulePayload, selectorTranslationRuleLoadDeferred);
                });

                return selectorTranslationRuleLoadDeferred.promise;
            }

            function loadSelectorEndPoint() {

                var selectorEndPointLoadDeferred = UtilsService.createPromiseDeferred();

                selectorEndPointReadyDeferred.promise.then(function () {
                    var selectorEndPointPayload = {};
                    if (endPointEntity != undefined && endPointEntity.EndPointType != undefined)
                        selectorEndPointPayload.selectedIds = endPointEntity.EndPointType;

                    VRUIUtilsService.callDirectiveLoad(selectorEndPointAPI, selectorEndPointPayload, selectorEndPointLoadDeferred);
                });

                return selectorEndPointLoadDeferred.promise;
            }

            function loadSelectorDomain() {
                var selectorDomainLoadDeferred = UtilsService.createPromiseDeferred();

                selectorDomainReadyDeferred.promise.then(function () {
                    var selectorDomainPayload = {};
                    if (endPointEntity != undefined && endPointEntity.DomainId != undefined)
                        selectorDomainPayload.selectedIds = endPointEntity.DomainId;
                    else selectorDomainPayload.selectedIds = 1;
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
                endPointEntity = undefined;
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
                 TransRuleId: !$scope.scopeModel.routeTableBasedRule ? $scope.scopeModel.translationruleid : null,
                 CurrentState: $scope.scopeModel.currentstate.value,
                 ChannelsLimit: $scope.scopeModel.channelslimit,
                 MaxCallDuration: $scope.scopeModel.maxcallduration,
                 RtpMode: $scope.scopeModel.rtpmode,
                 DomainId: $scope.scopeModel.domaineid,
                 Host: $scope.scopeModel.subnet != undefined ? $scope.scopeModel.host + "/" + $scope.scopeModel.subnet : $scope.scopeModel.host,
                 TechPrefix: $scope.scopeModel.techprefix,
                 SipLogin: $scope.scopeModel.siplogin,
                 SipPassword: $scope.scopeModel.sippassword,
                 EndPointType: $scope.scopeModel.endpointtype,
                 RouteTableBasedRule:$scope.scopeModel.routeTableBasedRule

             },

                CarrierAccountId: carrierAccountId

            };
        }
    }

    appControllers.controller('NP_IVSwitch_EndPointEditorController', EndPointEditorController);

})(appControllers);