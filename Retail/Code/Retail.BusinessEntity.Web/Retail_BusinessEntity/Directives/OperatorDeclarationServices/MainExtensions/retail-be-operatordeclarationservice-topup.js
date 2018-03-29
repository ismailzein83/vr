(function (app) {

    'use strict';

    OperatorDeclarationServiceTopup.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OperatorDeclarationServiceTopup(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                label: '@',
                customvalidate: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new OperatorDeclarationServiceTopupCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/OperatorDeclarationServices/MainExtensions/Templates/OperatorDeclarationServiceTopupTemplate.html"
        };

        function OperatorDeclarationServiceTopupCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                  
                    var topupEntity;

                    if (payload != undefined) {

                        topupEntity = payload.topupEntity;

                        $scope.scopeModel.numberOfTopups = topupEntity.NumberOfTopups;
                    }

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.MainExtensions.OperatorDeclarationServices.Topup,Retail.BusinessEntity.MainExtensions",

                        NumberOfTopups: $scope.scopeModel.numberOfTopups,
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailBeOperatordeclarationserviceTopup', OperatorDeclarationServiceTopup);

})(app);