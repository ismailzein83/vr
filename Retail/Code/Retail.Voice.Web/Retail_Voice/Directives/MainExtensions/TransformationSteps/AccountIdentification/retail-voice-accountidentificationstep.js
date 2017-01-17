'use strict';

app.directive('retailVoiceAccountidentificationstep', ['UtilsService', 'VRUIUtilsService',
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
                return '/Client/Modules/Retail_Voice/Directives/MainExtensions/TransformationSteps/AccountIdentification/Templates/AccountIdentificationStepTemplate.html';
            }
        };

        function AccountIdentificationStepCtor(ctrl, $scope) {
            var stepPayload;

            //Input Fields
            var rawCDRDirectiveReadyAPI;
            var rawCDRDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var callingNumberDirectiveReadyAPI;
            var callingNumberDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var calledNumberDirectiveReadyAPI;
            var calledNumberDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //Output Fields
            var callingAccountIdDirectiveReadyAPI;
            var callingAccountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var calledAccountIdDirectiveReadyAPI;
            var calledAccountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                //Input Fields
                $scope.scopeModel.onRawCDRReady = function (api) {
                    rawCDRDirectiveReadyAPI = api;
                    rawCDRDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCallingNumberReady = function (api) {
                    callingNumberDirectiveReadyAPI = api;
                    callingNumberDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.scopeModel.onCalledNumberReady = function (api) {
                    calledNumberDirectiveReadyAPI = api;
                    calledNumberDirectiveReadyPromiseDeferred.resolve();
                };

                //Output
                $scope.scopeModel.onCallingAccountIdReady = function (api) {
                    callingAccountIdDirectiveReadyAPI = api;
                    callingAccountIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCalledAccountIdReady = function (api) {
                    calledAccountIdDirectiveReadyAPI = api;
                    calledAccountIdDirectiveReadyPromiseDeferred.resolve();
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

                    //Loading CallingNumber Directive
                    var callingNumberDirectivePromiseDeferred = getCallingNumberDirectiveLoadPromiseDeferred();
                    promises.push(callingNumberDirectivePromiseDeferred.promise);

                    //Loading CalledNumber Directive
                    var calledNumberDirectivePromiseDeferred = getCalledNumberDirectiveLoadPromiseDeferred();
                    promises.push(calledNumberDirectivePromiseDeferred.promise);

                    //Output
                    //Loading CallingAccountId Directive
                    var callingAccountIdDirectivePromiseDeferred = getCallingAccountIdDirectivePromiseDeferred();
                    promises.push(callingAccountIdDirectivePromiseDeferred.promise);

                    //Loading CalledAccountId Directive
                    var calledAccountIdDirectivePromiseDeferred = getCalledAccountIdDirectivePromiseDeferred();
                    promises.push(calledAccountIdDirectivePromiseDeferred.promise);

                    
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

                    function getCallingNumberDirectiveLoadPromiseDeferred() {
                        var callingNumberDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        callingNumberDirectiveReadyPromiseDeferred.promise.then(function () {

                            var callingNumberPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                callingNumberPayload.selectedRecords = payload.stepDetails.CallingNumber;

                            VRUIUtilsService.callDirectiveLoad(callingNumberDirectiveReadyAPI, callingNumberPayload, callingNumberDirectiveLoadPromiseDeferred);
                        });

                        return callingNumberDirectiveLoadPromiseDeferred;
                    }

                    function getCalledNumberDirectiveLoadPromiseDeferred() {
                        var calledNumberDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        calledNumberDirectiveReadyPromiseDeferred.promise.then(function () {

                            var calledNumberPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                calledNumberPayload.selectedRecords = payload.stepDetails.CalledNumber;

                            VRUIUtilsService.callDirectiveLoad(calledNumberDirectiveReadyAPI, calledNumberPayload, calledNumberDirectiveLoadPromiseDeferred);
                        });

                        return calledNumberDirectiveLoadPromiseDeferred;
                    }
                    
                    

                    function getCallingAccountIdDirectivePromiseDeferred() {
                        var callingAccountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        callingAccountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var callingAccountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                callingAccountIdPayload.selectedRecords = payload.stepDetails.CallingAccountId;

                            VRUIUtilsService.callDirectiveLoad(callingAccountIdDirectiveReadyAPI, callingAccountIdPayload, callingAccountIdDirectiveLoadPromiseDeferred);
                        });

                        return callingAccountIdDirectiveLoadPromiseDeferred;
                    }

                    function getCalledAccountIdDirectivePromiseDeferred() {
                        var calledAccountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        calledAccountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var calledAccountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                calledAccountIdPayload.selectedRecords = payload.stepDetails.CalledAccountId;

                            VRUIUtilsService.callDirectiveLoad(calledAccountIdDirectiveReadyAPI, calledAccountIdPayload, calledAccountIdDirectiveLoadPromiseDeferred);
                        });

                        return calledAccountIdDirectiveLoadPromiseDeferred;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Voice.MainExtensions.TransformationSteps.VoiceAccountIdentificationStep, Retail.Voice.MainExtensions",
                        RawCDR: rawCDRDirectiveReadyAPI.getData(),
                        CallingNumber: callingNumberDirectiveReadyAPI.getData(),
                        CalledNumber: calledNumberDirectiveReadyAPI.getData(),
                        CallingAccountId: callingAccountIdDirectiveReadyAPI.getData(),
                        CalledAccountId: calledAccountIdDirectiveReadyAPI.getData()
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