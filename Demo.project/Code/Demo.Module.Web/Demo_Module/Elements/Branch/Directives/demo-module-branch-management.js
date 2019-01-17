"use-strict";

app.directive("demoModuleBranchManagement", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_BranchService',
    function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_BranchService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new BranchManagement($scope, ctrl, $attrs);
                ctor.initializaController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Demo_Module/Elements/Branch/Directives/Templates/BranchManagementTemplate.html"
        }

        function BranchManagement($scope, ctrl, $attrs) {
            this.initializaController = initializaController;

            var companyId;

            var gridAPI;

            function initializaController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineApi();
                }

                $scope.scopeModel.addBranch = function () {
                    var onBranchAdded = function (branch) {
                        gridAPI.onBranchAdded(branch);
                    };
                    var companyIdItem = { CompanyId: companyId };
                    Demo_Module_BranchService.addBranch(onBranchAdded, companyIdItem);
                };
            }

            function defineApi() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        companyId = payload.companyId;
                    }
                    return gridAPI.load(getGridPayload());
                };

                api.onBranchAdded = function (branchObject) {
                    gridAPI.onBranchAdded(branchObject);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(api);
            }

            function getGridPayload() {
                return {
                    query: { CompanyIds: [companyId]},
                    companyId: companyId,
                    hideCompanyColumn: true
                }
            }
        }
        return directiveDefinitionObject;
    }
])