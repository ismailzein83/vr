//(function (app) {

//    'use strict';

//    IsEmptyFilterRuntimeSettingsDirective.$inject = ['UtilsService'];

//    function IsEmptyFilterRuntimeSettingsDirective(UtilsService) {
//        return {
//            restrict: "E",
//            scope: {
//                onReady: "=",
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new IsEmptyFilterRuntimeSettingsController($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/IsEmptyFilterRuntimeTemplate.html"
//        };


//        function IsEmptyFilterRuntimeSettingsController($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var definitionSettings;
//            var filterValues;

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.filters = [];
//                defineAPI();
//            }
//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var initialPromises = [];
//                    console.log(payload);

//                    if (payload != undefined) {
//                        definitionSettings = payload.settings;
//                        filterValues = payload.filterValues;

//                        if (definitionSettings != undefined) {
//                            var allField = definitionSettings.AllField;
//                            allField.name = 'All';
//                            $scope.scopeModel.filters.push(allField);

//                            var nullField = definitionSettings.NullField;
//                            nullField.name = 'Null';
//                            $scope.scopeModel.filters.push(nullField);

//                            var notNullField = definitionSettings.NotNullField;
//                            notNullField.name = 'Not Null';
//                            $scope.scopeModel.filters.push(notNullField);
//                        }
//                    }

//                    var rootPromiseNode = {
//                        promises: initialPromises,
//                        getChildNode: function () {
//                            var directivePromises = [];

//                            return {
//                                promises: directivePromises
//                            };
//                        }
//                    };

//                    return UtilsService.waitPromiseNode(rootPromiseNode);
//                };

//                api.getData = function () {
//                    buildFilterGroup();
//                    return {
//                       // FilterGroup: buildFilterGroup()
//                    };
//                };

//                api.hasFilters = function () {
//                    return true;
//                };

//                if (ctrl.onReady != null) {
//                    ctrl.onReady(api);
//                }
//            }

//            function buildFilterGroup() {
//                conosle.log($scope.scopeModel.selectedFilter);
//            }
//        }
//    }

//    app.directive('vrGenericdataGenericbeFilterruntimeIsempty', IsEmptyFilterRuntimeSettingsDirective);

//})(app);