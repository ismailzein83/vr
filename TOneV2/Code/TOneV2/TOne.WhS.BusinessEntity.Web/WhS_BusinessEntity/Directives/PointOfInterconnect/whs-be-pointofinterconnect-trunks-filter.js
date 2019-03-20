'use strict';

app.directive('whsBePointofinterconnectTrunksFilter', ['UtilsService', 'VR_GenericData_StringRecordFilterOperatorEnum',
    function (UtilsService, VR_GenericData_StringRecordFilterOperatorEnum) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '=',
                normalColNum: '@',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new pointOfInterconnectStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
         
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PointOfInterconnect/Templates/PointOfInterconnectFilterTemplate.html"
        };

        function pointOfInterconnectStaticEditor(ctrl, $scope, $attrs) {


            this.initializeController = initializeController;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.normalColNum = ctrl.normalColNum ? ctrl.normalColNum : 8;

                defineApi();
            }

            function defineApi() {

                var api = {};

                api.load = function (payload) {
                    var fieldValue = payload.fieldValue;
                    $scope.scopeModel.trunk = (fieldValue != undefined) ? fieldValue.Value : undefined;
                };
                api.getData = function () {
                    var data = {
                        $type: "TOne.WhS.BusinessEntity.Entities.PointOfInterconnectRecordFilter,TOne.WhS.BusinessEntity.Entities",
                        Value: $scope.scopeModel.trunk,
                        CompareOperator: VR_GenericData_StringRecordFilterOperatorEnum.Contains.value
                    };
                    return data;
                };


                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
                
            }

        }
        return directiveDefinitionObject;
    }]);