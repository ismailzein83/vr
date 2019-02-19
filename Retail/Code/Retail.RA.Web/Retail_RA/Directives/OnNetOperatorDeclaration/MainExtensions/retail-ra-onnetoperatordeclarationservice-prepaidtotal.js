(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePrepaidTotal.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePrepaidTotal(UtilsService, VRUIUtilsService) {
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
                var ctor = new OnNetOperatorDeclarationServicePrepaidTotalCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePrepaidTotalTemplate.html"
        };

        function OnNetOperatorDeclarationServicePrepaidTotalCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var prepaidTotalEntity;

                    if (payload != undefined) {
                        prepaidTotalEntity = payload.settings;
                        $scope.scopeModel.numberOfSubscribers = prepaidTotalEntity.NumberOfSubscribers;
                        $scope.scopeModel.amountFromTopups = prepaidTotalEntity.AmountFromTopups;
                        $scope.scopeModel.residualAmountFromTopups = prepaidTotalEntity.ResidualAmountFromTopups;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPrepaidTotalOperationDeclarationService,Retail.RA.Business",
                        NumberOfSubscribers: $scope.scopeModel.numberOfSubscribers,
                        AmountFromTopups: $scope.scopeModel.amountFromTopups,
                        ResidualAmountFromTopups: $scope.scopeModel.residualAmountFromTopups
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('retailRaOnnetoperatordeclarationservicePrepaidtotal', OnNetOperatorDeclarationServicePrepaidTotal);

})(app);