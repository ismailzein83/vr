'use strict';
app.directive('vrBeCarriergroup', ['VRModalService', 'UtilsService', 'VRNotificationService', 'CarrierAccountAPIService','CarrierTypeEnum', function (VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
                type:"="

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
          //  var retrieveDataOnLoad = $scope.$parent.$eval($attrs.retrievedataonload);

            var beCarrierGroup = new BeCarrierGroup(ctrl, ctrl.settings, VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum);
            beCarrierGroup.initializeController();



        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {
            console.log(element);
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        template: function (element, attrs) {
            return getBeCarrierGroupTemplate(attrs.previewmode);
        }

    };
    function getBeCarrierGroupTemplate() {
        return '<vr-columns colnum="6">'
               + ' <vr-select label="Carrier" ismultipleselection isrequired datasource="datasource" selectedvalues="selectedvalues" datatextfield="Name" datavaluefield="CarrierAccountID"'
               + 'entityname="Carriers"></vr-select></vr-columns><vr-columns colnum="2">'
               + ' <span class="glyphicon glyphicon-th hand-cursor" style="top:30px" aria-hidden="true" ng-click="openTreePopup()"></span></vr-columns>';
    }
    function BeCarrierGroup(ctrl, settings, VRModalService, UtilsService, VRNotificationService, CarrierAccountAPIService, CarrierGroupAPIService, CarrierTypeEnum) {
        console.log("test");
        function initializeController() {
            loadCarriers();

                $scope.selectedvalues = [];

                $scope.datasource = [];

                $scope.openTreePopup = function () {
                    var settings = {
                        useModalTemplate: true,
                    };
                    settings.onScopeReady = function (modalScope) {
                        modalScope.title = "Carrier Group";
                        modalScope.onTreeSelected = function (selectedtNode) {
                            $scope.currentNode = undefined;
                            $scope.selectedtNode = selectedtNode;

                            console.log($scope.selectedtNode.EntityId);

                            //Load Selected
                            CarrierGroupAPIService.GetCarrierGroupMembersDesc($scope.selectedtNode.EntityId).then(function (response) {
                                $scope.selectedvalues = [];
                                angular.forEach(response, function (item) {
                                    $scope.selectedvalues.push(item);
                                });
                            }).catch(function (error) {
                                $scope.isGettingData = false;
                                VRNotificationService.notifyExceptionWithClose(error, $scope);
                            });

                        };
                    };

                    VRModalService.showModal('/Client/Modules/BusinessEntity/Views/TestCarrierGroupTree.html', null, settings);
                }

        }

        function retrieveData(type) {
            ctrl.isGettingData = true;
            return BIVisualElementService.retrieveWidgetData(ctrl, settings, filter)

                .then(function (response) {
                    if (ctrl.isDateTimeGroupedData) {
                        BIUtilitiesService.fillDateTimeProperties(response, filter.timeDimensionType.value, filter.fromDate, filter.toDate, false);
                        refreshChart(response);
                    }
                    else
                        refreshPIEChart(response);
                }).finally(function () {
                    ctrl.isGettingData = false;
                });

        }

        function loadCarriers() {
            return CarrierAccountAPIService.GetCarriers(CarrierTypeEnum.SaleZone.value).then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.datasource.push(itm);
                });
            });
        }

        this.initializeController = initializeController;
     
    }
    return directiveDefinitionObject;
}]);

