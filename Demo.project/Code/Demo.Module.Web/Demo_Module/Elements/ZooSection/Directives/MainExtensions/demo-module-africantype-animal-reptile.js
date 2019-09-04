'use strict';

app.directive('demoModuleAfricantypeAnimalReptile', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ReptileAnimal($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/ZooSection/Directives/MainExtensions/Templates/ReptileAnimalTemplate.html'
        };

        function ReptileAnimal($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var nourrisementSelectorAPI;
            var nourrisementSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onNourrisementSelectorReady = function (api) {
                    nourrisementSelectorAPI = api;
                    nourrisementSelectorReadyDeferred.resolve();
                };

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

                    var loadNourrisementSelectorPromise = loadNourrisementSelector();
                    promises.push(loadNourrisementSelectorPromise);

                    function loadNourrisementSelector() {
                        var nourrisementLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        nourrisementSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {};

                            if (zooSectionTypeAnimalEntity != undefined) {
                                selectorPayload.selectedIds = zooSectionTypeAnimalEntity.Nourrisement;
                            }

                            VRUIUtilsService.callDirectiveLoad(nourrisementSelectorAPI, selectorPayload, nourrisementLoadPromiseDeferred);
                        });

                        return nourrisementLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Demo.Module.MainExtension.Animal.Reptile, Demo.Module.MainExtension",
                        Nourrisement: nourrisementSelectorAPI.getSelectedIds()
                    };
                };
               
                if (ctrl.onReady != undefined && typeof ctrl.onReady == 'function')
                    ctrl.onReady(api);
            }
        }
    }]);