(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.controller('MainCtrl', ['$scope', function ($scope) {
        
    }]);

    OutOfRangeApp.controller('HomeCtrl', ['$scope', function ($scope) {

    }]);

    OutOfRangeApp.controller('RegisterCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            email: "",
            password: "",
            confirmPassword: ""
        };

        $scope.register = function () {
            Auth.register($scope.user);
        };
    }]);

    OutOfRangeApp.controller('LoginCtrl', ['$scope', 'Auth', function ($scope, Auth) {
        $scope.user = {
            username: "",
            password: ""
        };

        $scope.login = function () {
            debugger;
            Auth.login($scope.user);
        };
    }]);

})();
