'use strict';
app.directive('vrGenericdataDatarecordtypefieldsFormulaParentbusinessentity', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new textTypeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/DataRecordFieldFormulas/Templates/ParentBusinessEntityFieldFormulaTemplate.html';
            }

        };

        function textTypeCtor(ctrl, $scope) {
            var context;
            var childName;
            var dependantFieldsSelectorAPI;
            var dependantFieldsSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.fields = [];
                $scope.scopeModel = {};
                $scope.scopeModel.onDependantFieldsSelectorReady = function (api) {
                    dependantFieldsSelectorAPI = api;
                    dependantFieldsSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        $scope.fields.length = 0;
                        context = payload.context;
                        //if (context != undefined && context.getFields != undefined)
                        //    $scope.fields = context.getFields();
                        if (payload.formula != undefined) {
                            //$scope.selectedFieldName = UtilsService.getItemByVal($scope.fields, payload.formula.ChildFieldName, "fieldName");
                            childName = payload.formula.ChildFieldName;
                        }
                        promises.push(loadDependantFieldSelector());
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.DataRecordFieldFormulas.ParentBusinessEntityFieldFormula, Vanrise.GenericData.MainExtensions",
                        ChildFieldName: dependantFieldsSelectorAPI != undefined ? dependantFieldsSelectorAPI.getSelectedIds() : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function loadDependantFieldSelector() {
                var dependantFieldSelectorLoadDeferredReady = UtilsService.createPromiseDeferred();
                dependantFieldsSelectorReadyPromiseDeferred.promise.then(function () {
                    var dependantFieldSelectorPayload = {
                        context: context,
                    };
                    if (childName != undefined) {
                        dependantFieldSelectorPayload.selectedIds = childName;
                    }
                    VRUIUtilsService.callDirectiveLoad(dependantFieldsSelectorAPI, dependantFieldSelectorPayload, dependantFieldSelectorLoadDeferredReady);
                });
                return dependantFieldSelectorLoadDeferredReady.promise;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);