'use strict';
app.directive('vrWhsBeRateTypeSelector', ['WhS_BE_RateTypeAPIService', 'WhS_BE_MainService', 'UtilsService', '$compile', function (WhS_BE_RateTypeAPIService, WhS_BE_MainService, UtilsService, $compile) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            type: "=",
            onReady: '=',
            label: "@",
            ismultipleselection: "@",
            hideselectedvaluessection: '@',
            onselectionchanged: '=',
            isrequired: '@',
            isdisabled: "=",
            selectedvalues: "=",
            showaddbutton: '@'
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            $scope.selectedRateTypeValues;
            if ($attrs.ismultipleselection != undefined)
                $scope.selectedRateTypeValues = [];

            $scope.addNewRateType = function () {
                var onRateTypeAdded = function (rateTypeObj) {
                    $scope.datasource.length = 0;
                    return getAllRateTypes($scope, WhS_BE_RateTypeAPIService).then(function (response) {
                        if ($attrs.ismultipleselection == undefined)
                            $scope.selectedRateTypeValues = UtilsService.getItemByVal($scope.datasource, rateTypeObj.RateTypeId, "RateTypeId");
                    }).catch(function (error) {
                    }).finally(function () {

                    });;
                };
                WhS_BE_MainService.addRateType(onRateTypeAdded);
            }
            $scope.datasource = [];
            var beRateType = new BeRateType(ctrl, $scope, WhS_BE_RateTypeAPIService, $attrs);
            beRateType.initializeController();
            $scope.onselectionchanged = function () {
                ctrl.selectedvalues = $scope.selectedRateTypeValues;
                if (ctrl.onselectionchanged != undefined) {
                    var onvaluechangedMethod = $scope.$parent.$eval(ctrl.onselectionchanged);
                    if (onvaluechangedMethod != undefined && onvaluechangedMethod != null && typeof (onvaluechangedMethod) == 'function') {
                        onvaluechangedMethod();
                    }
                }

            }

        },
        controllerAs: 'ctrl',
        bindToController: true,
        link: function preLink($scope, iElement, iAttrs) {
            var ctrl = $scope.ctrl;
            $scope.$watch('ctrl.isdisabled', function () {
                var template = getBeRateTypeTemplate(iAttrs, ctrl);
                iElement.html(template);
                $compile(iElement.contents())($scope);
            });
        }

    };
    function getBeRateTypeTemplate(attrs, ctrl) {
        var label;
        var disabled = "";
        if (ctrl.isdisabled)
            disabled = "vr-disabled='true'"

        var required = "";
        if (attrs.isrequired != undefined)
            required = "isrequired";

        var hideselectedvaluessection = "";
        if (attrs.hideselectedvaluessection != undefined)
            hideselectedvaluessection = "hideselectedvaluessection";
        var addCliked = '';
        if (attrs.showaddbutton != undefined)
            addCliked = 'onaddclicked="addNewRateType"';
        if (attrs.ismultipleselection != undefined)
            return ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedRateTypeValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="RateTypeId"'
                   + 'entityname="RateType" label="RateType" ' + addCliked + '></vr-select>';
        else
            return '<div vr-loader="isLoadingDirective" style="display:inline-block;width:100%">'
               + ' <vr-select datasource="datasource" selectedvalues="selectedRateTypeValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="RateTypeId"'
               + 'entityname="RateType" label="RateType" ' + addCliked + '></vr-select></div>';
    }
    function BeRateType(ctrl, $scope, WhS_BE_RateTypeAPIService, $attrs) {

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};


            api.getData = function () {
                return $scope.selectedRateTypeValues;
            }
            api.getDataId = function () {
                return $scope.selectedRateTypeValues.RateTypeId;
            }
            api.getIdsData = function () {
                return getIdsList($scope.selectedRateTypeValues, "RateTypeId");
            }
            api.setData = function (selectedIds) {
                if ($attrs.ismultipleselection != undefined) {
                    for (var i = 0; i < selectedIds.length; i++) {
                        var selectedRateTypeValue = UtilsService.getItemByVal($scope.datasource, selectedIds[i], "RateTypeId");
                        if (selectedRateTypeValue != null)
                            $scope.selectedRateTypeValues.push(selectedRateTypeValue);
                    }
                } else {
                    var selectedRateTypeValue = UtilsService.getItemByVal($scope.datasource, selectedIds, "RateTypeId");
                    if (selectedRateTypeValue != null)
                        $scope.selectedRateTypeValues = selectedRateTypeValue;
                }
            }
            function getIdsList(tab, attname) {
                var list = [];
                for (var i = 0; i < tab.length ; i++)
                    list[list.length] = tab[i][attname];
                return list;

            }
            api.load = function () {

                return WhS_BE_RateTypeAPIService.GetAllRateTypes().then(function (response) {
                    angular.forEach(response, function (itm) {
                        $scope.datasource.push(itm);
                    });
                }).catch(function (error) {
                }).finally(function () {

                });;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;

    }

    function getAllRateTypes($scope, WhS_BE_RateTypeAPIService) {
        return WhS_BE_RateTypeAPIService.GetAllRateTypes().then(function (response) {
            angular.forEach(response, function (itm) {
                $scope.datasource.push(itm);
            });
        });
    }
    return directiveDefinitionObject;
}]);

