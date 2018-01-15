
'use strict';

app.directive('retailTelesUsertelesinfoView', ['UtilsService', 'VRUIUtilsService', 'Retail_Teles_UserAPIService',
    function (UtilsService, VRUIUtilsService, Retail_Teles_UserAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new UserTelesInfoView($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_Teles/Directives/AccountViews/Templates/UserTelesInfoView.html'
        };

        function UserTelesInfoView($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountId;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountViewDefinition;
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.accountBEDefinitionId;
                        accountId = payload.parentAccountId;
                        accountViewDefinition = payload.accountViewDefinition;
                    }
                    promises.push(loadAccountTelesInfo());

                    function loadAccountTelesInfo() {
                        return Retail_Teles_UserAPIService.GetUserTelesInfo(accountBEDefinitionId, accountId, accountViewDefinition.Settings.VRConnectionId).then(function (response) {
                            if(response != undefined)
                            {
                                $scope.scopeModel.firstName = response.FirstName;
                                $scope.scopeModel.lastName = response.LastName;
                                $scope.scopeModel.loginName = response.LoginName;
                                $scope.scopeModel.siteName = response.SiteName;
                                $scope.scopeModel.enterpriseName = response.EnterpriseName;
                                $scope.scopeModel.routingGroupName = response.RoutingGroupName;
                            }
                        });
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);