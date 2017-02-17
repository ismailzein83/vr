(function (app) {

    "use strict";

    vrDropDownSection.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService', 'MultiTranscludeService'];

    function vrDropDownSection(BaseDirService, VRValidationService, UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var api = {};
                api.collapse = function () {
                    if (ctrl.showmenu == true)
                        ctrl.toggelSectionMenu(false);
                };
                api.expand = function () {
                    if (ctrl.showmenu == false)
                        ctrl.toggelSectionMenu(true);
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            },
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;


                        ctrl.id = BaseDirService.generateHTMLElementName();
                        ctrl.menuid = BaseDirService.generateHTMLElementName();

                        ctrl.getSummaryText = function () {
                            return "Summary";
                        };
                        ctrl.showmenu = true;
                        
                        ctrl.toggelSectionMenu = function (e, bool) {    
                            if (e != undefined && $(e.target).hasClass('vanrise-inpute')) {
                                return;
                            }
                          
                            if (ctrl.showmenu == false) {
                                var self = $('#' + ctrl.id).find('.summary-container');
                                var selfHeight = $(self).parent().height();
                                var selfOffset = $(self).offset();
                                var dropDown = $('#' + ctrl.menuid);
                                var top = 0;
                                var basetop = selfOffset.top - $(window).scrollTop();
                                var baseleft = selfOffset.left - $(window).scrollLeft();
                                ctrl.showmenu = true;
                                var height = ctrl.bodysectionheight;
                                if (innerHeight - basetop < height + 100)
                                    top = basetop - (height);
                                else
                                    top = selfOffset.top - $(window).scrollTop() - 2;

                                $(dropDown).css({ position: 'fixed', top: top, left: baseleft, width: self.parent().width() });
                                setTimeout(function () {
                                    $scope.$apply(function () {
                                        ctrl.showmenu = true;
                                    });
                                    $('#' + ctrl.menuid).slideDown("slow");
                                })
                            }
                            else {
                                $('#' + ctrl.menuid).slideUp("slow", function () {
                                    ctrl.showmenu = false;
                                });
                            }
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
                            $('.vr-section-dropdown').find('.dropdown-menu').hide();

                        };
                        $scope.$on('start-drag', function (event, args) {
                            fixDropdownPosition();
                        });


                    },
                    post: function (scope, elem, attr, ctrl, transcludeFn) {

                        MultiTranscludeService.transclude(elem, transcludeFn);
                        setTimeout(function () {
                            ctrl.toggelSectionMenu(undefined, false);
                        }, 1000);

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
                                + '<div class="summary-container hand-cursor" ng-click="ctrl.toggelSectionMenu($event)" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' " >'
                                    + '<div transclude-id="summary" class="summary-main"></div>'
                                    + '<span  class="glyphicon  glyphicon-sort-by-attributes  toogle-icon"></span>'
                                    + '<span ng-if="ctrl.validationContext.validate() != null" class="validation-sign"  title="has validation errors!">*</span>'
                              + '</div>'
                                + '<ul class="dropdown-menu drop-down-section"   id="{{ctrl.menuid}}" ng-show="ctrl.showmenu" ng-class="ctrl.validationContext.validate() != null  ? \'invalid-section\':\'\' "  >'
                                    + '<li role="presentation" >'
                                        + '<span class="hand-cursor glyphicon  glyphicon-remove remove-icon" ng-click="ctrl.toggelSectionMenu($event,false)" ></span>'
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



