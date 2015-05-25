VisualElementEditorController.$inject = ['$scope', 'BIEntityTypeEnum', 'BIMeasureTypeEnum'];

function VisualElementEditorController($scope, BIEntityTypeEnum, BIMeasureTypeEnum) {

    var biVisualElementAPI;
    defineScope();
    load();

    function defineScope() {
        defineElementTypes();
        defineOperationTypes();
        defineEntityTypes();
        defineMeasureTypes();
        defineNumberOfColumns();

        $scope.save = function () {
            var elementSettings = {
                operationType: $scope.selectedOperationType.value,
                entityType: $scope.selectedEntityType,
                measureTypes: $scope.selectedMeasureTypes
            };

            if ($scope.onAddElement != undefined)
                $scope.onAddElement({
                    type: $scope.selectedElementType.value,
                    settings: elementSettings,
                    numberOfColumns: $scope.selectedNumberOfColumns.value
                });
            $scope.modalContext.closeModal();
        }
    }

    function load() {

    }

    function defineElementTypes() {
        $scope.elementTypes = [
            {
                value: "chart",
                description: "Chart"
            },
            {
                value: "datagrid",
                description: "Report"
            }
        ]; 
        $scope.selectedElementType = $scope.elementTypes[0];
    }

    function defineOperationTypes() {
        $scope.operationTypes = [{
                value: "TopEntities",
                description: "Top X"
            }, {
                value: "MeasuresGroupedByTime",
                description: "Time Variation Data"
            }
        ];
        $scope.selectedOperationType = $scope.operationTypes[0];
    }

    function defineEntityTypes() {
        $scope.entityTypes = [];
        for (var e in BIEntityTypeEnum) {
            $scope.entityTypes.push(BIEntityTypeEnum[e]);
        }

        $scope.selectedEntityType = $scope.entityTypes[0];
    }

    function defineMeasureTypes() {
        $scope.measureTypes = [];
        for (var m in BIMeasureTypeEnum) {
            $scope.measureTypes.push(BIMeasureTypeEnum[m]);
        }

        $scope.selectedMeasureTypes = [];
    }

    function defineNumberOfColumns() {
        $scope.numberOfColumns = [
            {
                value: "6",
                description: "Half Row"
            },
            {
                value: "12",
                description: "Full Row"
            }
        ];

        $scope.selectedNumberOfColumns = $scope.numberOfColumns[0];
    }

}

appControllers.controller('BI_VisualElementEditorController', VisualElementEditorController);