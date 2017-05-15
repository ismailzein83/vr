'use strict';

app.directive('retailTelesAccountextrafieldInternationalcallsblocked', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new InternationalCallsBlockedExtraField($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/InternationalCallsBlockedExtraFieldTemplate.html'
        };

        function InternationalCallsBlockedExtraField($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

          

            function initializeController() {
                $scope.scopeModel = {};

               

                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                api.load = function (payload) {
                    var promises = [];
                
                    var accountExtraFieldDefinitionSettings;
                    if (payload != undefined) {
                        accountExtraFieldDefinitionSettings = payload.accountExtraFieldDefinitionSettings;
                        if(accountExtraFieldDefinitionSettings != undefined)
                        {
                            $scope.scopeModel.actionType = accountExtraFieldDefinitionSettings.ActionType;
                            $scope.scopeModel.fieldTitle = accountExtraFieldDefinitionSettings.FieldTitle;
                            $scope.scopeModel.fieldName = accountExtraFieldDefinitionSettings.FieldName;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.InternationalCallsBlockedFieldDefinition, Retail.Teles.Business',
                        ActionType: $scope.scopeModel.actionType,
                        FieldTitle: $scope.scopeModel.fieldTitle,
                        FieldName: $scope.scopeModel.fieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);