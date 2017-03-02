(function (app) {

    "use strict";

    vrOverallWidget.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService'];

    function vrOverallWidget(BaseDirService, VRValidationService, UtilsService) {

        return {
            restrict: 'E',
            scope: {
                datasource: '=',
                maxitemperrow: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;                
            },
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                return '<vr-datalist maxitemsperrow="{{ctrl.maxitemperrow}}" datasource="ctrl.datasource">'
                       + '<div class="overallwidget">'
                        + '<div class="overallwidget-value"><div class="widget-center-text">{{dataItem.value}}</div></div>'
                        + '<div class="overallwidget-label"><div class="widget-center-text">{{dataItem.label}}</div></div>'
                       +'</div>'
                       +'</vr-datalist>';
            }

        };
        
    }

    app.directive('vrOverallWidget', vrOverallWidget);

})(app);