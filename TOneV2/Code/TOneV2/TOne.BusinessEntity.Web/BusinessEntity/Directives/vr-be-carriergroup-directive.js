'use strict';
app.directive('vrBeCarriergroup', ['VRModalService', 'UtilsService', 'VRNotificationService', 'CarrierAccountAPIService','CarrierGroupAPIService','CarrierTypeEnum', function (VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            label: "@",
            selectedvalues:"="

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

                VRModalService.showModal('/Client/Modules/BusinessEntity/Directives/Templates/CarrierGroupTree.html', null, settings);
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
        else
            label = "Carriers";
        return '<vr-columns width="normal">'
                   + '<vr-label >' + label + '</vr-label>'
               + ' <vr-select  ismultipleselection isrequired datasource="datasource" selectedvalues="selectedCarrierValues" onselectionchanged="onselectionvalueschanged" datatextfield="Name" datavaluefield="CarrierAccountID"'
               + 'entityname="' + label + '"></vr-select></vr-columns><vr-columns colnum="2">'
               + ' <span class="glyphicon glyphicon-th hand-cursor" style="top:30px" aria-hidden="true" ng-click="openTreePopup()"></span></vr-columns>';
    }
    function BeCarrierGroup(ctrl, VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum,datasource) {
        console.log(ctrl.type);
        var type;
        if (ctrl.type == "Customer")
            type = CarrierTypeEnum.Customer.value;
        else if (ctrl.type == "Supplier")
            type = CarrierTypeEnum.Supplier.value;
        else if (ctrl.type == "Exchange")
            type = CarrierTypeEnum.SaleZone.value;
            
        function initializeController() {
            loadCarriers();
        }

        function loadCarriers() {

            return CarrierAccountAPIService.GetCarriers(type).then(function (response) {
                angular.forEach(response, function (itm) {
                    datasource.push(itm);
                }); 
            });
        }

        this.initializeController = initializeController;
     
    }
    return directiveDefinitionObject;
}]);

