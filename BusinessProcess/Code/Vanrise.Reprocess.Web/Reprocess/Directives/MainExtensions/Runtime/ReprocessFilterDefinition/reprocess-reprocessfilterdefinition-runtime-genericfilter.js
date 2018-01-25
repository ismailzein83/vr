(function (app) {

    'use strict';

    ReprocessfilterdefinitionRuntimeGenericfilter.$inject = ['VRUIUtilsService', 'UtilsService'];

    function ReprocessfilterdefinitionRuntimeGenericfilter(VRUIUtilsService, UtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var reprocessFilterFieldDefinition = new ReprocessFilterFieldDefinition($scope, ctrl);
                reprocessFilterFieldDefinition.initializeController();
            },
            controllerAs: 'filterCtrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Reprocess/Directives/MainExtensions/Runtime/ReprocessFilterDefinition/Templates/ReprocessFilterDefinitionGenericFilter.html'
        };

        function ReprocessFilterFieldDefinition($scope, ctrl) {
            this.initializeController = initializeController;

            $scope.scopeModel = { fields: [], isLoadingDirective: false };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var filterDefinition = payload.filterDefinition;
                    var filter = payload.filter;

                    var fields = filterDefinition.Fields;
                    $scope.scopeModel.isLoadingDirective = true;

                    for (var x = 0; x < fields.length; x++) {
                        var currentField = fields[x];

                        var selectedIds;
                        if (filter != undefined && filter.Fields!=undefined) {
                            selectedIds = filter.Fields[currentField.FieldName];
                        }
                        var currentItem = {
                            fieldName: currentField.FieldName,
                            runtimeEditor: currentField.FieldType.RuntimeEditor,
                        };

                        var filterItem = {
                            loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                            readyPromiseDeferred: UtilsService.createPromiseDeferred()
                        };
                        addMappingFieldAPI(filterItem, currentItem, currentField, selectedIds);
                        promises.push(filterItem.loadPromiseDeferred.promise);
                        $scope.scopeModel.fields.push(currentItem);
                    }
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        $scope.scopeModel.isLoadingDirective = false;
                    });
                };

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.Reprocess.Entities.GenericReprocessFilter, Vanrise.Reprocess.Entities",
                        Fields: {}
                    };

                    var oneItemIsAddedAtLeast = false;
                    for (var x = 0; x < $scope.scopeModel.fields.length; x++) {
                        var currentField = $scope.scopeModel.fields[x];
                        var fieldData = currentField.directiveAPI != undefined ? currentField.directiveAPI.getData() : undefined;
                        if (fieldData != undefined){
                            obj.Fields[currentField.fieldName] = fieldData;
                            oneItemIsAddedAtLeast = true;
                        }
                    }
                    return oneItemIsAddedAtLeast ? obj : undefined;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            };

            function addMappingFieldAPI(filterItem, currentItem, field, selectedIds) {

                var directivePayload = { fieldType: field.FieldType, fieldTitle: field.FieldTitle, fieldValue: selectedIds };

                currentItem.onDirectiveReady = function (api) {
                    currentItem.directiveAPI = api;
                    filterItem.readyPromiseDeferred.resolve();
                };

                filterItem.readyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(currentItem.directiveAPI, directivePayload, filterItem.loadPromiseDeferred);
                });
            };
        }
    }

    app.directive('reprocessReprocessfilterdefinitionRuntimeGenericfilter', ReprocessfilterdefinitionRuntimeGenericfilter);

})(app);