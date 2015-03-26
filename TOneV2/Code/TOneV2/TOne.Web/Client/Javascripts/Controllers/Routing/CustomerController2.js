
var CustomerController2 = function ($scope, $http, CarriersService) {

    defineScopeObjects();
    defineScopeMethods();
    load();
    function defineScopeObjects() {
        $scope.optionsCustomers = {
            selectedvalues: [],
            datasource: []
        };
    }
    function defineScopeMethods() {

        $scope.isvalidCompCus = function () {
            return ($scope.optionsCustomers.selectedvalues.length > 0) ? "" : "required-inpute";
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
            $.each($scope.optionsCustomers.selectedvalues, function (i, value) {
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

        CarriersService.getCustomers().then(function (response) {
            $scope.optionsCustomers.datasource = response;
            if ($scope.routeRule != null) {
                var tab = [];
                $.each($scope.routeRule.CarrierAccountSet.Customers.SelectedValues, function (i, value) {
                    var existobj = $scope.findExsiteObj($scope.optionsCustomers.datasource, value, 'CarrierAccountID')
                    if (existobj != null)
                        tab[i] = existobj;

                });
                $scope.optionsCustomers.selectedvalues = tab;
                $scope.carrierAccountSelectionOption = $scope.routeRule.CarrierAccountSet.Customers.SelectionOption;
            }
            else {
                $scope.carrierAccountSelectionOption = 1;
            }
        })

    }

}
CustomerController2.$inject = ['$scope', '$http', 'CarriersService'];
appControllers.controller('CustomerController2', CustomerController2)

