(function (app) {

    'use strict';

    GroupCustomerDirective.$inject = [];

    function GroupCustomerDirective() {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var group = new GroupCustomer(ctrl, $scope, $attrs);
                group.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true
        };

        function GroupCustomer(ctrl, $scope, attrs) {
           

            function initializeController() {
                

                getDirectiveAPI();
            }

            function getDirectiveAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        $type: "CP.SupplierPricelist.Business.CustomerGroup, CP.SupplierPricelist.Business"
                    };
                };

                api.load = function () {
                    
                };
                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }

       

        return directiveDefinitionObject;
    }

    app.directive('vrSecGroupCustomer', GroupCustomerDirective)

})(app);
