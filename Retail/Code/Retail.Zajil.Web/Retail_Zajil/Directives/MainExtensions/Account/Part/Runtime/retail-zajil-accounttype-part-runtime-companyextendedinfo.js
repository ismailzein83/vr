'use strict';

app.directive('retailZajilAccounttypePartRuntimeCompanyextendedinfo', ["UtilsService", "VRUIUtilsService",  function (UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runtimeEditor = new AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs);
            runtimeEditor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/Retail_Zajil/Directives/MainExtensions/Account/Part/Runtime/Templates/AccountTypePartCompanyExtendedInfoRuntimeTemplate.html'
    };

    function AccountTypeExtendedInfoRuntime($scope, ctrl, $attrs) {
       
        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }
        function defineAPI() {
            $scope.scopeModel = {};
            var api = {};
            api.load = function (payload) {
                console.log(payload)

                if (payload != undefined && payload.partSettings != undefined) {
                    $scope.scopeModel.cRMCompanyId =  payload.partSettings.CRMCompanyId ;
                    $scope.scopeModel.cRMCompanyAccountNo = payload.partSettings.CRMCompanyAccountNo;
                    $scope.scopeModel.salesAgent = payload.partSettings.SalesAgent;
                    $scope.scopeModel.serviceType =  payload.partSettings.ServiceType ;
                    $scope.scopeModel.remarks =  payload.partSettings.Remarks ;
                    $scope.scopeModel.gPVoiceCustomerNo =  payload.partSettings.GPVoiceCustomerNo ;
                    $scope.scopeModel.serviceId = payload.partSettings.ServiceId;
                    $scope.scopeModel.customerPO = payload.partSettings.CustomerPO;

                }

            };
            api.getData = function () {
                return {
                    $type: 'Retail.Zajil.MainExtensions.ZajilCompanyExtendedInfo, Retail.Zajil.MainExtensions',
                    CRMCompanyId: $scope.scopeModel.cRMCompanyId,
                    CRMCompanyAccountNo: $scope.scopeModel.cRMCompanyAccountNo,
                    SalesAgent: $scope.scopeModel.salesAgent,
                    ServiceType: $scope.scopeModel.serviceType,
                    Remarks: $scope.scopeModel.remarks,
                    GPVoiceCustomerNo: $scope.scopeModel.gPVoiceCustomerNo,
                    ServiceId: $scope.scopeModel.serviceId,
                    CustomerPO: $scope.scopeModel.customerPO
                };
            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
       
    }
}]);