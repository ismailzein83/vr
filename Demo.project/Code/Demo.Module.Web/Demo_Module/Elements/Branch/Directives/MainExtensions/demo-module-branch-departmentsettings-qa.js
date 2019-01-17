"use strict"

app.directive("demoModuleBranchDepartmentsettingsQa", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new QADepartment($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/MainExtensions/Templates/QADepartmentTemplate.html"
        }

        function QADepartment($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineApi();
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Department.QADepartment, Demo.Module.MainExtension"
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
])