(function (app) {

    'use strict';

    RestAPIRecordQueryInterceptorDirective.$inject = ['UtilsService'];

    function RestAPIRecordQueryInterceptorDirective(UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RestAPIRecordQueryInterceptorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/PartnerPortal_CustomerAccess/Elements/VRRestAPIRecordQueryInterceptor/Directives/MainExtensions/Templates/RetailAccountVRRestAPIRecordQueryInterceptorTemplate.html'
        };

        function RestAPIRecordQueryInterceptorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                };

                api.getData = function () {

                    return {
                        $type: 'PartnerPortal.CustomerAccess.MainExtensions.VRRestAPIRecordQueryInterceptor.RetailAccountVRRestAPIRecordQueryInterceptor, PartnerPortal.CustomerAccess.MainExtensions'
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }

    app.directive('partnerportalCustomeraccessRetailaccountqueryinterceptor', RestAPIRecordQueryInterceptorDirective);

})(app);