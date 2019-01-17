"use strict";

app.directive("demoModuleBranchEmployeesettingsParttime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new PartTimeEmployee($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/PartTimeEmployeeTemplate.html"
        }

        function PartTimeEmployee($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineApi();
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.employeeSettingsEntity != undefined) {
                        $scope.scopeModel.numberOfHourPerMonth = payload.employeeSettingsEntity.NumberOfHourPerMonth;
                        $scope.scopeModel.hourRate = payload.employeeSettingsEntity.HourRate;
                    }

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Employee.PartTimeEmployee, Demo.Module.MainExtension", // namespace , dll
                        NumberOfHourPerMonth: $scope.scopeModel.numberOfHourPerMonth,
                        HourRate: $scope.scopeModel.hourRate
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);