appControllers.controller('RouteOverrideController',
    function RouteOverrideController($scope, $http) {
        $scope.getSelectSuppliersText = function () {
            var label;
            if ($scope.selectedSuppliers.length == 0)
                label = "Select Suppliers...";
            else if ($scope.selectedSuppliers.length == 1)
                label = $scope.selectedSuppliers[0].Name;
            else if ($scope.selectedSuppliers.length == 2)
                label = $scope.selectedSuppliers[0].Name + "," + $scope.selectedSuppliers[1].Name;
            else if ($scope.selectedSuppliers.length == 3)
                label = $scope.selectedSuppliers[0].Name + "," + $scope.selectedSuppliers[1].Name + "," + $scope.selectedSuppliers[2].Name;
            else
                label = $scope.selectedSuppliers.length + " Suppliers selected";
            if (label.length > 21)
                label = label.substring(0, 20) + "..";
            return label;
        };

        // 
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
          {
              params: {
                  carrierType: 2
              }
          })
      .success(function (response) {
          $scope.suppliers = response;
      });
        $scope.selectedSuppliers = [];
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.selectSupplier = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();
            var index = null;
            try {
                var index = $scope.selectedSuppliers.indexOf(s);
            }
            catch (e) {

            }
            if (index >= 0) {
                $scope.selectedSuppliers.splice(index, 1);
            }
            else
                $scope.selectedSuppliers.push(s);
        };
    });