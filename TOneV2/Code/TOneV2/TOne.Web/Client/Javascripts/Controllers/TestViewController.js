appControllers.controller('TestViewController',
    function DefaultController($scope) {
        $scope.model = 'Test View model';
        $scope.Input = '123';
        $scope.alertMsg = function () {
            alert($scope.Input);
        };

        $scope.items = [{ name: "Moroni", age: 50 },
                        { name: "Tiancum", age: 43 },
                        { name: "Jacob", age: 27 },
                        { name: "Nephi", age: 29 },
                        { name: "Enos", age: 34 },
                        { name: "Tiancum", age: 43 },
                        { name: "Jacob", age: 27 },
                        { name: "Nephi", age: 29 },
                        { name: "Enos", age: 34 },
                        { name: "Tiancum", age: 43 },
                        { name: "Jacob", age: 27 },
                        { name: "Nephi", age: 29 },
                        { name: "Enos", age: 34 },
                        { name: "Tiancum", age: 43 },
                        { name: "Jacob", age: 27 },
                        { name: "Nephi", age: 29 },
                        { name: "Enos", age: 34 }];


        $scope.header = [
            { name: "Name" }, { name: "Age" }
        ];

    });