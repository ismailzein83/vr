'use strict';

app.directive('vrWhsBePeriodSelector', ['WhS_BE_PeriodTypeEnum', 'UtilsService', 'VRUIUtilsService', function (WhS_BE_PeriodTypeEnum, UtilsService, VRUIUtilsService) {
    return {
        restrict: 'E',
        scope: {
            onReady: '=',
            label: '@',
            normalColNum: '@',
            isrequired: "@"
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var periodSelector = new PeriodSelector(ctrl, $scope, $attrs);
            periodSelector.initializeController();
        },
        controllerAs: 'periodCtrl',
        bindToController: true,
        template: function (element, attrs) {
            return getTemplate(attrs);
        }
    };

    function PeriodSelector(ctrl, $scope, $attrs) {

        this.initializeController = initializeController;

        var selectorAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.periodTypes = [];

            $scope.scopeModel.isValid = function () {
                if ($scope.scopeModel.periodValue == undefined || $scope.scopeModel.periodValue == null || $scope.scopeModel.periodValue == '')
                    return 'Period value is required';

                var periodValueAsNumber = Number($scope.scopeModel.periodValue);
                if (isNaN(periodValueAsNumber))
                    return 'Period value is an invalid number';

                if (periodValueAsNumber <= 0)
                    return 'Period value must be positive';

                return null;
            };

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                defineAPI();
            };
        }
        function defineAPI() {

            var api = {};

            api.load = function (payload) {
                selectorAPI.clearDataSource();

                var period;

                if (payload != undefined) {
                    period = payload.period;
                }

                $scope.scopeModel.periodTypes = UtilsService.getArrayEnum(WhS_BE_PeriodTypeEnum);

                if (period != undefined) {
                    $scope.scopeModel.periodValue = period.periodValue;

                    if (period.periodType != null) {
                        $scope.scopeModel.selectedPeriodType = UtilsService.getItemByVal($scope.scopeModel.periodTypes, period.periodType, 'value');
                    }
                }
            };

            api.getData = function () {
                var data = {
                    periodValue: $scope.scopeModel.periodValue
                };
                if ($scope.scopeModel.selectedPeriodType != undefined) {
                    data.periodType = $scope.scopeModel.selectedPeriodType.value;
                }
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    function getTemplate(attrs) {

        var label = (attrs.label != undefined) ? attrs.label : 'Period';

        return '<vr-columns colnum="{{periodCtrl.normalColNum}}">\
                    <vr-textbox type="number" label="' + label + '" value="scopeModel.periodValue" customvalidate="scopeModel.isValid()" isrequired="periodCtrl.isrequired"></vr-textbox>\
                </vr-columns>\
                <vr-columns colnum="{{periodCtrl.normalColNum}}" withemptyline>\
                    <vr-select on-ready="scopeModel.onSelectorReady"\
                        datasource="scopeModel.periodTypes"\
                        datavaluefield="value"\
                        datatextfield="description"\
                        selectedvalues="scopeModel.selectedPeriodType"\
                        isrequired="periodCtrl.isrequired"\
                        hideremoveicon="periodCtrl.isrequired"\
                    </vr-select>\
                </vr-columns>';
    }
}]);