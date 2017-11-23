'use strict';

app.directive('vrCommonGaSettingsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new settingEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Common/Directives/GATechnicalSettings/Templates/GATemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
          
            $scope.scopeModel = {};
          
         
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promiseDeferred = UtilsService.createPromiseDeferred();
                    $scope.scopeModel.clientCacheNumber = 0;
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.isEnabled = payload.data.IsEnabled;
                        $scope.scopeModel.account = payload.data.Account;
                    }
                    promiseDeferred.resolve();
                    return promiseDeferred.promise;
                };
               
                api.getData = function () {
                    return {
                        $type: "Vanrise.Entities.GoogleAnalyticsData, Vanrise.Entities",
                        Account: $scope.scopeModel.account,
                        IsEnabled: $scope.scopeModel.isEnabled
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);