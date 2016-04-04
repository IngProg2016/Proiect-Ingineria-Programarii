(function () {
    'use strict';

    var OutOfRangeApp = angular.module('OutOfRangeApp');

    OutOfRangeApp.factory('Auth', ['$resource', function ($resource) {
        var Register = $resource('api/account/register');
        return {
            register: function (user) {
                var _user = new Register(user);
                _user.$save();
            }
        }
    }])

})();