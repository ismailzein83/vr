﻿'use strict';

app.directive('vrWhsDealSwapdealSelector', ['UtilsService', 'VRUIUtilsService', 'WhS_Deal_SwapDealAPIService',
    function (UtilsService, VRUIUtilsService, WhS_Deal_SwapDealAPIService) {

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
                var ctor = new swapDealSelectorCtor(ctrl, $scope, $attrs);
                if ($attrs.ismultipleselection != undefined)
                    ctrl.selectedvalues = [];
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


            return '<vr-whs-deal-dealdefinition-selector  includeviewhandler hasviewpremission="hasViewSwapDealPermission"  on-ready="onDealDefinitionSelectorReady" isrequired="ctrl.isrequired"' + ' ' + hideremoveicon + ' ' + multipleselection + ' ' + 'normal-col-num="{{ctrl.normalColNum}}"' +
                'selectedvalues="ctrl.selectedvalues" onselectionchanged="ctrl.onselectionchanged"></vr-whs-deal-dealdefinition-selector>';

        }

        function swapDealSelectorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var swapDealDirectiveAPI;
            var swapDealReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onDealDefinitionSelectorReady = function (api) {
                    swapDealDirectiveAPI = api;
                    swapDealReadyPromiseDeferred.resolve();
                };
                $scope.hasViewSwapDealPermission = function () {
                    return WhS_Deal_SwapDealAPIService.HasViewSwapDealPermission();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var loadSwapDealPromiseDeferred = UtilsService.createPromiseDeferred();
                    swapDealReadyPromiseDeferred.promise.then(function () {
                        var dealDefinitionSelectorPayload = { filter: { Filters: [] } };

                        if (payload != undefined && payload.filter!=undefined) {
                            if ($attrs.ismultipleselection != undefined)
                                ctrl.selectedvalues = [];
                            else 
                                ctrl.selectedvalues = undefined;
                            dealDefinitionSelectorPayload.filter.Filters.push(payload.filter);
                        }

                        else {
                            var swapDealDefinitionFilter = {
                                $type: "TOne.WhS.Deal.MainExtensions.SwapDeal.SwapDealDefinitionFilter, TOne.WhS.Deal.MainExtensions"
                            };

                            dealDefinitionSelectorPayload.filter.Filters.push(swapDealDefinitionFilter);
                        }
                        if (payload != undefined && payload.selectedIds != undefined) {
                            dealDefinitionSelectorPayload.selectedIds = payload.selectedIds;
                        }
                        VRUIUtilsService.callDirectiveLoad(swapDealDirectiveAPI, dealDefinitionSelectorPayload, loadSwapDealPromiseDeferred);

                    });
                    return loadSwapDealPromiseDeferred.promise;
                };

                api.getSelectedIds = function () {
                    return swapDealDirectiveAPI.getSelectedIds();
                };
                api.clearDataSource = function () {
                    swapDealDirectiveAPI.clearDataSource();
                };
                api.getSelectedValues = function () {
                    return swapDealDirectiveAPI.getSelectedValues();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);