(function (app) {

    "use strict";

    vrTile.$inject = ['UtilsService', 'MultiTranscludeService'];

    function vrTile( UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                index: '@',
                imgpath: '@',
                datasource: '=',
                title:"@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.classIndex = ctrl.index % 16;

                ctrl.onClickEvent = function () {
                    if ($attrs.onclicktile != undefined) {
                        var onClickTileMethod = $scope.$parent.$eval($attrs.onclicktile);
                        if (onClickTileMethod != undefined && typeof (onClickTileMethod) == 'function') {
                            onClickTileMethod();
                        }
                    }
                };

            },
            controllerAs: 'tileCtrl',
            bindToController: true,
            template: function (element, attrs) {
                var template = '<div class="vr-tile hand-cursor " ng-class="\'bgcolor-{{::tileCtrl.classIndex}}\'" ng-click="tileCtrl.onClickEvent($event)">'
                                    + '<div  class="section thumbnail-section" >'
                                      + '<img src="{{tileCtrl.imgpath}}" class="img-responsive" />'
                                    +'</div>'
                                     + '<div class="section content-section"  >'
                                            + '<div class="vr-tile-container">'
                                                  + '<div class="vr-tile-inner-container" >'
                                                      + ' <div class="title">{{tileCtrl.title}}</div>'
                                                      + ' <div ng-repeat="i in tileCtrl.datasource" class="tilevalue">'
                                                      + '   <span ng-if="i.name" class="itemlabel">{{i.name}} :</span><span class="itemvalue" ng-style="{\'width\': i.name ? \'50%\' : \'100%\'}">{{i.value}}</span>'
                                                      + ' </div>'
                                                  +'</div>'
                                            + '</div>'
                                    + '</div>'
                                + '</div>';

                ;

                return template;
            }

        };
      
    }

    app.directive('vrTile', vrTile);

})(app);