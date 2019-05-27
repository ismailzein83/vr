(function (app) {

    'use strict';

    IsEmptyFilterRuntimeSettingsDirective.$inject = ['UtilsService', 'VR_Invoice_LogicalOperatorEnum'];

    function IsEmptyFilterRuntimeSettingsDirective(UtilsService, VR_Invoice_LogicalOperatorEnum) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new IsEmptyFilterRuntimeSettingsController($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Runtime/GenericBEFilterDefinition/Templates/IsEmptyFilterRuntimeTemplate.html"
        };


        function IsEmptyFilterRuntimeSettingsController($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var definitionSettings;
            var fieldName;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.filters = [];
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var initialPromises = [];

                    if (payload != undefined) {
                        definitionSettings = payload.settings;

                        if (definitionSettings != undefined) {
                            fieldName = definitionSettings.FieldName;
                            $scope.scopeModel.isRequired = definitionSettings.IsRequired;
                            $scope.scopeModel.filters.push(definitionSettings.AllField);
                            $scope.scopeModel.filters.push(definitionSettings.NullField);
                            $scope.scopeModel.filters.push(definitionSettings.NotNullField);
                        }
                    }

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
                    return {
                        RecordFilter: buildFilterGroup()
                    };
                };

                api.hasFilters = function () {
                    return true;
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
            }

            function buildFilterGroup() {
                var obj = {
                    $type: "Vanrise.GenericData.Entities.RecordFilterGroup, Vanrise.GenericData.Entities",
                    LogicalOperator: VR_Invoice_LogicalOperatorEnum.AND.value,
                    Filters: []
                };

                var notEmptyFilter = {
                    $type: "Vanrise.GenericData.Entities.NonEmptyRecordFilter, Vanrise.GenericData.Entities",
                    FieldName: fieldName
                };
                var emptyFilter = {
                    $type: "Vanrise.GenericData.Entities.EmptyRecordFilter, Vanrise.GenericData.Entities",
                    FieldName: fieldName
                };

                if ($scope.scopeModel.selectedFilter == undefined) {
                    if (definitionSettings.NotNullField.IsDefault) {
                        $scope.scopeModel.selectedFilter = definitionSettings.NotNullField;
                        obj.Filters.push(notEmptyFilter);
                    }
                    else if (definitionSettings.NullField.IsDefault) {
                        $scope.scopeModel.selectedFilter = definitionSettings.NullField;
                        obj.Filters.push(emptyFilter);
                    }
                    else if (definitionSettings.AllField.IsDefault) {
                        $scope.scopeModel.selectedFilter = definitionSettings.AllField;
                        obj.Filters.push(notEmptyFilter);
                        obj.Filters.push(emptyFilter);
                        obj.LogicalOperator = VR_Invoice_LogicalOperatorEnum.OR.value;
                    }
                }
                else {
                    if ($scope.scopeModel.selectedFilter.Title == definitionSettings.NotNullField.Title) {
                        obj.Filters.push(notEmptyFilter);
                    }
                    else if ($scope.scopeModel.selectedFilter.Title == definitionSettings.NullField.Title) {
                        obj.Filters.push(emptyFilter);
                    }
                    else {
                        obj.Filters.push(notEmptyFilter);
                        obj.Filters.push(emptyFilter);
                        obj.LogicalOperator = VR_Invoice_LogicalOperatorEnum.OR.value;
                    }
                }
                return obj;
            }
        }
    }

    app.directive('vrGenericdataGenericbeFilterruntimeIsempty', IsEmptyFilterRuntimeSettingsDirective);

})(app);