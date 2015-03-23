appControllers.controller('PriorityController',
    function PriorityController($scope, $http) {
        $('#dropdownSup').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#dropdownSup').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        })
        $scope.subViewConnector.getActionData = function () {
            return {
                $type: "TOne.LCR.Entities.PriorityRouteActionData, TOne.LCR.Entities",
                Options: $scope.fillOptionsData()
            }
        }
        $scope.fillOptionsData = function () {
            var tab = [];
            $.each($scope.selectedSuppliers, function (i, value) {
                tab[i] = {
                    SupplierId: value.CarrierAccountID,
                    Priority: ($scope.selectedSuppliers.length ) - i,
                    Force:(value.Force==true)?true:false,
                    Percentage: value.Percentage
                }

            });
            return tab;
        }
        $scope.selectedSuppliers = [];
        $scope.suppliers = [];
        $http.get($scope.baseurl + "/api/BusinessEntity/GetCarriers",
           {
               params: {
                   carrierType: 2
               }
           })
           .success(function (response) {
               $scope.suppliers = response;
               $scope.selectedSuppliers.length = 0;
               if ($scope.routeRule) {
                   var tab = [];
                   $.each($scope.routeRule.ActionData.Options, function (i, value) {
                       var existobj = $scope.findExsiteObj($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                       if (existobj != null) {
                           var existobj = $scope.findExsiteObj($scope.suppliers, value.SupplierId, 'CarrierAccountID')
                           if (existobj != null) {
                               tab[i] = {
                                   CarrierAccountID: value.SupplierId,
                                   Name: existobj.Name,
                                   Force: value.Force,
                                   Percentage: $scope.routeRule.ActionData.Options[i].Percentage,
                                   Priority: value.Priority
                               }
                           }
                       }

                   });
                   $scope.selectedSuppliers = tab;
               }
          });
        
       
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
        $scope.itemsSortable = { handle: '.handeldrag', animation: 150 };
        $scope.selectSupplier = function ($event, s) {
            $event.preventDefault();
            $event.stopPropagation();

            var index = $scope.findExsite($scope.selectedSuppliers, s.CarrierAccountID, 'CarrierAccountID');

            if (index >= 0) {
                $scope.selectedSuppliers.splice(index, 1);
            }
            else {
                $scope.selectedSuppliers.push(s);
            }

        };
       
    });