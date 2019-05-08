'use strict';

app.directive('vrGenericdataRdbrecordstoragesettingsExpressionfieldFromjoin', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new FromJoinExpressionFieldCtol(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/MainExtensions/DataStorage/DataRecordStorage/RDBExpressionFields/Templates/FromJoinExpressionFieldTemplate.html"
        };

        function FromJoinExpressionFieldCtol(ctrl, $scope, attrs) {
            this.initializeController = initializeController;

            var context;
            var expressionFieldSettings;

            var joinFieldNameDirectiveAPI;
            var joinFieldNameDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                ctrl.datasource = [];
                ctrl.selectedvalues = [];

                $scope.scopeModel.onJoinFieldNameSelectorReady = function (api) {
                    joinFieldNameDirectiveAPI = api;
                    joinFieldNameDirectiveReadyDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([joinFieldNameDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        context = payload.context;
                        var dependentJoinsList = context.getJoinsList();
                        expressionFieldSettings = payload.expressionFieldSettings;
                    }
           

                    initialPromises.push(loadJoinFieldNameDirective());

                    var rootPromiseNode = {
                        promises: initialPromises,
                        getChildNode: function () {
                            var directivePromises = [];

                            return {
                                promises: directivePromises
                            };
                        }
                    };

                    return UtilsService.waitPromiseNode(rootPromiseNode);
                };

                api.getData = function () {
                    var data = joinFieldNameDirectiveAPI.getData();
                    return {
                        $type: "Vanrise.GenericData.RDBDataStorage.MainExtensions.ExpressionFields.RDBDataRecordStorageExpressionFieldFromJoin, Vanrise.GenericData.RDBDataStorage",
                        JoinName: data.JoinName,
                        FieldName: data.FieldName
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadJoinFieldNameDirective() {
                var joinFieldNameDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                joinFieldNameDirectiveReadyDeferred.promise.then(function () {
                    var fieldNameDirectivePayload = {
                        context: context,
                        joinName: expressionFieldSettings != undefined ? expressionFieldSettings.JoinName : undefined,
                        fieldName: expressionFieldSettings != undefined ? expressionFieldSettings.FieldName : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(joinFieldNameDirectiveAPI, fieldNameDirectivePayload, joinFieldNameDirectiveLoadDeferred);
                });
                return joinFieldNameDirectiveLoadDeferred.promise;
            }
        }

        return directiveDefinitionObject;
    }]);