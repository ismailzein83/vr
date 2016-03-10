(function (app) {

    'use strict';

    GroupSupplierDirective.$inject = [];

    function GroupSupplierDirective() {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var group = new GroupSupplier(ctrl, $scope, $attrs);
                group.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true
        };

        function GroupSupplier(ctrl, $scope, attrs) {


            function initializeController() {
                getDirectiveAPI();
            }

            function getDirectiveAPI() {
                var api = {};
                api.getData = function () {
                    return {
                        $type: "CP.SupplierPricelist.Business.SupplierGroup, CP.SupplierPricelist.Business"
                    };
                };

                api.load = function (payload) {};
                if (ctrl.onReady && typeof ctrl.onReady == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }



        return directiveDefinitionObject;
    }

    app.directive('vrCpGroupSupplier', GroupSupplierDirective)

})(app);
