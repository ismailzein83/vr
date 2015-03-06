appControllers.controller('TestViewController',
    function DefaultController($scope) {
        $scope.model = 'Test View model';
        $scope.Input = '123';
        $scope.alertMsg = function () {
            alert($scope.Input);
        };

        $scope.gridOptions = {
            data:[{ name: "Moroni", age: 50 },
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
                { name: "Enos", age: 34 }],
            header : ["Name","Age"]
        };


        $scope.gridOptionsV1 = {
            data: [{ name: "Moroni", age: 50 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 },
                { name: "Tiancum", age: 43 },
                { name: "Jacob", age: 27 },
                { name: "Nephi", age: 29 },
                { name: "Enos", age: 34 }]
        };

    });