'use strict';

app.directive('vrWhsBeCustomerSelector', ['UtilsService', 'VRUIUtilsService',
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
                isdisabled:'=',
                ismultipleselection: '@',
                normalColNum: '@',
                hideremoveicon: "@",
                customlabel: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new customerSelectorCtor(ctrl, $scope);
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

            var multipleselection = "";
            if (attrs.ismultipleselection != undefined)
                multipleselection = "ismultipleselection";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";

            var hideselectedvaluessection = "";
            if (attrs.hideselectedvaluessection != undefined)
                hideselectedvaluessection = "hideselectedvaluessection";

            var hidelabel = "";
            if (attrs.hidelabel != undefined)
                hidelabel = "hidelabel";

            var customlabel = '';
            if (attrs.customlabel != undefined)
                customlabel = 'customlabel="{{ctrl.customlabel}}"';

            var usefullcolumn = "";
            if (attrs.usefullcolumn != undefined)
                usefullcolumn = "usefullcolumn";

            return '<span vr-disabled="ctrl.isdisabled"><vr-whs-be-carrieraccount-selector on-ready="onCarrierAccountDirectiveReady" onselectionchanged="ctrl.onselectionchanged" selectedvalues="ctrl.selectedvalues" ' +
                         ' onselectitem="ctrl.onselectitem" ondeselectitem="ctrl.ondeselectitem" ondeselectallitems="ctrl.ondeselectallitems" isrequired="ctrl.isrequired"  normal-col-num="{{ctrl.normalColNum}}" getcustomers '
                         + multipleselection + ' ' + hideremoveicon + ' ' + hideselectedvaluessection + ' ' + hidelabel + ' ' + customlabel + ' ' + usefullcolumn + '>' +
                   '</vr-whs-be-carrieraccount-selector></span>';
        }

        function customerSelectorCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var carrierAccountDirectiveAPI;
            var carrierAccountReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onCarrierAccountDirectiveReady = function (api) {
                    carrierAccountDirectiveAPI = api;
                    carrierAccountReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var loadCarrierAccountPromiseDeferred = UtilsService.createPromiseDeferred();

                    carrierAccountReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(carrierAccountDirectiveAPI, payload, loadCarrierAccountPromiseDeferred);
                    });

                    return loadCarrierAccountPromiseDeferred.promise;
                };

                api.getSelectedIds = function () {
                    return carrierAccountDirectiveAPI.getSelectedIds();
                };

                api.getSelectedValues = function () {
                    return carrierAccountDirectiveAPI.getSelectedValues();
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);