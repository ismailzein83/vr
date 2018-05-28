'use strict';

app.directive('vrWhsDealSwapdealSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                onselectionchanged: "=",
                onselectitem: "=",
                ondeselectitem: "=",
                ondeselectallitems: "=",
                selectedvalues: "=",
                isrequired: '=',
                ismultipleselection: '@',
                normalColNum: '@',
                hideremoveicon: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new swapDealSelectorCtor(ctrl, $scope);
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
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            return '<vr-whs-deal-dealdefinition-selector on-ready="onDealDefinitionSelectorReady" isrequired="ctrl.isrequired"' + ' ' + hideremoveicon + ' ' + multipleselection + ' ' + 'normal-col-num="{{ctrl.normalColNum}}"' +
            'selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"></vr-whs-deal-dealdefinition-selector>';

        }

        function swapDealSelectorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var swapDealDirectiveAPI;
            var swapDealReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var dealId;

            function initializeController() {

                $scope.onDealDefinitionSelectorReady = function (api) {
                    swapDealDirectiveAPI = api;
                    swapDealReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var loadSwapDealPromiseDeferred = UtilsService.createPromiseDeferred();

                    swapDealReadyPromiseDeferred.promise.then(function () {
                        var dealDefinitionSelectorPayload = { filter: { Filters: [] } };
                        var swapDealDefinitionFilter = {
                            $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealDefinitionFilter, TOne.WhS.Deal.MainExtensions"
                        };
                        if (payload.selectedIds != undefined)
                            dealDefinitionSelectorPayload.selectedIds = payload.selectedIds;
                        dealDefinitionSelectorPayload.filter.Filters.push(swapDealDefinitionFilter);

                        VRUIUtilsService.callDirectiveLoad(swapDealDirectiveAPI, dealDefinitionSelectorPayload, loadSwapDealPromiseDeferred);
                    });
                    return loadSwapDealPromiseDeferred.promise;
                };

                api.getSelectedIds = function () {
                    return swapDealDirectiveAPI.getSelectedIds();
                };

                api.getSelectedValues = function () {
                    console.log(swapDealDirectiveAPI.getSelectedValues());
                    return swapDealDirectiveAPI.getSelectedValues();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);