﻿(function (app) {

    "use strict";

    vrFigure.$inject = ['UtilsService', 'MultiTranscludeService', '$location'];

    function vrFigure(UtilsService, MultiTranscludeService, $location) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                index: '@',
                imgpath: '@',
                datasource: '=',
                header: "@",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.classIndex = ctrl.index % 16;

                ctrl.onClickEvent = function () {
                    if ($attrs.target)
                        $element.find("#link").click();
                };

            },
            controllerAs: 'vrFigure',
            bindToController: true,
            template: function (element, attrs) {
                var template = '<div class="vr-figure" >'
                                   + ' <div class="title">{{vrFigure.header}}</div>'
                                    + '<div class="figure-content">'
                                        + '<div class="figure-info-content">'
                                          + '<img ng-src="{{vrFigure.imgpath}}" style="padding-top: 5px;"  />'
                                        + '</div>'
                                         + '<div class="figure-data-content">'
                                         + '<div style="width:100%;min-height: 100px;" >'
                                              + '<div style="width:100% ; padding-top: 5px;"  ng-repeat="i in vrFigure.datasource">'
                                                   + ' <div class="figure-item" class="tilevalue">'
                                                    + '   <span ng-if="i.name" class="item-label" title="{{i.name}}"><div>{{i.name}}:</div></span><span class="item-value" ng-class="!i.name ? \'centered\':\'\'">{{i.value}}</span>'
                                                    + ' </div>'
                                              + '</div>'
                                          + '</div>'
                                     + '</div>'
                                + '</div>';

                return template;
            }

        };

    }

    app.directive('vrFigure', vrFigure);

})(app);