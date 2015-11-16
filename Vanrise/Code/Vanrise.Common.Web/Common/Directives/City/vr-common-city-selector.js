'use strict';
app.directive('vrCommonCitySelector', ['VRCommon_CityAPIService', 'VRCommon_CityService', 'UtilsService', 'VRUIUtilsService',
    function (VRCommon_CityAPIService, VRCommon_CityService, UtilsService, VRUIUtilsService) {

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

                $scope.selectedCityValues;
                if ($attrs.ismultipleselection != undefined)
                    $scope.selectedCityValues = [];

                $scope.addNewCity = function () {
                    var onCityAdded = function (cityObj) {
                        $scope.datasource.length = 0;
                        return getAllCities($scope, VRCommon_CityAPIService).then(function (response) {
                            if ($attrs.ismultipleselection == undefined)
                                $scope.selectedCityValues = UtilsService.getItemByVal($scope.datasource, cityObj.Entity.CityId, "CityId");
                        }).catch(function (error) {
                        }).finally(function () {

                        });;
                    };
                    VRCommon_CityService.addCity(onCityAdded);
                }
                $scope.datasource = [];
                var beCity = new City(ctrl, $scope, VRCommon_CityAPIService, $attrs);
                beCity.initializeController();
                $scope.onselectionchanged = function () {
                    ctrl.selectedvalues = $scope.selectedCityValues;
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
                    var template = getBeCityTemplate(iAttrs, ctrl);
                    iElement.html(template);
                    $compile(iElement.contents())($scope);
                });
            }

        };
        function getBeCityTemplate(attrs, ctrl) {
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
                addCliked = 'onaddclicked="addNewCity"';
            if (attrs.ismultipleselection != undefined)
                return ' <vr-select ismultipleselection datasource="datasource" ' + required + ' ' + hideselectedvaluessection + ' selectedvalues="selectedCityValues" ' + disabled + ' onselectionchanged="onselectionchanged" datatextfield="Name" datavaluefield="CityId"'
                       + 'entityname="City" label="City" ' + addCliked + '></vr-select>';
            else
                return '<div vr-loader="isLoadingDirective" style="display:inline-block;width:100%">'
                   + ' <vr-select datasource="datasource" selectedvalues="selectedCityValues" ' + required + ' ' + hideselectedvaluessection + ' onselectionchanged="onselectionchanged"  ' + disabled + ' datatextfield="Name" datavaluefield="CityId"'
                   + 'entityname="City" label="City" ' + addCliked + '></vr-select></div>';
        }
        function City(ctrl, $scope, VRCommon_CityAPIService, $attrs) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};


                api.getData = function () {
                    return $scope.selectedCityValues;
                }
                api.getDataId = function () {
                    return $scope.selectedCityValues.CityId;
                }
                api.getIdsData = function () {
                    return getIdsList($scope.selectedCityValues, "CityId");
                }
               
                function getIdsList(tab, attname) {
                    var list = [];
                    for (var i = 0; i < tab.length ; i++)
                        list[list.length] = tab[i][attname];
                    return list;

                }
                api.load = function (payload) {

                    var filter;
                    var selectedIds;
                    if (payload != undefined) {
                        filter = payload.filter;
                        selectedIds = payload.selectedIds;
                    }

                    var serializedFilter = {};
                    if (filter != undefined)
                        serializedFilter = UtilsService.serializetoJson(filter);


                    return VRCommon_CityAPIService.GetCitiesInfo(serializedFilter).then(function (response) {
                        angular.forEach(response, function (itm) {
                            $scope.datasource.push(itm);
                        });

                        if (selectedIds != undefined) {
                            VRUIUtilsService.setSelectedValues(selectedIds, 'CityId', $attrs, ctrl);
                        }
                    });
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;

        }

        function getAllCities($scope, VRCommon_CityAPIService) {
            return VRCommon_CityAPIService.GetCitiesInfo().then(function (response) {
                angular.forEach(response, function (itm) {
                    $scope.datasource.push(itm);
                });
            });
        }
        return directiveDefinitionObject;
    }]);

