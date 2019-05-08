'use strict';

app.directive('vrGenericdataRdbrecordstoragesettingsExpressionfieldCustomcode', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomCodeExpressionFieldCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBExpressionFields/Templates/CustomCodeExpressionFieldTemplate.html"
        };

        function CustomCodeExpressionFieldCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var context;
            var expressionFieldSettings;

            var dependentJoinsAPI;
            var dependentJoinsReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.selectedvalues = [];

                $scope.scopeModel.onDependentJoinsSelectorReady = function (api) {
                    dependentJoinsAPI = api;
                    dependentJoinsReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([dependentJoinsReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    dependentJoinsAPI.clearDataSource();
                    var initialPromises = [];

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            if (payload != undefined) {
                                context = payload.context;
                                var dependentJoinsList = context.getJoinsList();
                                expressionFieldSettings = payload.expressionFieldSettings;

                                if (dependentJoinsList != undefined) {
                                    ctrl.datasource = dependentJoinsList;
                                }

                                if (expressionFieldSettings != undefined) {
                                    $scope.scopeModel.customCode = expressionFieldSettings.CustomCode;
                                    VRUIUtilsService.setSelectedValues(expressionFieldSettings.DependentJoins, 'RDBRecordStorageJoinName', attrs, ctrl);
                                }
                            }

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.ExpressionFields.RDBDataRecordStorageCustomCodeExpressionField, Vanrise.GenericData.RDBDataStorage",
                        CustomCode: $scope.scopeModel.customCode,
                        DependentJoins: getSelectedDependentJoinslist(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getSelectedDependentJoinslist() {
                var length = ctrl.selectedvalues.length;
                var dependenJoins = [];
                if (length > 0) {
                    for (var i = 0; i < length; i++) {
                        var dependentJoin = ctrl.selectedvalues[i];
                        dependenJoins.push(dependentJoin.RDBRecordStorageJoinName);
                    }
                }
                return dependenJoins;
            }
        }

        return directiveDefinitionObject;
    }]);