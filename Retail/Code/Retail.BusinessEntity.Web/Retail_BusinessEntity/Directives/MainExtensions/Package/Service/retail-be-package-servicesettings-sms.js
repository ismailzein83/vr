(function (app) {

    'use strict';

    SMSServiceDirective.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_BE_ConnectionTypeEnum'];

    function SMSServiceDirective(UtilsService, VRUIUtilsService, Retail_BE_ConnectionTypeEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var smsService = new SMSService($scope, ctrl, $attrs);
                smsService.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/MainExtensions/Package/Service/Templates/SMSServiceTemplate.html"

        };
        function SMSService($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var mainPayload;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        mainPayload = payload;
                        if (payload.serviceSettings != undefined) {
                            $scope.scopeModel.mMSSupport = payload.serviceSettings.MMSSupport;
                            $scope.scopeModel.unicode = payload.serviceSettings.Unicode;
                            $scope.scopeModel.nbofCharPerMessage = payload.serviceSettings.NbofCharPerMessage;
                        }

                    }

                };

                api.getData = getData;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }

                function getData() {
                    var data = {
                        $type: "Retail.BusinessEntity.MainExtensions.Package.SMSService,Retail.BusinessEntity.MainExtensions",
                        MMSSupport: $scope.scopeModel.mMSSupport,
                        Unicode: $scope.scopeModel.unicode,
                        NbofCharPerMessage: $scope.scopeModel.nbofCharPerMessage
                    };
                    return data;
                }
            }
        }
    }

    app.directive('retailBePackageServiceSms', SMSServiceDirective);

})(app);