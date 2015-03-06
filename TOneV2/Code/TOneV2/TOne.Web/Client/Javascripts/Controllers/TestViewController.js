appControllers.controller('TestViewController',
    function DefaultController($scope) {
        $scope.model = 'Test View model';
        $scope.Input = '123';
        $scope.alertMsg = function () {
            alert($scope.Input);
        };
        $scope.items = [
            { name: "Test1", value: "0123" },
            { name: "Test2", value: "1123" },
            { name: "Test3", value: "2123" },
            { name: "Test4", value: "3123" }
        ];

        $scope.header = [
            { name: "Name" }, { name: "Value" }
        ];

    });