'use strict';

app.directive('demoModuleZoosectionTypeSouthamerican', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                label: '@',
                customvalidate: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ZooSectionTypeSouthAmerican($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/SouthAmericanSectionTypeTemplate.html'
        };

        function ZooSectionTypeSouthAmerican($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                var zooSectionTypeEntity;

                api.load = function (payload) {
                    var zooSectionTypeSouthAmericanLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    if (payload != undefined) {
                        zooSectionTypeEntity = payload.zooSectionTypeEntity;
                    }

                    if (zooSectionTypeEntity != undefined) {
                        $scope.scopeModel.nbOfAnimals = zooSectionTypeEntity.NbOfAnimals;
                    }

                    zooSectionTypeSouthAmericanLoadPromiseDeferred.resolve();

                    return zooSectionTypeSouthAmericanLoadPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.ZooSection.SouthAmerican, Demo.Module.MainExtension",
                        NbOfAnimals: $scope.scopeModel.nbOfAnimals
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);