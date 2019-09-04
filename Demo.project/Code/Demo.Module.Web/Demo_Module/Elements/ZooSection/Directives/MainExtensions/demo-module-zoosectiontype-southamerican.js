'use strict';

app.directive('demoModuleZoosectiontypeSouthamerican', ['UtilsService',
    function (UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SouthAmericanType($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/SouthAmericanSectionTypeTemplate.html'
        };

        function SouthAmericanType($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};
                
                var zooSectionTypeEntity;

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        zooSectionTypeEntity = payload.zooSectionTypeEntity;
                    }

                    if (zooSectionTypeEntity != undefined) {
                        $scope.scopeModel.nbOfAnimals = zooSectionTypeEntity.NbOfAnimals;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.ZooSection.SouthAmerican, Demo.Module.MainExtension",
                        NbOfAnimals: $scope.scopeModel.nbOfAnimals
                    };
                };

                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);