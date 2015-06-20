
var CustomerController = function ($scope, $http, CarrierAPIService, UtilsService, CarrierTypeEnum) {

    defineScopeObjects();
    defineScopeMethods();
    load();
    function defineScopeObjects() {
        $scope.customers = [];
        $scope.selectedCustomers = [];
    }
    function defineScopeMethods() {

        $scope.isvalidCompCus = function () {
            return ($scope.selectedCustomers.length > 0) ? "" : "required-inpute";
        }

        $scope.initErrortooltip = function () {
            var showmsg = false;
            if ($scope.showt == true) {
                if ($scope.selectedCustomers.length == 0) {
                    $scope.msg = "please select at least one Customer.";
                    showmsg = true;
                }
            }
            return showmsg;
        }
        $scope.subViewConnector.getCarrierAccountSet = function () {
            return {
                $type: "TOne.LCR.Entities.CustomerSelectionSet, TOne.LCR.Entities",
                Customers: {
                    SelectionOption: ($scope.carrierAccountSelectionOption == 1) ? "OnlyItems" : "AllExceptItems",
                    SelectedValues: $scope.getSelectedValues()
                }
            }
        }
        $scope.getSelectedValues = function () {
            var tab = [];
            $.each($scope.selectedCustomers, function (i, value) {
                tab[i] = value.CarrierAccountID;

            });
            return tab;
        }

    }
    function load() {

        var dropdownHidingTimeoutHandlerc;
        $('#dropdownMenuddl').on('show.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideDown();
        });

        //ADD SLIDEUP ANIMATION TO DROPDOWN //
        $('#dropdownMenuddl').on('hide.bs.dropdown', function (e) {
            $(this).find('.dropdown-menu').first().stop(true, true).slideUp();
        });

        $('.dropdown-custom').on('mouseenter', function () {
            var $this = $(this);
            clearTimeout(dropdownHidingTimeoutHandlerc);
            if (!$this.hasClass('open')) {
                $('.dropdown-toggle', $this).dropdown('toggle');
            }
        });
        $('.dropdown-custom').on('mouseleave', function () {
            var $this = $(this);
            dropdownHidingTimeoutHandlerc = setTimeout(function () {
                if ($this.hasClass('open')) {
                    $('.dropdown-toggle', $this).dropdown('toggle');
                }
            }, 150);
        });

        CarrierAPIService.GetCarriers(CarrierTypeEnum.Customer.value).then(function (response) {
            $scope.customers = response;
            if ($scope.routeRule != null) {
                var tab = [];
                $.each($scope.routeRule.CarrierAccountSet.Customers.SelectedValues, function (i, value) {
                    var existobj = UtilsService.getItemByVal($scope.customers, value, 'CarrierAccountID');
                    if (existobj != null)
                        tab[i] = existobj;

                });
                $scope.selectedCustomers = tab;
                $scope.carrierAccountSelectionOption = $scope.routeRule.CarrierAccountSet.Customers.SelectionOption;
            }
            else {
                $scope.carrierAccountSelectionOption = 1;
            }
        })

    }

}
CustomerController.$inject = ['$scope', '$http', 'CarrierAPIService', 'UtilsService', 'CarrierTypeEnum'];
appControllers.controller('RoutingRules_CustomerTemplateController', CustomerController)

