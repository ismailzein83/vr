(function (app) {

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
                maxitemsperrow: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.classIndex = ctrl.index % 16;

                ctrl.onClickEvent = function () {
                    if ($attrs.target)
                        $element.find("#link").click();
                };

                var maxItemsPerRow = ctrl.maxitemsperrow != undefined ? parseInt(ctrl.maxitemsperrow) : 1;
                var colNumber,mdColnumber;
                switch (maxItemsPerRow) {
                    case 1: colNumber = 12;mdColnumber = 12; break;
                    case 2: colNumber = 6;mdColnumber = 12; break;
                    case 3: colNumber = 4;mdColnumber = 6; break;
                    case 4: colNumber = 3;mdColnumber = 4; break;
                    case 5: colNumber = 3; mdColnumber =4; break;
                    case 6: colNumber = 2; mdColnumber = 3; break;
                }
                if (colNumber == undefined) {
                    if (maxItemsPerRow < 12)
                        colNumber = 2;
                    else colNumber = 1;
                }
                ctrl.colNumber = colNumber;
                ctrl.mdColNumber = mdColnumber;

                ctrl.getObjectClass = function (item) {
                    if (typeof item["className"]) {
                        var color = item["className"];
                        if (typeof color === 'string' || typeof color instanceof String) {
                            return color+ "-figure";
                        }
                        else if (typeof color === 'object' || color instanceof Object) {
                            switch (color.UniqueName) {
                                case "VR_AccountBalance_StyleFormating_CSSClass": return color.ClassName + "-figure";
                            }
                        }
                    }
                };

            },
            controllerAs: 'vrFigure',
            bindToController: true,
            template: function (element, attrs) {
                var template = '<div class="vr-figure" >'
                                   + ' <div class="title" ng-if="vrFigure.header">{{vrFigure.header}}</div>'
                                    + '<div class="figure-content">'
                                        + '<div class="figure-info-content" >'
                                          + '<img ng-src="{{vrFigure.imgpath}}" style="padding-top: 5px;"  />'
                                        + '</div>'
                                         + '<div class="figure-data-content">'
                                         + '<div style="width:100%;min-height: 141px;position: relative;" >'
                                             + '<div style="position: absolute;top: 50%;transform: translate(0px,-50%);text-align: center;width: 100%;">'
                                                  + '<div class="col-lg-{{vrFigure.colNumber}} col-md-{{vrFigure.mdColNumber}}"  ng-repeat="i in vrFigure.datasource">'
                                                       + ' <div class="figure-item" class="tilevalue ">'
                                                        + '  <div class="item-value {{!i.name ? \'centered\':\'\'}}" ng-class="vrFigure.getObjectClass(i)">{{i.value}}</div><div ng-if="i.name" class="item-label" title="{{i.name}}">{{i.name}}</div>'
                                                        + '</div>'
                                                  + '</div>'
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