(function (app) {

    'use strict';

    OnNetOperatorDeclarationServicePostpaidSMS.$inject = ['UtilsService', 'VRUIUtilsService'];

    function OnNetOperatorDeclarationServicePostpaidSMS(UtilsService, VRUIUtilsService) {
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
                var ctor = new OnNetOperatorDeclarationServicePostpaidSMSCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/OnNetOperatorDeclaration/MainExtensions/Templates/OnNetOperatorDeclarationServicePostpaidSMSTemplate.html"
        };

        function OnNetOperatorDeclarationServicePostpaidSMSCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var postpaidSMSEntity;

                    if (payload != undefined) {
                        postpaidSMSEntity = payload.settings;
                        $scope.scopeModel.revenue = postpaidSMSEntity.Revenue;
                        $scope.scopeModel.sms = postpaidSMSEntity.SMS;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Retail.RA.Business.OnNetPostpaidSMSOperationDeclarationService,Retail.RA.Business",
                        Revenue: $scope.scopeModel.revenue,
                        SMS: $scope.scopeModel.sms
                    };
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }
        }
    }
    
    app.directive('retailRaOnnetoperatordeclarationservicePostpaidsms', OnNetOperatorDeclarationServicePostpaidSMS);

})(app);