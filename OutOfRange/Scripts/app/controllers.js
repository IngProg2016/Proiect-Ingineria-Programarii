(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    OutOfRangeApp.controller('RegisterCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            Email: "",
            Password: "",
            ConfirmPassword: ""
        };

        $scope.register = function () {
            alert();
            Auth.register($scope.user);
        };
    }]);

    OutOfRangeApp.controller('LoginCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            Email: "",
            Password: ""
        };

        $scope.login = function () {
            Auth.login($scope.user);
        };
    }]);

})();
