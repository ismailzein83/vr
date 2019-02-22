(function (app) {
    'use strict';
    technicalNumberPlan.$inject = ["UtilsService", 'VRUIUtilsService'];

    function technicalNumberPlan(UtilsService, VRUIUtilsService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new TechnicalNumberPlanCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/TechnicalSettings/Templates/RouteSyncTechnicalNumberPlanTemplate.html"
        };

        function TechnicalNumberPlanCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codeList = [];

                $scope.scopeModel.validateCode = function () {
                    if ($scope.scopeModel.codeToAdd == undefined)
                        return;

                    if (!$scope.scopeModel.isCodeValid()) {
                        return "Zone code already exists";
                    }
                };

                $scope.scopeModel.isCodeValid = function () {
                    var codeToAdd = $scope.scopeModel.codeToAdd;

                    if (codeToAdd == undefined || codeToAdd.length == 0 || codeToAdd == '')
                        return false;

                    for (var i = 0; i < $scope.scopeModel.codeList.length; i++) {
                        var code = $scope.scopeModel.codeList[i].Code;
                        if (codeToAdd == code)
                            return false;
                    }

                    return true;
                };

                $scope.scopeModel.addCode = function () {
                    $scope.scopeModel.codeList.push({ Code: $scope.scopeModel.codeToAdd });
                    $scope.scopeModel.codeToAdd = undefined;
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.selectedValues != undefined && payload.selectedValues.Settings != undefined && payload.selectedValues.Settings.Codes != undefined) {
                        $scope.scopeModel.codeList = payload.selectedValues.Codes.Codes;
                    }
                };

                api.setData = function (data) {
                    data.Settings = {
                        $type: "TOne.WhS.BusinessEntity.Entities.TechnicalNumberPlanSettings, TOne.WhS.BusinessEntity.Entities",
                        Codes: $scope.scopeModel.codeList
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('vrWhsTechnicalnumberplan', technicalNumberPlan);
})(app);