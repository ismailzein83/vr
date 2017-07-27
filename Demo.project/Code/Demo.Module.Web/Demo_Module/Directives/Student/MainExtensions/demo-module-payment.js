(function (appControllers) {
"use strict";
demoModulePaymentDirective.$inject = ["UtilsService", "VRUIUtilsService", "Demo_Module_PaymentConfigsAPIService"];
function demoModulePaymentDirective(UtilsService, VRUIUtilsService, Demo_Module_PaymentConfigsAPIService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            isrequired: '=',
            label: '@',
            normalColNum: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var payment = new Payment($scope, ctrl, $attrs);
            payment.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTamplate(attrs);
        }
    };
    function Payment($scope, ctrl, $attrs) {
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
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, undefined, setLoader, directiveReadyDeferred);
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();
                var promises = [];
                var paymentTypeEntity;
                if (payload != undefined) {
                    paymentTypeEntity = payload;
                }

                if (paymentTypeEntity != undefined) {
                    var loadDirectivePromise = loadDirective();
                    promises.push(loadDirectivePromise);
                }

                var getPaymentTypeTemplateConfigsPromise = GetPaymentTypeTemplateConfigs();
                promises.push(getPaymentTypeTemplateConfigsPromise);

                function GetPaymentTypeTemplateConfigs() {
                    return Demo_Module_PaymentConfigsAPIService.GetPaymentTypeTemplateConfigs().then(function (response) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $scope.scopeModel.templateConfigs.push(response[i]);
                            }
                            if (paymentTypeEntity != undefined) {
                                $scope.scopeModel.selectedTemplateConfig =
                                    UtilsService.getItemByVal($scope.scopeModel.templateConfigs, paymentTypeEntity.ConfigId, 'ExtensionConfigurationId');
                            }
                        }
                    });
                }
                function loadDirective() {
                    directiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var directiveLoadDeferred = UtilsService.createPromiseDeferred();

                    directiveReadyDeferred.promise.then(function () {
                        var directivePayload = paymentTypeEntity;
                        VRUIUtilsService.callDirectiveLoad(directiveAPI, directivePayload, directiveLoadDeferred);
                        directiveReadyDeferred = undefined;
                    });

                    return directiveLoadDeferred.promise;
                }
                return UtilsService.waitMultiplePromises(promises);
            };
            api.getData = function () {
                var data;
                if ($scope.scopeModel.selectedTemplateConfig != undefined && directiveAPI != undefined) {

                    data = directiveAPI.getData();
                    if (data != undefined) {
                        data.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
                    }
                }
                return data;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    function getTamplate(attrs) {
        var hideremoveicon = '';
        if (attrs.hideremoveicon != undefined) {
            hideremoveicon = 'hideremoveicon';
        }
        var template =
            '<vr-row removeline>'
             + '<vr-columns colnum="{{ctrl.normalColNum}}" >'
                    + ' <vr-select on-ready="scopeModel.onSelectorReady"'
                        + ' datasource="scopeModel.templateConfigs"'
                        + ' selectedvalues="scopeModel.selectedTemplateConfig"'
                        + ' datavaluefield="ExtensionConfigurationId"'
                        + ' datatextfield="Title"'
                        + 'label="Payment Type" '
                        + ' ' + hideremoveicon + ' '
                         + 'isrequired ="ctrl.isrequired"'
                       + ' >'
                    + '</vr-select>'
                + ' </vr-columns>'
                + '</vr-row>'
            + '<vr-directivewrapper ng-if="scopeModel.selectedTemplateConfig != undefined" directive="scopeModel.selectedTemplateConfig.Editor"'
                    + 'on-ready="scopeModel.onDirectiveReady" normal-col-num="{{ctrl.normalColNum}}" isrequired="ctrl.isrequired">'
            + '</vr-directivewrapper>'

        return template;
    }
}
        app.directive('demoModulePayment', demoModulePaymentDirective);

    })(app);