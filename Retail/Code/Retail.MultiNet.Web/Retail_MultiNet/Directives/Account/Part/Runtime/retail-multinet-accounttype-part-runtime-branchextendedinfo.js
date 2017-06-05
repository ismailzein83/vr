'use strict';

app.directive('retailMultinetAccounttypePartRuntimeBranchextendedinfo', ["UtilsService", "VRUIUtilsService", function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new BranchExtendedInfoRuntime($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_MultiNet/Directives/Account/Part/Runtime/Templates/AccountTypePartBranchExtendedInfoRuntimeTemplate.html'
    };

    function BranchExtendedInfoRuntime($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        $scope.scopeModel = {};
        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.load = function (payload) {
                console.log(payload);
                if (payload != undefined && payload.partSettings != undefined) {
                    $scope.scopeModel.branchCode = payload.partSettings.BranchCode;
                    $scope.scopeModel.contractReferenceNumber = payload.partSettings.ContractReferenceNumber;
                }
            };
            api.getData = function () {
                return {
                    $type: 'Retail.MultiNet.MainExtensions.MultiNetBranchExtendedInfo, Retail.MultiNet.MainExtensions',
                    BranchCode: $scope.scopeModel.branchCode,
                    ContractReferenceNumber: $scope.scopeModel.contractReferenceNumber
                }
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);