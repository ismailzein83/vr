'use strict';

app.directive('retailVoiceInternationalidentificationstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AccountIdentificationStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_Voice/Directives/MainExtensions/TransformationSteps/InternationalIdentification/Templates/InternationalIdentificationStepTemplate.html';
            }
        };

        function AccountIdentificationStepCtor(ctrl, $scope) {
            var stepPayload;

            //Input Fields
            var rawCDRDirectiveReadyAPI;
            var rawCDRDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var otherPartyNumberDirectiveReadyAPI;
            var otherPartyNumberDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //Output Fields
            var isInternationalDirectiveReadyAPI;
            var isInternationalDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                //Input Fields
                $scope.scopeModel.onRawCDRReady = function (api) {
                    rawCDRDirectiveReadyAPI = api;
                    rawCDRDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onOtherPartyNumberReady = function (api) {
                    otherPartyNumberDirectiveReadyAPI = api;
                    otherPartyNumberDirectiveReadyPromiseDeferred.resolve();
                };

                //Output
                $scope.scopeModel.onIsInternationalReady = function (api) {
                    isInternationalDirectiveReadyAPI = api;
                    isInternationalDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Input
                    //Loading RawCDR Directive
                    var rawCDRDirectivePromiseDeferred = getRawCDRDirectiveLoadPromiseDeferred();
                    promises.push(rawCDRDirectivePromiseDeferred.promise);

                    //Loading OtherPartyNumber Directive
                    var otherPartyNumberDirectivePromiseDeferred = getOtherPartyNumberDirectiveLoadPromiseDeferred();
                    promises.push(otherPartyNumberDirectivePromiseDeferred.promise);

                    //Output
                    //Loading CallingAccountId Directive
                    var isInternationalDirectivePromiseDeferred = getIsInternationalDirectivePromiseDeferred();
                    promises.push(isInternationalDirectivePromiseDeferred.promise);

                    function getRawCDRDirectiveLoadPromiseDeferred() {
                        var rawCDRDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rawCDRDirectiveReadyPromiseDeferred.promise.then(function () {

                            var rawCDRPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                rawCDRPayload.selectedRecords = payload.stepDetails.RawCDR;

                            VRUIUtilsService.callDirectiveLoad(rawCDRDirectiveReadyAPI, rawCDRPayload, rawCDRDirectiveLoadPromiseDeferred);
                        });

                        return rawCDRDirectiveLoadPromiseDeferred;
                    }

                    function getOtherPartyNumberDirectiveLoadPromiseDeferred() {
                        var otherPartyNumberDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        otherPartyNumberDirectiveReadyPromiseDeferred.promise.then(function () {

                            var otherPartyNumberPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                otherPartyNumberPayload.selectedRecords = payload.stepDetails.OtherPartyNumber;

                            VRUIUtilsService.callDirectiveLoad(otherPartyNumberDirectiveReadyAPI, otherPartyNumberPayload, otherPartyNumberDirectiveLoadPromiseDeferred);
                        });

                        return otherPartyNumberDirectiveLoadPromiseDeferred;
                    }

                    function getIsInternationalDirectivePromiseDeferred() {
                        var isInternationalDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        isInternationalDirectiveReadyPromiseDeferred.promise.then(function () {

                            var isInternationalPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                isInternationalPayload.selectedRecords = payload.stepDetails.IsInternational;

                            VRUIUtilsService.callDirectiveLoad(isInternationalDirectiveReadyAPI, isInternationalPayload, isInternationalDirectiveLoadPromiseDeferred);
                        });

                        return isInternationalDirectiveLoadPromiseDeferred;
                    }
                    
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Voice.MainExtensions.TransformationSteps.VoiceInternationalIdentificationStep, Retail.Voice.MainExtensions",
                        RawCDR: rawCDRDirectiveReadyAPI.getData(),
                        OtherPartyNumber: otherPartyNumberDirectiveReadyAPI.getData(),
                        IsInternational: isInternationalDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);