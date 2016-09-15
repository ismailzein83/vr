'use strict';
app.directive('whsInvoiceCarrierSupplierSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new carriersCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'carrierSupplierCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };

        function getTemplate(attrs) {
            return '<whs-invoice-carrier-selector isrequired="carrierSupplierCtrl.isrequired" normal-col-num="{{carrierSupplierCtrl.normalColNum}}" on-ready="carrierSupplierCtrl.onDirectiveReady" getsuppliers="true"></whs-invoice-carrier-selector>';
        }

        function carriersCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, undefined, directiveLoadPromiseDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getSelectedIds = function () {
                    return directiveReadyAPI.getSelectedIds();
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);