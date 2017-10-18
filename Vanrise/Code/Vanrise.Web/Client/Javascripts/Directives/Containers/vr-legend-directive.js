'use strict';
app.directive('vrLegend', ['$compile', 'UtilsService', function ($compile, UtilsService) {
    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            header: '@',
            content: '@',
        },
        controllerAs: 'legendCtrl',
        controller: function ($scope, $element, $attrs) {
            $scope.$on("$destroy", function () {
                $(window).unbind('scroll', hideLegendContent);
                $(window).unbind('resize', hideLegendContent);
            });
            var ctrl = this;
            var showContent = false;
            ctrl.id = UtilsService.guid();
            ctrl.toogleLegendContent = function (evt) {
                if (showContent == false) {
                    showLegendContent()
                }
                else {
                    hideLegendContent();
                }
            };

            function showLegendContent() {
                calculatePosition();
                $element.find('#MainContent').slideDown("slow", function () {
                    showContent = true;
                    $(document).bind('click', boundDocumentLengendOutside);
                });
            }

            function hideLegendContent() {
                $element.find('#MainContent').slideUp("slow", function () {
                    showContent = false;
                    $(document).unbind('click', boundDocumentLengendOutside);
                });
            }

            //function hideAllother


            function boundDocumentLengendOutside(e) {
                if (!$(e.target).parents().hasClass('vr-legend-' + ctrl.id) || $(e.target).hasClass('legend-header-container-' + ctrl.id))
                    hideLegendContent();
            }
            $scope.$on('hide-all-menu', function (event, args) {
                hideLegendContent();
            });
            $scope.$on('hide-all-select', function (event, args) {
                hideLegendContent();
            });
            $(window).on('scroll', hideLegendContent);
            $(window).on('resize', hideLegendContent);
            ctrl.hideLegend = hideLegendContent;
            function calculatePosition() {
                var self = $element.find('#LegendHeader').parent();
                var selfHeight = $(self).height();
                var selfOffset = $(self).offset();
                var legend = $element.find('#MainContent');
                var legendHeigth = $(legend).height();
                var top = 0;
                var basetop = selfOffset.top - $(window).scrollTop() + selfHeight - 10;
                var baseleft = selfOffset.left - $(window).scrollLeft();
                top = (innerHeight - basetop < legendHeigth + 200) ? basetop - (legendHeigth) - selfHeight : basetop;
                $(legend).css({position: 'fixed',top: top ,left: baseleft, width: self.width()});
            };

            var htmlContent = $compile(ctrl.content)($scope.$new());
            $element.find('#LengendContent').html(htmlContent);
        },
        bindToController: true,
        template: function (element, attrs) {
            var template = '<div class="vr-legend-{{legendCtrl.id}}">'
                              + '<div class="legend-header-container-{{legendCtrl.id}}"><label  ng-click="legendCtrl.toogleLegendContent($event)" id="LegendHeader" class="legend-header">{{legendCtrl.header}}</label></div>'
                              + '<div  id="MainContent" class="main-content">'
                                  + '<div class="legend-container">'
                                    + '<div class="legend-close" ng-click="legendCtrl.hideLegend($event)">X</div>'
                                    + '<div id="LengendContent"></div>'
                                  + '</div>'
                               + '</div>'
                         + '</div>';

            return template;
        }

    };

    return directiveDefinitionObject;
}]);




