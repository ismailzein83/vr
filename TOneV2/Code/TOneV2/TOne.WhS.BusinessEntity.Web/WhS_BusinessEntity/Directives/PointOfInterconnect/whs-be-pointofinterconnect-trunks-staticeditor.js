'use strict';

app.directive('whsBePointofinterconnectTrunksStaticeditor', ['UtilsService', 'VRUIUtilsService', 'VRDateTimeService', 'WhS_BE_SaleZoneAPIService', 'WhS_BE_FaultTicketAPIService',
    function (UtilsService, VRUIUtilsService, VRDateTimeService, WhS_BE_SaleZoneAPIService, WhS_BE_FaultTicketAPIService) {
        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new pointOfInterconnectStaticEditor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
         
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/PointOfInterconnect/Templates/PointOfInterconnectStaticEditorTemplate.html"
        };

        function pointOfInterconnectStaticEditor(ctrl, $scope, $attrs) {


            this.initializeController = initializeController;
            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.trunks = [];
                $scope.scopeModel.disabledAddTrunk = true;

                $scope.scopeModel.addTrunk = function () {
                    $scope.scopeModel.trunks.push({ trunk: $scope.scopeModel.TrunkValue, message: null });
                    $scope.scopeModel.TrunkValue = undefined;
                    $scope.scopeModel.disabledAddTrunk = true;
                };

                $scope.scopeModel.onTrunkValueChange = function (value) {
                    $scope.scopeModel.disabledAddTrunk = (value == undefined && $scope.scopeModel.TrunkValue.length - 1 < 1) || UtilsService.getItemIndexByVal($scope.scopeModel.trunks, $scope.scopeModel.TrunkValue, "trunk") != -1;
                };

                $scope.scopeModel.getImportedTrunks = function (values) {
                    $scope.scopeModel.Trunk.length = 0;
                    for (var i = 0; i < values.length ; i++) {
                        if (UtilsService.getItemIndexByVal($scope.scopeModel.trunks, values[i], "trunk") == -1)
                            $scope.scopeModel.trunks.push({ trunk: values[i], message: null });
                    }
                };

                $scope.scopeModel.validateTrunks = function () {
                    if ($scope.scopeModel.trunks.length == 0)
                        return "Enter at least one code.";
                    if (ValidateTrunk() == false)
                        return "Invalid trunks inputs.";
                    return null;
                };

           
                defineApi();

            }


            function ValidateTrunk() {
                for (var i = 0; i < $scope.scopeModel.trunks.length ; i++) {
                    if ($scope.scopeModel.trunks[i].message != null)
                        return false;
                }
                return true;
            }
        

            function defineApi() {

                var api = {};

                api.load = function (payload) {

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;

                        if (selectedValues != undefined) {

                            for (var i = 0; i < selectedValues.trunks.length ; i++) {
                                $scope.scopeModel.trunks.push({ trunk: selectedValues.trunks[i], message: null });
                            }

                        }
                    }
   
            };

                api.setData = function (obj) {

                    console.log("setdata");

                        obj.trunksDetails = {

                            trunks: $scope.scopeModel.trunks         
                    }
                   
                };

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
                
            }

        }
        return directiveDefinitionObject;
    }]);