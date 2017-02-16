(function (app) {

    "use strict";

    vrDropDownSection.$inject = ['BaseDirService', 'VRValidationService', 'UtilsService', 'MultiTranscludeService'];

    function vrDropDownSection(BaseDirService, VRValidationService, UtilsService, MultiTranscludeService) {

        return {
            restrict: 'E',
            transclude: true,
            scope: {
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
            },
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs) {
                        var ctrl = $scope.ctrl;


                        ctrl.id = BaseDirService.generateHTMLElementName();
                        ctrl.menuid = BaseDirService.generateHTMLElementName();

                        //ctrl.muteAction = function (e) {
                        //    BaseDirService.muteAction(e);
                        //};

                        ctrl.getSummaryText = function () {
                            return "Summary";
                        };
                        ctrl.showmenu = false;
                        ctrl.pinned = false;
                        ctrl.toggelPinBody = function (e) {
                            ctrl.pinned = !ctrl.pinned;
                            if (ctrl.pinned == false)
                              ctrl.toggelSectionMenu(e, false);
                        }
                        ctrl.toggelSectionMenu = function (e, bool) {
                            if (ctrl.pinned == true)
                                return;
                            if (bool != undefined) {

                                $('#' + ctrl.menuid).slideUp("slow", function () {
                                    $scope.$apply(function () {
                                        ctrl.showmenu = bool;
                                    });
                                });
                            }

                            else {
                                if (ctrl.showmenu == false) {
                                    var self = angular.element(e.currentTarget);
                                    var selfHeight = $(self).parent().height();
                                    var selfOffset = $(self).offset();
                                    var dropDown = self.parent().find('ul');
                                    var top = 0;
                                    var basetop = selfOffset.top - $(window).scrollTop() + selfHeight;
                                    var baseleft = selfOffset.left - $(window).scrollLeft();

                                    var heigth = self.height() + 120;
                                    if (innerHeight - basetop < heigth)
                                        top = basetop - (heigth + (selfHeight * 2.7));
                                    else
                                        top = selfOffset.top - $(window).scrollTop() + selfHeight;

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
                            $('#' + ctrl.id).removeClass("open");

                        };
                        $scope.$on('start-drag', function (event, args) {
                            fixDropdownPosition();
                        });


                    },
                    post: function (scope, elem, attr, ctrl, transcludeFn) {
                        MultiTranscludeService.transclude(elem, transcludeFn);
                    },

                }
            },

            controllerAs: 'ctrl',
            bindToController: true,
            template: function (element, attrs) {
                var startTemplate = '<div id="rootDiv"  click-outside="ctrl.toggelSectionMenu($event, false)">';
                var endTemplate = '</div>';

                var sectionTemplate = '<div style="position: relative;" >'
                            + '<vr-validation-group validationcontext="ctrl.validationContext">'
                            + '<div class="vr-dropdown-section" id="{{ctrl.id}}" >'
                                + '<div class="summary-container hand-cursor" type="button" ng-click="ctrl.toggelSectionMenu($event)" ng-class="ctrl.validationContext.validate() != null ? \'invalid-section\':\'\' " >'
                                    + '<div transclude-id="summary" class="summary-main"></div>'
                                    + '<span  class="glyphicon  glyphicon-sort-by-attributes  toogle-icon"></span>'
                                    + '<span ng-if="ctrl.validationContext.validate() != null" class="validation-sign"  title="has validation errors!">*</span>'
                              + '</div>'
                                + '<ul class="dropdown-menu drop-down-section"   id="{{ctrl.menuid}}" ng-show="ctrl.showmenu"  outside-if-not="out-div">'
                                    + '<li role="presentation" >'
                                        + '<span class="hand-cursor pin-icon glyphicon glyphicon glyphicon-pushpin " ng-click="ctrl.toggelPinBody($event)" ng-class="ctrl.pinned==true?\'unpined-icon\':\'\'"></span>'
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



