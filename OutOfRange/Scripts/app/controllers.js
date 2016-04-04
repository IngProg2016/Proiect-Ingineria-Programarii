(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    OutOfRangeApp.controller('RegisterCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            email: "radu.niculcea@gmail.com",
            password: "PENSETA1",
            confirmPassword: "PENSETA1"
        };

        $scope.register = function () {
            Auth.register($scope.user);
        };
    }])

})();
