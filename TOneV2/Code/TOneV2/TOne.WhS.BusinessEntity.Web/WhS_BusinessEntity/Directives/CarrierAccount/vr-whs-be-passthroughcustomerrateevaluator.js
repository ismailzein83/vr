'use strict';

app.directive('vrWhsBePassthroughcustomerrateevaluator', ['UtilsService', 'VRUIUtilsService', 'WhS_BE_CarrierAccountAPIService',
	function (UtilsService, VRUIUtilsService, WhS_BE_CarrierAccountAPIService) {
	    return {
	        restrict: 'E',
	        scope: {
	            onReady: '=',
	            selectedvalues: '=',
	            onselectionchanged: '=',
	            onselectitem: '=',
	            ondeselectitem: '=',
	            isrequired: '=',
	            normalColNum: '@'
	        },
	        controller: function ($scope, $element, $attrs) {

	            var ctrl = this;
	            ctrl.datasource = [];
	            ctrl.selectedvalues;

	            var ctor = new PassThroughCustomerRateEvaluatorCtor(ctrl, $scope, $attrs);
	            ctor.initializeController();
	        },
	        controllerAs: 'PassThroughCustomerRateEvaluatorCtrl',
	        bindToController: true,
	        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CarrierAccount/Templates/PassThroughCustomerRateEvaluatorTemplate.html"
	    };

	    function PassThroughCustomerRateEvaluatorCtor(ctrl, $scope, attrs) {
	        this.initializeController = initializeController;

	        var selectorAPI;

	        var directiveAPI;
	        var directiveReadyDeferred;

	        function initializeController() {
	            $scope.scopeModel = {};
	            $scope.scopeModel.templateConfigs = [];
	            $scope.scopeModel.selectedTemplateConfig;

	            $scope.scopeModel.onSelectorReady = function (api) {
	                selectorAPI = api;
	                defineAPI();
	            };

	            $scope.scopeModel.onDirectiveReady = function (api) {
	                directiveAPI = api;
	                var setLoader = function (value) {
	                    $scope.scopeModel.isLoadingDirective = value;
	                };
	                var directivePayload;
	                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, directivePayload, setLoader, directiveReadyDeferred);
	            };
	        }

	        function defineAPI() {
	            var api = {};

	            api.load = function (payload) {
	                selectorAPI.clearDataSource();

	                var passThroughCustomerRateEvaluator;

	                if (payload != undefined) {
	                    passThroughCustomerRateEvaluator = payload.passThroughCustomerRateEvaluator;
	                }

	                var promises = [];

	                var passThroughCustomerRateExtensionConfigsLoadPromise = getPassThroughCustomerRateExtensionConfigs();
	                promises.push(passThroughCustomerRateExtensionConfigsLoadPromise);
	                
	                if (passThroughCustomerRateEvaluator != undefined) {
	                    var loadDirectivePromise = loadDirective();
	                    promises.push(loadDirectivePromise);
	                }

	                function getPassThroughCustomerRateExtensionConfigs() {
	                    return WhS_BE_CarrierAccountAPIService.GetPassThroughCustomerRateExtensionConfigs().then(function (response) {
	                        if (response != null) {
	                            for (var i = 0; i < response.length; i++) {
	                                $scope.scopeModel.templateConfigs.push(response[i]);
	                            }
	                            if (passThroughCustomerRateEvaluator != undefined) {
	                                $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, passThroughCustomerRateEvaluator.ConfigId, 'ExtensionConfigurationId');
	                            }
	                        }
	                    });
	                }

	                function loadDirective() {
	                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

	                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

	                    directiveReadyDeferred.promise.then(function () {
	                        directiveReadyDeferred = undefined;

	                        var directivePayload;
	                        if (payload != undefined) {
	                            directivePayload = payload.passThroughCustomerRateEvaluator;
	                        }
	                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
	                    });

	                    return directiveLoadDeferred.promise;
	                }

	                return UtilsService.waitMultiplePromises(promises);
	            };

	            api.getData = function () {
	                var data;
	                if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {
	                    data = directiveAPI.getData();
	                    if (data != undefined)
	                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
	                }
	                return data;
	            };

	            if (ctrl.onReady != null)
	                ctrl.onReady(api);
	        }
	    }
	}]);