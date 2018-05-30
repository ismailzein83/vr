'use strict';

app.directive('vrWhsBeCodesbyzoneMappedvalueOtherrates', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var basicSettingsOtherRatesMappedValue = new BasicSettingsOtherRatesMappedValue($scope, ctrl, $attrs);
            basicSettingsOtherRatesMappedValue.initializeController();
        },
        controllerAs: "codesByZoneOtherRatesMappedValueCtrl",
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function BasicSettingsOtherRatesMappedValue($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;
        var selectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        var selectedRateTypeId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyPromiseDeferred.resolve();
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                var promises = [];
                if (payload != undefined && payload.mappedValue != undefined) {
                    selectedRateTypeId = payload.mappedValue.RateTypeId;
                }
                promises.push(loadRateTypeSelector());
                return UtilsService.waitMultiplePromises(promises).then(function () {
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            api.getData = function getData() {
                return {
                    $type: 'TOne.WhS.BusinessEntity.MainExtensions.CodesByZoneOtherRatesMappedValue, TOne.WhS.BusinessEntity.MainExtensions',
                    RateTypeId: selectorAPI.getSelectedIds()
                };
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function loadRateTypeSelector() {
            var rateTypeSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
            selectorReadyPromiseDeferred.promise.then(function () {
                var payload = {
                    selectedIds: selectedRateTypeId
                };
                VRUIUtilsService.callDirectiveLoad(selectorAPI, payload, rateTypeSelectorLoadPromiseDeferred);
            });
            return rateTypeSelectorLoadPromiseDeferred.promise;
        }
    }

    function getTemplate() {
        return '<vr-columns colnum="{{codesByZoneOtherRatesMappedValueCtrl.normalColNum}}"><vr-common-ratetype-selector on-ready="scopeModel.onSelectorReady" hidelabel selectedvalues="scopeModel.selectedOtherRate" isrequired = "true"> </vr-common-ratetype-selector></vr-columns>';

    }
}]);