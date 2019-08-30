'use strict';

app.directive('demoModuleZoosectionTypeAnimalMammal', ['UtilsService',
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
                var ctor = new ZooSectionTypeAfrican($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/MammalAnimalTemplate.html'
        };

        function ZooSectionTypeAfrican($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                var zooSectionTypeAnimalEntity;

                api.load = function (payload) {
                    var zooSectionTypeAnimalMammalLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                    if (payload != undefined) {
                        zooSectionTypeAnimalEntity = payload.zooSectionTypeAnimalEntity;
                    }

                    if (zooSectionTypeAnimalEntity != undefined) {
                        $scope.scopeModel.highestJump = zooSectionTypeAnimalEntity.HighestJump;
                    }

                    zooSectionTypeAnimalMammalLoadPromiseDeferred.resolve();

                    return zooSectionTypeAnimalMammalLoadPromiseDeferred.promise;
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Animal.Mammal, Demo.Module.MainExtension",
                        HighestJump: $scope.scopeModel.highestJump
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);