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
                    $scope.scopeModel.trunks.push({ trunk: $scope.scopeModel.trunkValue, message: null });
                    $scope.scopeModel.trunkValue = undefined;
                    $scope.scopeModel.disabledAddTrunk = true;
                };

                $scope.scopeModel.onTrunkValueChange = function (value) {
                    $scope.scopeModel.disabledAddTrunk = (value == undefined && $scope.scopeModel.trunkValue.length - 1 < 1) || UtilsService.getItemIndexByVal($scope.scopeModel.trunks, $scope.scopeModel.trunkValue, "trunk") != -1;
                };

                $scope.scopeModel.getImportedTrunks = function (values) {
                    if (values != undefined)
                    {
                        for (var i = 0; i < values.length ; i++) {
                            if (UtilsService.getItemIndexByVal($scope.scopeModel.trunks, values[i], "trunk") == -1)
                                $scope.scopeModel.trunks.push({ trunk: values[i], message: null });
                        }
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
                      var  selectedValues = payload.selectedValues;
                        if (selectedValues != undefined) {
                            if (selectedValues.TrunkDetails != undefined && selectedValues.TrunkDetails.Trunks != undefined)
                            {
                                for (var i = 0; i < selectedValues.TrunkDetails.Trunks.length ; i++) {
                                    $scope.scopeModel.trunks.push({ trunk: selectedValues.TrunkDetails.Trunks[i].Trunk, message: null });
                                }
                            }
                        }
                    }

                };

                api.setData = function (obj) {
                    obj.TrunkDetails = {
                        $type: "TOne.WhS.BusinessEntity.Entities.PointOfInterconnect, TOne.WhS.BusinessEntity.Entities",
                        Trunks:getTruncks() 
                    };
                };

                function getTruncks() {
                    var trunks;
                    if ($scope.scopeModel.trunks != null) {
                        trunks = [];
                        for (var i = 0; i < $scope.scopeModel.trunks.length; i++) {
                            var trunk = $scope.scopeModel.trunks[i];
                            trunks.push({
                                $type: "TOne.WhS.BusinessEntity.Entities.PointOfInterconnectTrunk, TOne.WhS.BusinessEntity.Entities",
                                Trunk: trunk.trunk
                            });
                        }
                    }
                    return trunks;
                }

                if (ctrl.onReady != null) {
                    ctrl.onReady(api);
                }
                
            }

        }
        return directiveDefinitionObject;
    }]);