'use strict';
app.directive('vrBeCarriergroup', ['VRModalService', 'UtilsService', 'VRNotificationService', 'CarrierAccountAPIService','CarrierGroupAPIService','CarrierTypeEnum', function (VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            label: "@",
            selectedvalues:"=",
            isassignedcarrier: "@"
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            $scope.selectedCarrierValues = [];
            $scope.datasource = [];
            var beCarrierGroup = new BeCarrierGroup(ctrl, VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum, $scope.datasource);
            beCarrierGroup.initializeController();
            $scope.openTreePopup = function () {
                var settings = {
                    useModalTemplate: true,
                };
                settings.onScopeReady = function (modalScope) {
                    modalScope.title = ctrl.type+ " Group";
                    modalScope.onTreeSelected = function (selectedtNode) {
                        $scope.currentNode = undefined;
                        $scope.selectedtNode = selectedtNode;

                        CarrierGroupAPIService.GetCarrierGroupMembersDesc($scope.selectedtNode.EntityId).then(function (response) {
                            $scope.selectedCarrierValues.length = 0;
                            angular.forEach(response, function (item) {
                                $scope.selectedCarrierValues.push(item);
                            });
                            ctrl.selectedvalues = $scope.selectedCarrierValues;
                        }).catch(function (error) {
                            $scope.isGettingData = false;
                            VRNotificationService.notifyExceptionWithClose(error, $scope);
                        });

                    };
                };

                var parameters = {};

                if (ctrl.type == "Customer")
                    parameters.carrierType = CarrierTypeEnum.Customer.value;
                else if (ctrl.type == "Supplier")
                    parameters.carrierType = CarrierTypeEnum.Supplier.value;
                else if (ctrl.type == "Exchange")
                    parameters.carrierType = CarrierTypeEnum.SaleZone.value;

                parameters.assignedCarrier = ctrl.isassignedcarrier != undefined ? true : false;
                

                VRModalService.showModal('/Client/Modules/BusinessEntity/Directives/Templates/CarrierGroupTree.html', parameters, settings);
            }
            $scope.onselectionvalueschanged = function () {
                ctrl.selectedvalues=$scope.selectedCarrierValues;
               
            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {
                    $scope.$watch('ctrl.selectedvalues.length', function () {
                         $scope.selectedCarrierValues = ctrl.selectedvalues;
                        if (iAttrs.onselectionchanged != undefined) {
                            var onvaluechangedMethod = $scope.$parent.$eval(iAttrs.onselectionchanged);
                            if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                                onvaluechangedMethod();
                            }
                        }
                    });
                }
            }
        },
        template: function (element, attrs) {
            return getBeCarrierGroupTemplate(attrs);
        }

    };
    function getBeCarrierGroupTemplate(attrs) {
        var label;
        if (attrs.label != undefined)
            label = attrs.label;
        else {
            if (attrs.type == "'Customer'")
                label = "Customers";
            else if (attrs.type == "'Supplier'")
                label = "Suppliers";
            else if (attrs.type == "'Exchange'")
                label = "Carriers";
        }
           
        return '<div style="display:inline-block;width: calc(100% - 18px);">'
                   + '<vr-label >' + label + '</vr-label>'
               + '<vr-select  ismultipleselection  datasource="datasource" selectedvalues="selectedCarrierValues" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="CarrierAccountID"'
               + 'entityname="' + label + '"></vr-select></div>'
               + ' <span class="glyphicon glyphicon-th hand-cursor"  aria-hidden="true" ng-click="openTreePopup()"></span></div>';
    }
    function BeCarrierGroup(ctrl, VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum,datasource) {
        var type;
        if (ctrl.type == "Customer")
            type = CarrierTypeEnum.Customer.value;
        else if (ctrl.type == "Supplier")
            type = CarrierTypeEnum.Supplier.value;
        else if (ctrl.type == "Exchange")
            type = CarrierTypeEnum.SaleZone.value;
        var isAssignedCarrier = ctrl.isassignedcarrier != undefined ? true : false;
        function initializeController() {
            loadCarriers();
        }

        function loadCarriers() {
            
            return CarrierAccountAPIService.GetCarriers(type, isAssignedCarrier).then(function (response) {
                angular.forEach(response, function (itm) {
                    datasource.push(itm);
                }); 
            });
        }

        this.initializeController = initializeController;
     
    }
    return directiveDefinitionObject;
}]);

