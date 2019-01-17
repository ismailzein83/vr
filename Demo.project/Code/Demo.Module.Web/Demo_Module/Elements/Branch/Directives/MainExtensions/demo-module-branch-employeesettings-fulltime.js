"use strict";

app.directive("demoModuleBranchEmployeesettingsFulltime", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FullTimeEmployee($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/FullTimeEmployeeTemplate.html"
        }

        function FullTimeEmployee($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineApi();
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    console.log(payload);
                    if (payload != undefined && payload.employeeSettingsEntity != undefined)
                        $scope.scopeModel.salary = payload.employeeSettingsEntity.Salary;

                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Employee.FullTimeEmployee, Demo.Module.MainExtension", // namespace , dll
                        Salary: $scope.scopeModel.salary
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);