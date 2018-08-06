
"use strict";

app.directive("vrAnalyticAdvancedexcelFilegeneratorMappedtableTitlesgrid", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
function (UtilsService, VRNotificationService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var titlesGrid = new TitlesGrid($scope, ctrl);
            titlesGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
        },
        templateUrl: "/Client/Modules/Analytic/Directives/MainExtensions/AutomatedReport/FileGenerator/Templates/AdvancedExcelFileGeneratorMappedTableTitlesGridTemplate.html"
    };

    function TitlesGrid($scope, ctrl) {
        this.initializeController = initializeController;

        var context;
        var id=0;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.tableTitles = [];

            $scope.scopeModel.addTableTitle = function () {
                var title = {
                    Id: id
                };
                id++;
                $scope.scopeModel.tableTitles.push(title);
            };

            $scope.scopeModel.deleteTableTitle = function (title) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.tableTitles, title.Id, 'Id');
                $scope.scopeModel.tableTitles.splice(index, 1);
            };

            defineAPI();

        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                var titles;
                if (payload != undefined && payload.titles != undefined) {
                    for (var i = 0; i < payload.titles.length; i++) {
                        $scope.scopeModel.tableTitles.push({
                            Id: i,
                            Title: payload.titles[i] });
                    }
                }
            };

            api.getData = function () {
                var titles = [];
                if ($scope.scopeModel.tableTitles != undefined) {
                    for (var i = 0; i < $scope.scopeModel.tableTitles.length; i++) {
                        var titleEntity = $scope.scopeModel.tableTitles[i];
                        if (titleEntity != undefined && titleEntity.Title!=undefined) {
                            titles.push(titleEntity.Title);
                        }
                    }
                }
                return titles;
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                ctrl.onReady(api);
            }
        }

        function getContext() {
            var currentContext = context;
            if (currentContext == undefined) {
                currentContext = {};
            }
            return currentContext;
        }
    }
}]);