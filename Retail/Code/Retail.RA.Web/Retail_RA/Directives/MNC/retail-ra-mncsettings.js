(function (app) {
    'use strict';
    mncSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'Retail_RA_OperatorDirectionServicesAPIService'];

    function mncSettings(UtilsService, VRUIUtilsService, Retail_RA_OperatorDirectionServicesAPIService) {
        return {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new MNCSettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_RA/Directives/MNC/Templates/MNCSettingsTemplate.html"
        };

        function MNCSettingsCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.codeList = [];

                $scope.scopeModel.validateCode = function () {
                    if ($scope.scopeModel.codeToAdd == undefined)
                        return;

                    if (!$scope.scopeModel.isCodeValid()) {
                        return "Network code already exists";
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
                        $scope.scopeModel.codeList = payload.selectedValues.Settings.Codes;
                    }
                };

                api.setData = function (data) {
                    data.Settings = {
                        $type: "Retail.RA.Entities.MNCSettings,Retail.RA.Entities",
                        Codes: $scope.scopeModel.codeList
                    };
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }
    app.directive('retailRaMncsettings', mncSettings);
})(app);