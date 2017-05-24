(function (app) {

    "use strict";

    vrTile.$inject = ['UtilsService', 'MultiTranscludeService', '$location'];

    function vrTile(UtilsService, MultiTranscludeService, $location) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                index: '@',
                imgpath: '@',
                datasource: '=',
                title: "@",
                target:"@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.classIndex = ctrl.index % 16;

                ctrl.onClickEvent = function () {
                    if ($attrs.target)
                        $element.find("#link").click();
                };

            },
            controllerAs: 'tileCtrl',
            bindToController: true,
            template: function (element, attrs) {
                var tileclass = attrs.target != undefined && "hand-cursor" || ""
                var template = '<div class="vr-tile ' + tileclass + ' " ng-class="\'bgcolor-{{::tileCtrl.classIndex}}\'" ng-click="tileCtrl.onClickEvent()"><a ng-if="tileCtrl.target" ng-href="{{::tileCtrl.target}}" id="link"/>'
                                    + '<div  class="section thumbnail-section" >'
                                      + '<img src="{{tileCtrl.imgpath}}" class="img-responsive" />'
                                    +'</div>'
                                     + '<div class="section content-section"  >'
                                            + '<div class="vr-tile-container">'
                                                  + '<div class="vr-tile-inner-container" >'
                                                      + ' <div class="title">{{tileCtrl.title}}</div>'
                                                      + ' <div ng-repeat="i in tileCtrl.datasource" class="tilevalue">'
                                                      + '   <span ng-if="i.name" class="itemlabel">{{i.name}}:</span><span class="itemvalue" ng-style="{\'width\': i.name ? \'50%\' : \'100%\'}">{{i.value}}</span>'
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