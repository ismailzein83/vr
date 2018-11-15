"use strict";
app.directive("vrAnalyticReportgenerationcustomcodeFilegenerator", ["UtilsService",
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var flatFile = new FlatFile($scope, ctrl, $attrs);
                flatFile.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/Analytic/Directives/VRRe/AutomatedReport/FileGenerator/FlatFile/Templates/FlatFileGeneratorTemplate.html"
        };


        function FlatFile($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {

                UtilsService.waitMultiplePromises(promises).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    var promises = [];
                    if (payload != undefined) {

                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isLoading = false;

                    });

                };


                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Analytic.MainExtensions.AutomatedReport.FileGenerators.FlatFileGenerator,Vanrise.Analytic.MainExtensions",
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            return directiveDefinitionObject;
        }
]);