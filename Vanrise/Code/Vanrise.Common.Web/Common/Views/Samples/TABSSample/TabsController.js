(function (appControllers) {

    "use strict";

    TabsController.$inject = ['$scope'];

    function TabsController($scope) {

        defineScope();
        load();
        function defineScope() {
            $scope.deleteMe = function (itemName) {
                alert('item deleted: ' + itemName);
            };
            $scope.itemsPerRow =3;
            $scope.listItems = [];
            for (var i = 0; i < 10; i++)
                $scope.listItems.push('item ' + i);

            $scope.deleteItem = function (item) {
                var index = $scope.listItems.indexOf(item);
                $scope.listItems.splice(index, 1);
            };

            $scope.removeItem = function (item) {
                var index = $scope.listItems.indexOf(item);
                $scope.listItems.splice(index, 1);
            };

            $scope.isValid = function (value) {               
                return (value != undefined && value.length > 0) ? null : "Not Valid";
            };


            $scope.isValid2 = function () {
                return "Invalid ";
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