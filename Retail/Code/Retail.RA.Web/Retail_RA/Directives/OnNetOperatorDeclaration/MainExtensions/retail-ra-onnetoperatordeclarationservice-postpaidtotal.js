(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePostpaidtotal.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePostpaidtotal(UtilsService, VRUIUtilsService) {
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
                var ctor = new OnNetOperatorDeclarationServicePostpaidtotalCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePostpaidtotalTemplate.html"
        };

        function OnNetOperatorDeclarationServicePostpaidtotalCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var postpaidTotalEntity;

                    if (payload != undefined) {
                        postpaidTotalEntity = payload.settings;
                        $scope.scopeModel.numberOfSubscribers = postpaidTotalEntity.NumberOfSubscribers;
                        $scope.scopeModel.monthlyCharges = postpaidTotalEntity.MonthlyCharges;
                        $scope.scopeModel.revenue = postpaidTotalEntity.Revenue;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPostpaidTotalOperationDeclarationService,Retail.RA.Business",
                        NumberOfSubscribers: $scope.scopeModel.numberOfSubscribers,
                        MonthlyCharges: $scope.scopeModel.monthlyCharges,
                        Revenue: $scope.scopeModel.revenue
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('retailRaOnnetoperatordeclarationservicePostpaidtotal', OnNetOperatorDeclarationServicePostpaidtotal);

})(app);