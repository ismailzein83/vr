'use strict';

app.directive('retailTelesAccountextrafieldAccountmappedtoteles', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountMappedToTelesExtraField($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/AccountMappedToTelesExtraFieldTemplate.html'
        };

        function AccountMappedToTelesExtraField($scope, ctrl, $attrs) {
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
                            $scope.scopeModel.fieldTitle = accountExtraFieldDefinitionSettings.FieldTitle;
                            $scope.scopeModel.fieldName = accountExtraFieldDefinitionSettings.FieldName;
                        }
                    }
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.Teles.Business.AccountMappedToTelesFieldDefinition, Retail.Teles.Business',
                        FieldTitle: $scope.scopeModel.fieldTitle,
                        FieldName: $scope.scopeModel.fieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);