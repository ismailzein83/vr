(function (app) {

    "use strict";

    vrDropDownSection.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService', 'MultiTranscludeService'];

    function vrDropDownSection(BaseDirService, VRValidationService, UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                onReady: '=',
                defaultstate:'='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                ctrl.id = BaseDirService.generateHTMLElementName();
                ctrl.menuid = BaseDirService.generateHTMLElementName();
                ctrl.initializeSection =  function() {
                    ctrl.showmenu = true;
                    setTimeout(function () {
                        calculatePosition(true);
                    }, 300);
                };
               
                ctrl.expandSection = function (e) {
                    if (e != undefined && $(e.target).hasClass('vanrise-inpute')) {
                        return;
                    }
                    calculatePosition();
                    setTimeout(function () {                        
                        $scope.$apply(function () {                          
                            ctrl.showmenu = true;                           
                        });
                        $('#' + ctrl.menuid).slideDown("slow");
                    });
                };
                ctrl.collapseSection = function () {
                    $('#' + ctrl.menuid).slideUp("slow", function () {
                        ctrl.showmenu = false;
                    });
                };              

                $('#' + ctrl.id).parents('div').scroll(function () {
                    fixDropdownPosition();
                });
                $(window).scroll(function () {
                    fixDropdownPosition();
                });
                $(window).resize(function () {
                    fixDropdownPosition();
                });

                var fixDropdownPosition = function () {
                    ctrl.collapseSection(undefined);
                };
                $scope.$on('start-drag', function (event, args) {
                    fixDropdownPosition();
                });

                function calculatePosition(forcedisplay) {
                  //  setTimeout(function () {
                        var self = $('#' + ctrl.id).find('.summary-container');

                        var selfHeight = $(self).parent().height();
                        var selfOffset = $(self).offset();
                        var dropDown = $('#' + ctrl.menuid);
                        var top = 0;
                        var basetop = selfOffset.top - $(window).scrollTop();
                        var baseleft = selfOffset.left - $(window).scrollLeft();

                        var height = ctrl.bodysectionheight;
                         console.log($(dropDown).height())
                        if (innerHeight - basetop < height + 100)
                            top = basetop - (height);
                        else
                            top = selfOffset.top - $(window).scrollTop() - 2;
                        var style = {
                            position: 'fixed',
                            top: top,
                            left: baseleft,
                            width: self.parent().width()
                        };
                        if (forcedisplay == true)
                            style.display = 'block';
                        $(dropDown).css(style);
                  //  }, 100);
                };
               
            },
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;


                        

                    },
                    post: function (scope, elem, attr, ctrl, transcludeFn) {

                        MultiTranscludeService.transclude(elem, transcludeFn);
                        if (ctrl.defaultstate == true)
                            ctrl.initializeSection();
                        //ctrl.collapseSection();
                        var api = {};
                        api.collapse = function () {
                            ctrl.collapseSection();
                        };
                        api.expand = function () {
                            ctrl.expandSection(undefined);
                        };
                        if (ctrl.onReady != null)
                            ctrl.onReady(api);
                    },
                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv">';
                var endTemplate = '</div>';

                var sectionTemplate = '<div style="position: relative;" >'
                            + '<vr-validation-group validationcontext="ctrl.validationContext">'
                            + '<div class="vr-dropdown-section" id="{{ctrl.id}}" >'
                                + '<div class="summary-container hand-cursor" ng-click="ctrl.expandSection($event)" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' " >'
                                    + '<div transclude-id="summary" class="summary-main"></div>'
                                    + '<span  class="glyphicon  glyphicon-sort-by-attributes  toogle-icon"></span>'
                                    + '<span ng-if="ctrl.validationContext.validate() != null" class="validation-sign"  title="has validation errors!">*</span>'
                              + '</div>'
                                + '<ul class="dropdown-menu drop-down-section"   id="{{ctrl.menuid}}" ng-show="ctrl.showmenu" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' "  >'
                                    + '<li role="presentation" >'
                                        + '<span class="hand-cursor glyphicon  glyphicon-remove remove-icon" ng-click="ctrl.collapseSection($event,false)" ></span>'
                                        + '<div class="section-body" transclude-id="body" ng-click="ctrl.muteAction($event);"></div>'
                                    + '</li>'
                                + '</ul>'
                            + '</vr-validation-group>'
                        + '</div>';

                ;

                return startTemplate + sectionTemplate + endTemplate;
            }

        };

    }

    app.directive('vrDropdownSection', vrDropDownSection);

})(app);



