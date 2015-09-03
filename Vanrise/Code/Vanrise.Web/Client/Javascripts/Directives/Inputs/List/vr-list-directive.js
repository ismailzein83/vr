'use strict';


app.directive('vrList', [ 'MultiTranscludeService', function ( MultiTranscludeService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        transclude: true,
        scope: {
            datasource: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.itemsSortable = { animation: 150 };

            this.getdatasource = function () {
                return ctrl.datasource;
            };
           
        },
        controllerAs: 'ctrl',
        bindToController: true,       
        link: function (scope, elem, attr, ctrl, transcludeFn) {
            MultiTranscludeService.transclude(elem, transcludeFn);
        },
        template: function (element, attrs) {
            var template = '';
            template =  '<div transclude-id="header"></div>'
                      + '<div ng-sortable="itemsSortable"><div ng-repeat="c in ctrl.getdatasource()"  > {{c}}  </div> </div>';
            return template;
        }


    };

    return directiveDefinitionObject;



}]);

