'use strict';
app.directive('vrWhsBeCustomerSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@',
                isrequired: '=',
                normalColNum: '@',
                hideremoveicon: "@",
                onselectionchanged: "=",
                selectedvalues: "="
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var obj = new customerSelector(ctrl, $scope);
                obj.initializeController();

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

            //var required = "";
            //if (attrs.isrequired != undefined)
            //    required = "isrequired";

            var hideremoveicon = "";
            if (attrs.hideremoveicon != undefined)
                hideremoveicon = "hideremoveicon";


            return '<vr-whs-be-carrieraccount-selector onselectionchanged="ctrl.onselectionchanged"  normal-col-num="{{ctrl.normalColNum}}"   getcustomers on-ready="onCarrierAccountDirectiveReady" ' +
                multipleselection + ' isrequired="ctrl.isrequired" selectedvalues="ctrl.selectedvalues" ' + hideremoveicon + '></vr-whs-be-carrieraccount-selector>';
        }

        function customerSelector(ctrl, $scope) {
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

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);