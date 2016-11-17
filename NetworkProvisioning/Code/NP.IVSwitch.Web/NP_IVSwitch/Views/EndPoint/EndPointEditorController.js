(function (appControllers) {

    "use strict";

    EndPointEditorController.$inject = ['$scope', 'NP_IVSwitch_EndPointAPIService', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'VRUIUtilsService', 'NP_IVSwitch_StateEnum', 'NP_IVSwitch_TraceEnum'];

    function EndPointEditorController($scope, NP_IVSwitch_EndPointAPIService, VRNotificationService, UtilsService, VRNavigationService, VRUIUtilsService, NP_IVSwitch_StateEnum, NP_IVSwitch_TraceEnum) {

        var isEditMode;

        var endPointId;
        var accountId;
        var endPointEntity;

        var selectorCodecProfileAPI;
        var selectorCodecProfileReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTranslationRuleAPI;
        var selectorTranslationRuleReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorTariffAPI;
        var selectorTariffReadyDeferred = UtilsService.createPromiseDeferred();

        var selectorEndPointAPI;
        var selectorEndPointReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();

        defineScope();

        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined && parameters != null) {
                endPointId = parameters.EndPointId;
                accountId = parameters.AccountId;
            }

            isEditMode = (endPointId != undefined);
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


            $scope.scopeModel.onSelectionChangedState = function (SelectedItem) {
                if (SelectedItem != undefined) {
                    $scope.scopeModel.currentstate = SelectedItem;
                }
            }

        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getEndPoint().then(function () {
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadSelectorCodecProfile, loadSelectorTranslationRule, loadSelectorTariff, loadSelectorEndPoint]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

            function setTitle() {
                if (isEditMode) {
                    var endPointName = (endPointEntity != undefined) ? endPointEntity.FirstName : null;
                    $scope.title = UtilsService.buildTitleForUpdateEditor(endPointName, 'EndPoint');
                }
                else {
                    $scope.title = UtilsService.buildTitleForAddEditor('EndPoint');
                }
            }
            function loadStaticData() {

                $scope.scopeModel.states = UtilsService.getArrayEnum(NP_IVSwitch_StateEnum);
                $scope.scopeModel.trace = UtilsService.getArrayEnum(NP_IVSwitch_TraceEnum);

                $scope.scopeModel.currenttrace = $scope.scopeModel.trace[0]; // always disabled

                if (endPointEntity == undefined) {

                    $scope.scopeModel.currentstate = $scope.scopeModel.states[0];

                    return;
                }
                $scope.scopeModel.description = endPointEntity.Description;
                $scope.scopeModel.logalias = endPointEntity.LogAlias;

                $scope.scopeModel.channelslimit = endPointEntity.ChannelsLimit;
                $scope.scopeModel.host = endPointEntity.Host;
                $scope.scopeModel.port = endPointEntity.Port;
                $scope.scopeModel.connectiontimeout = endPointEntity.ConnectionTimeOut;
                $scope.scopeModel.pscore = endPointEntity.PScore;

                $scope.scopeModel.currentstate = $scope.scopeModel.states[endPointEntity.CurrentState - 1];
                $scope.scopeModel.wakeuptime = endPointEntity.WakeUpTime;
            }

            function loadSelectorCodecProfile() {
                var selectorCodecProfileLoadDeferred = UtilsService.createPromiseDeferred();

                selectorCodecProfileReadyDeferred.promise.then(function () {
                    var selectorCodecProfilePayload = {};

                    if (endPointEntity != undefined && endPointEntity.CodecProfileId != 0)
                        selectorCodecProfilePayload.selectedIds = endPointEntity.CodecProfileId;

                    VRUIUtilsService.callDirectiveLoad(selectorCodecProfileAPI, selectorCodecProfilePayload, selectorCodecProfileLoadDeferred);
                });

                return selectorCodecProfileLoadDeferred.promise;
            }

            function loadSelectorTranslationRule() {
                var selectorTranslationRuleLoadDeferred = UtilsService.createPromiseDeferred();

                selectorTranslationRuleReadyDeferred.promise.then(function () {
                    var selectorTranslationRulePayload = {};
                    if (endPointEntity != undefined && endPointEntity.TransRuleId != 0)
                        selectorTranslationRulePayload.selectedIds = endPointEntity.TransRuleId;

                    VRUIUtilsService.callDirectiveLoad(selectorTranslationRuleAPI, selectorTranslationRulePayload, selectorTranslationRuleLoadDeferred);
                });

                return selectorTranslationRuleLoadDeferred.promise;
            }

            function loadSelectorTariff() {
                var selectorTariffLoadDeferred = UtilsService.createPromiseDeferred();

                selectorTariffReadyDeferred.promise.then(function () {
                    var selectorTariffPayload = {};
                    if (endPointEntity != undefined && endPointEntity.TariffId != 0)
                        selectorTariffPayload.selectedIds = endPointEntity.TariffId;

                    VRUIUtilsService.callDirectiveLoad(selectorTariffAPI, selectorTariffPayload, selectorTariffLoadDeferred);
                });

                return selectorTariffLoadDeferred.promise;
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
                EndPointId: endPointEntity != undefined ? endPointEntity.EndPointId : undefined,
                AccountId: accountId,
                Description: $scope.scopeModel.description,
                ChannelsLimit: $scope.scopeModel.channelslimit,
                LogAlias: $scope.scopeModel.logalias,
                Host: $scope.scopeModel.host,
                CurrentState: $scope.scopeModel.currentstate.value,
                CodecProfileId: $scope.scopeModel.codecprofileid,
                TransRuleId: $scope.scopeModel.translationruleid,
                TariffId: $scope.scopeModel.tariffid,
                LastHitDate: $scope.scopeModel.lasthittime,

            };
        }
    }

    appControllers.controller('NP_IVSwitch_EndPointEditorController', EndPointEditorController);

})(appControllers);