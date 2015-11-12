(function (appControllers) {

    "use strict";

    TabsController.$inject = ['$scope'];

    function TabsController($scope) {

        defineScope();
        load();
        function defineScope() {

            $scope.isValid = function (value) {               
                return (value != undefined && value.length > 0) ? null : "Not Valid";
            };


            $scope.isValid2 = function () {
                return "Invlaid ";
            };

            $scope.testModel = "Common_TABSSample TabsController";
            $scope.dynamicTabs = [{
                title: "Appendix 1",
                directive: "vr-common-appendixsample-appendix",
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            }, {
                title: "Appendix 2",
                directive: "vr-common-appendixsample-appendix",
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            }, {
                title: "Appendix 3",
                directive: "vr-common-appendixsample-appendix",
                dontLoad: false,
                loadDirective: function (directiveAPI) {
                    return directiveAPI.load();
                }
            }];
        }

        function load() {



        }
    }

    appControllers.controller('Common_TABSSample_TabsController', TabsController);
})(appControllers);