'use strict';
app.directive('vrGenericdataFieldtypeChoices', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.values = [];
                var ctor = new choicesTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/FieldType/Choices/Templates/ChoicesFieldTypeTemplate.html';
            }

        };

        function choicesTypeCtor(ctrl, $scope) {

            var choicesGridAPI;
            var choicesGridReadyDeferred;

            var dataRecordFieldChoiceSelectorGridAPI;
            var dataRecordFieldChoiceSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onChoicesGridReady = function (api) {
                    choicesGridAPI = api;
                    var setLoader = function (value) {
                        $scope.scopeModel.isLoadingDirective = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, choicesGridAPI, undefined, setLoader, choicesGridReadyDeferred);
                };
                $scope.scopeModel.onDataRecordFieldChoiceSelectorReady = function (api) {
                    dataRecordFieldChoiceSelectorGridAPI = api;
                    dataRecordFieldChoiceSelectorReadyDeferred.resolve();
                };
                defineAPI();
            }
        
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        ctrl.isNullable = payload.IsNullable;
                    }

                    if (payload != undefined  && payload.ChoiceDefinitionId == undefined) {
                        choicesGridReadyDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadChoicesGrid());

                    }
                    function loadChoicesGrid() {
                        var loadChoicesGridPromiseDeferred = UtilsService.createPromiseDeferred();
                        choicesGridReadyDeferred.promise.then(function () {
                            choicesGridReadyDeferred = undefined;
                            var choicesGridPayLoad;
                            if (payload != undefined && payload.Choices != undefined) {
                                choicesGridPayLoad = {
                                    choices: payload.Choices
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(choicesGridAPI, choicesGridPayLoad, loadChoicesGridPromiseDeferred);
                        });
                        return loadChoicesGridPromiseDeferred.promise;
                    }
                    promises.push(loadDataRecordFieldChoiceSelector());
                    function loadDataRecordFieldChoiceSelector() {
                        var loadDataRecordFieldChoiceSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
                        dataRecordFieldChoiceSelectorReadyDeferred.promise.then(function () {
                            var dataRecordFieldChoicePayLoad;
                            if (payload != undefined && payload.ChoiceDefinitionId != undefined) {
                                dataRecordFieldChoicePayLoad = {
                                    selectedIds: payload.ChoiceDefinitionId
                                }
                            }
                            VRUIUtilsService.callDirectiveLoad(dataRecordFieldChoiceSelectorGridAPI, dataRecordFieldChoicePayLoad, loadDataRecordFieldChoiceSelectorPromiseDeferred);
                        });
                        return loadDataRecordFieldChoiceSelectorPromiseDeferred.promise;
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data  = {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType, Vanrise.GenericData.MainExtensions",
                        Choices: choicesGridAPI.getData(),
                        IsNullable: ctrl.isNullable,
                        ChoiceDefinitionId: dataRecordFieldChoiceSelectorGridAPI.getSelectedIds()
                    };
                    return data;

                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);