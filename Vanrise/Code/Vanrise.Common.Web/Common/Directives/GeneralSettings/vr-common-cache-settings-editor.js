'use strict';

app.directive('vrCommonCacheSettingsEditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/Common/Directives/GeneralSettings/Templates/CacheTemplateSettings.html"
        };

        function settingEditorCtor(ctrl, $scope, $attrs) {
          
            $scope.scopeModel = {};
            $scope.scopeModel.onUpdateSwitchChanged = function () {
                if( $scope.scopeModel.updateCache == true){
                     $scope.scopeModel.updateSwitchDisabled = true;
                     $scope.scopeModel.clientCacheNumber ++;
                }
               
            };
         
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.clientCacheNumber = 0;
                    if (payload != undefined && payload.data != undefined) {
                        $scope.scopeModel.clientCacheNumber = payload.data.ClientCacheNumber;
                    }

                };
               
                api.getData = function () {
                    return {
                        ClientCacheNumber: $scope.scopeModel.clientCacheNumber
                    }
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }
        return directiveDefinitionObject;
    }]);