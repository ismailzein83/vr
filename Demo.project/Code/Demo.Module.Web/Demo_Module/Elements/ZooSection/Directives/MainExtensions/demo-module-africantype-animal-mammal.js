'use strict';

app.directive('demoModuleAfricantypeAnimalMammal', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new MammalAnimal($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/MammalAnimalTemplate.html'
        };

        function MammalAnimal($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                var zooSectionTypeAnimalEntity;

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        zooSectionTypeAnimalEntity = payload.zooSectionTypeAnimalEntity;
                    }

                    if (zooSectionTypeAnimalEntity != undefined) {
                        $scope.scopeModel.highestJump = zooSectionTypeAnimalEntity.HighestJump;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Animal.Mammal, Demo.Module.MainExtension",
                        HighestJump: $scope.scopeModel.highestJump
                    };
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);